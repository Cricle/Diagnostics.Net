using System;

namespace Diagnostics.Generator.Core.Annotations
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class RelatedActivityIdAttribute : Attribute
    {
    }
}
