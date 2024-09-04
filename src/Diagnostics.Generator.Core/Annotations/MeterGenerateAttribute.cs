using System;

namespace Diagnostics.Generator.Core.Annotations
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class MeterGenerateAttribute : Attribute
    {
    }
}
