﻿using System;

namespace Diagnostics.Generator.Core.Annotations
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class MeterRecordAttribute : Attribute
    {
        public MeterRecordAttribute(string meterVisitName)
        {
            MeterVisitName = meterVisitName ?? throw new ArgumentNullException(nameof(meterVisitName));
        }

        public string MeterVisitName { get; }
    }
}
