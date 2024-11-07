namespace Diagnostics.Generator.Core.Annotations
{
    public enum DiagnosticGenerateTypes
    {
        Event = 1,
        Activity = Event << 1,
        Log = Event << 2
    }
}
