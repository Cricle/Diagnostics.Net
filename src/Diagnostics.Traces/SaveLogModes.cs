﻿namespace Diagnostics.Traces
{
    public static class SaveModeValues
    {
        public static readonly IReadOnlyList<SaveLogModes> AllSaveLogModes =
        [
             SaveLogModes.Timestamp,
             SaveLogModes.LogLevel,
             SaveLogModes.CategoryName,
             SaveLogModes.TraceId,
             SaveLogModes.SpanId,
             SaveLogModes.Attributes,
             SaveLogModes.FormattedMessage,
             SaveLogModes.Body
        ];

        public static readonly IReadOnlyList<SaveExceptionModes> AllSaveExceptionModes = 
        [
           SaveExceptionModes.TraceId ,
           SaveExceptionModes.SpanId ,
           SaveExceptionModes.CreateTime ,
           SaveExceptionModes.TypeName ,
           SaveExceptionModes.Message ,
           SaveExceptionModes.HelpLink ,
           SaveExceptionModes.HResult ,
           SaveExceptionModes.Data ,
           SaveExceptionModes.StackTrace ,
           SaveExceptionModes.InnerException ,
        ];

        public static readonly IReadOnlyList<SaveActivityModes> AllSaveActivityModes =
        [
            SaveActivityModes.Id ,
            SaveActivityModes.Status ,
            SaveActivityModes.StatusDescription ,
            SaveActivityModes.HasRemoteParent ,
            SaveActivityModes.Kind ,
            SaveActivityModes.OperationName ,
            SaveActivityModes.DisplayName ,
            SaveActivityModes.SourceName ,
            SaveActivityModes.SourceVersion ,
            SaveActivityModes.Duration ,
            SaveActivityModes.StartTimeUtc ,
            SaveActivityModes.ParentId ,
            SaveActivityModes.RootId ,
            SaveActivityModes.Tags ,
            SaveActivityModes.Events ,
            SaveActivityModes.Links ,
            SaveActivityModes.Baggage ,
            SaveActivityModes.Context ,
            SaveActivityModes.TraceStateString ,
            SaveActivityModes.SpanId ,
            SaveActivityModes.TraceId ,
            SaveActivityModes.Recorded ,
            SaveActivityModes.ActivityTraceFlags ,
            SaveActivityModes.ParentSpanId ,
        ];
    }

    public enum SaveLogModes
    {
        Timestamp = 1,
        LogLevel = Timestamp << 1,
        CategoryName = Timestamp << 2,
        TraceId = Timestamp << 3,
        SpanId = Timestamp << 4,
        Attributes = Timestamp << 5,
        FormattedMessage = Timestamp << 6,
        Body = Timestamp << 7,
        Mini = Timestamp | TraceId | SpanId | FormattedMessage,
        All = Timestamp | LogLevel | CategoryName | TraceId | SpanId | Attributes | FormattedMessage | Body
    }
    public enum SaveExceptionModes
    {
        TraceId = 1,
        SpanId = TraceId << 1,
        CreateTime = TraceId << 2,
        TypeName = TraceId << 3,
        Message = TraceId << 4,
        HelpLink = TraceId << 5,
        HResult = TraceId << 6,
        Data = TraceId << 7,
        StackTrace = TraceId << 8,
        InnerException = TraceId << 9,
        Mini = TraceId | SpanId | Message | StackTrace,
        All = TraceId | SpanId | CreateTime | TypeName | Message | HelpLink | HResult | Data | StackTrace | InnerException
    }
    public enum SaveActivityModes
    {
        Id = 1,
        Status = Id << 1,
        StatusDescription = Id << 2,
        HasRemoteParent = Id << 3,
        Kind = Id << 4,
        OperationName = Id << 5,
        DisplayName = Id << 6,
        SourceName = Id << 7,
        SourceVersion = Id << 8,
        Duration = Id << 9,
        StartTimeUtc = Id << 10,
        ParentId = Id << 11,
        RootId = Id << 12,
        Tags = Id << 13,
        Events = Id << 14,
        Links = Id << 15,
        Baggage = Id << 16,
        Context = Id << 17,
        TraceStateString = Id << 18,
        SpanId = Id << 19,
        TraceId = Id << 20,
        Recorded = Id << 21,
        ActivityTraceFlags = Id << 22,
        ParentSpanId = Id << 23,
        Mini = Status | OperationName | DisplayName | SourceName | Duration | StartTimeUtc | ParentId | RootId | Tags | Events | SpanId | TraceId | ActivityTraceFlags | ParentSpanId,
        All = Id | Status | StatusDescription | HasRemoteParent | Kind | OperationName | DisplayName | SourceName | SourceVersion | Duration | StartTimeUtc | ParentId | RootId | Tags | Events | Links | Baggage | Context | TraceStateString | SpanId | TraceId | Recorded | ActivityTraceFlags | ParentSpanId
    }
}
