using System;

namespace Diagnostics.Generator.Core.Annotations
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class EventSourceGenerateAttribute : Attribute
    {
        public bool IncludeInterface { get; set; }

        public Accessibility InterfaceAccessibility { get; set; }

        public bool GenerateSingleton { get; set; }

        public bool UseIsEnable { get; set; } = true;
    }
}
