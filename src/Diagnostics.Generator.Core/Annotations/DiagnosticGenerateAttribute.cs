using System;

namespace Diagnostics.Generator.Core.Annotations
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class DiagnosticGenerateAttribute : Attribute
    {
        public DiagnosticGenerateAttribute(DiagnosticGenerateTypes generateTypes)
        {
            GenerateTypes = generateTypes;
        }

        public DiagnosticGenerateTypes GenerateTypes { get; }
    }
}
