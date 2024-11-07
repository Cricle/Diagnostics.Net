using Microsoft.CodeAnalysis;

namespace Diagnostics.Generator.Internal
{
    internal class GeneratorTransformResult<T>
    {
        public GeneratorTransformResult(T value, GeneratorAttributeSyntaxContext syntaxContext)
        {
            Value = value;
            SyntaxContext = syntaxContext;
        }

        public T Value { get; }

        public SemanticModel SemanticModel => SyntaxContext.SemanticModel;

        public GeneratorAttributeSyntaxContext SyntaxContext { get; }

        public IAssemblySymbol AssemblySymbol => SyntaxContext.SemanticModel.Compilation.Assembly;

        public bool IsAutoGen()
        {
            return ParserBase.IsAutoGen(SyntaxContext.TargetSymbol);
        }
        public string GetNameSpace()
        {
            return ParserBase.GetNameSpace(SyntaxContext.TargetSymbol);
        }
        public string GetTypeFullName()
        {
            return ParserBase.GetTypeFullName(SyntaxContext.TargetSymbol);
        }
        public void GetWriteNameSpace(SemanticModel model,out string nameSpaceStart, out string nameSpaceEnd)
        {
            ParserBase.GetWriteNameSpace(SyntaxContext.TargetSymbol,model, out nameSpaceStart, out nameSpaceEnd);
        }
        public string GetAccessibilityString()
        {
            return ParserBase.GetAccessibilityString(SyntaxContext.TargetSymbol.DeclaredAccessibility);
        }
    }
}
