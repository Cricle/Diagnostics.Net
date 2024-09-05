using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Diagnostics.CodeAnalysis;

namespace Diagnostics.Generator.Test
{
    [ExcludeFromCodeCoverage]
    internal static class Baselines
    {
        public static SourceText GetBaselineNode(string fileName)
        {
            var text = GetBaseline(fileName);
            return SourceText.From(text);
        }
        public static string GetBaseline(string fileName)
        {
            using (var stream = typeof(Baselines).Assembly.GetManifestResourceStream($"Diagnostics.Generator.Test.Baselines.{fileName}"))
            {
                if (stream==null)
                {
                    throw new ArgumentException($"No base line file {fileName}");
                }
                return new StreamReader(stream).ReadToEnd();
            }
        }
    }
}
