using Diagnostics.Generator.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Diagnostics.Generator.Test
{
    [ExcludeFromCodeCoverage]
    internal static class Compiler
    {
        private static readonly CSharpCompilationOptions compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true, nullableContextOptions: NullableContextOptions.Enable);

        private static readonly MetadataReference[] metadataReferences = new[]
                {
                    MetadataReference.CreateFromFile(typeof(Attribute).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(InterlockedHelper).GetTypeInfo().Assembly.Location),
                }.Concat(AppDomain.CurrentDomain.GetAssemblies().Where(x =>
                {
                    try
                    {
                        return !string.IsNullOrEmpty(x.Location);
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }).Select(x => MetadataReference.CreateFromFile(x.Location))).ToArray();

        public static Compilation CreateCompilation(string source)
            => CSharpCompilation.Create("compilation",
                new[] { CSharpSyntaxTree.ParseText(source) },
                metadataReferences,
                compilationOptions);

        public static GeneratorDriverRunResult CheckGenerated(string code, int willGeneratedFile)
        {
            var compiler = CreateCompilation(code);

            GeneratorDriver driver = CSharpGeneratorDriver.Create(
                new EventSourceGenerator(),
                new ActivityMapGenerator(),
                new ActivityAsGenerator(),
                new CounterMappingGenerator(),
                new MeterMethodGenerator(),
                new MapToEventSourceGenerator());
            driver = driver.RunGeneratorsAndUpdateCompilation(compiler, out var outputCompilation, out var diagnostics);

            Assert.IsTrue(diagnostics.IsEmpty); // there were no diagnostics created by the generators
            Assert.IsTrue(outputCompilation.GetDiagnostics().IsEmpty, string.Join("\n", outputCompilation.GetDiagnostics().Select(x => x.ToString()))); // verify the compilation with the added source has no diagnostics

            GeneratorDriverRunResult runResult = driver.GetRunResult();

            Assert.AreEqual(willGeneratedFile, runResult.GeneratedTrees.Length);
            Assert.IsTrue(runResult.Diagnostics.IsEmpty);

            return runResult;
        }

        public static void CheckGeneratedSingle(string code, string baseLineFileName)
        {
            var result = CheckGenerated(code, 1);

            var baseLine = Baselines.GetBaselineNode(baseLineFileName);
            GeneratorRunResult generatorResult = result.Results[0];
            Assert.IsTrue(generatorResult.GeneratedSources[0].SourceText.ContentEquals(baseLine));
        }

        public static void CheckGeneratedMore(string code, List<(Type sourceGenerateType, string hitName, string baseLineFileName)> equals)
        {
            var result = CheckGenerated(code, equals.Count);

            foreach (var item in equals)
            {
                var gen = result.Results.FirstOrDefault(x => x.Generator.GetGeneratorType()== item.sourceGenerateType);

                if (gen.Generator == null)
                {
                    Assert.Fail($"Can't find {item.sourceGenerateType} generated result");
                }

                var baseLine = Baselines.GetBaselineNode(item.baseLineFileName);

                var fi = gen.GeneratedSources.FirstOrDefault(x => x.HintName == item.hitName);

                if (fi.HintName != item.hitName)
                {
                    Assert.Fail($"Can't find {item.sourceGenerateType} hitName {item.hitName} file");
                }

                Assert.IsTrue(fi.SourceText.ContentEquals(baseLine));
            }
        }
    }
}
