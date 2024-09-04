using System;

namespace Diagnostics.Generator.Core.Annotations
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class MapToEventSourceAttribute : Attribute
    {
        public MapToEventSourceAttribute(Type eventSourceType)
        {
            EventSourceType = eventSourceType ?? throw new ArgumentNullException(nameof(eventSourceType));
        }

        public Type EventSourceType { get; }

        //public bool GenerateIds { get; set; }

        //public int GenerateIdStart { get; set; }

        //public Accessibility GenerateIdClassAccessibility { get; set; }
    }
}
