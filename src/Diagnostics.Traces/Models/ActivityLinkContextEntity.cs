﻿using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Diagnostics.Traces.Models
{
    [ExcludeFromCodeCoverage]
    public record class ActivityLinkContextEntity
    {
        public string? TraceId { get; set; }

        public string? TraceState { get; set; }

        public ActivityTraceFlags TraceFlags { get; set; }

        public bool IsRemote { get; set; }

        public string? SpanId { get; set; }
    }
}
