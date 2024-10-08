﻿using Diagnostics.Helpers.DotNetHeapDump;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Diagnostics.Helpers
{

    public static class GcDumpHelper
    {
        public static void WriteGcDump(int processId, string filePath, int timeout = 30, TextWriter? log = null, bool isVeryLargeGraph = false, int expectedSize = 50_000, CancellationToken token = default)
        {
            using (var fs = File.Open(filePath, FileMode.Create))
            {
                WriteGcDump(processId, fs, timeout, log, false, isVeryLargeGraph,expectedSize, token);
            }
        }
        public static void WriteGcDump(int processId, Stream outputStream, int timeout = 30, TextWriter? log = null,bool leaveOpen=false, bool isVeryLargeGraph=false, int expectedSize = 50_000, CancellationToken token = default)
        {
            if (TryCollectMemoryGraph(processId, timeout, log, out MemoryGraph memoryGraph, isVeryLargeGraph,expectedSize, token))
            {
                GCHeapDump.WriteMemoryGraph(memoryGraph, outputStream, leaveOpen: leaveOpen);
            }
        }
        public static bool TryCollectMemoryGraph(int processId, int timeout, TextWriter? log, out MemoryGraph memoryGraph,bool isVeryLargeGraph = false,int expectedSize=50_000, CancellationToken ct=default)
        {
            DotNetHeapInfo heapInfo = new();
            log ??= TextWriter.Null;

            memoryGraph = new MemoryGraph(expectedSize, isVeryLargeGraph);

            if (!EventPipeDotNetHeapDumper.DumpFromEventPipe(ct, processId, memoryGraph, log, timeout, heapInfo))
            {
                return false;
            }

            memoryGraph.AllowReading();
            return true;
        }
        public static async Task WriteAsync(this MemoryGraph memoryGraph,TextWriter writer)
        {
            // Print summary
            await WriteSummaryRowAsync(memoryGraph.TotalSize, "GC Heap bytes",writer);
            await WriteSummaryRowAsync(memoryGraph.NodeCount, "GC Heap objects", writer);

            if (memoryGraph.TotalNumberOfReferences > 0)
            {
                await WriteSummaryRowAsync(memoryGraph.TotalNumberOfReferences, "Total references", writer);
            }

            await writer.WriteLineAsync();

            // Print Details
            await writer.WriteAsync($"{"Object Bytes",15:N0}");
            await writer.WriteAsync($"  {"Count",8:N0}");
            await writer.WriteAsync("  Type");
            await writer.WriteLineAsync();

            IOrderedEnumerable<ReportItem> filteredTypes = GetReportItem(memoryGraph)
                .OrderByDescending(t => t.SizeBytes)
                .ThenByDescending(t => t.Count);

            foreach (ReportItem filteredType in filteredTypes)
            {
                await filteredType.WriteToAsync(writer);
                await writer.WriteLineAsync();
            }
        }
        static async ValueTask WriteSummaryRowAsync(object value, string text,TextWriter writer)
        {
            await writer.WriteAsync($"{value,15:N0}  ");
            await writer.WriteAsync(text);
            await writer.WriteLineAsync();
        }

        public static IEnumerable<ReportItem> EnumerableReportItem(MemoryGraph memoryGraph)
        {
            Graph.SizeAndCount[] histogramByType = memoryGraph.GetHistogramByType();
            for (int index = 0; index < memoryGraph.m_types.Count; index++)
            {
                Graph.TypeInfo type = memoryGraph.m_types[index];
                if (string.IsNullOrEmpty(type.Name) || type.Size == 0)
                {
                    continue;
                }

                Graph.SizeAndCount? sizeAndCount = histogramByType.FirstOrDefault(c => (int)c.TypeIdx == index);
                if (sizeAndCount == null || sizeAndCount.Count == 0)
                {
                    continue;
                }

                yield return new ReportItem(sizeAndCount.Count, type.Size, type.Name, type.ModuleName);
            }
        }
        public static GcDumpReport GetReportItem(MemoryGraph memoryGraph)
        {
            return new GcDumpReport(EnumerableReportItem(memoryGraph));
        }
    }
    public readonly struct GcDumpReport:IEnumerable<ReportItem>
    {
        public GcDumpReport(IEnumerable<ReportItem> reportItems)
        {
            ReportItems = reportItems;
        }

        public IEnumerable<ReportItem> ReportItems { get; }

        public IEnumerable<ReportItem> GetMax(int length)
        {
            return ReportItems.OrderByDescending(t => t.SizeBytes)
                .ThenByDescending(t => t.Count).Take(length);
        }

        public IEnumerator<ReportItem> GetEnumerator()
        {
            return ReportItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    public readonly record struct ReportItem
    {
        public ReportItem(int? count, long sizeBytes, string typeName, string moduleName)
        {
            Count = count;
            SizeBytes = sizeBytes;
            TypeName = typeName;
            ModuleName = moduleName;
        }

        public int? Count { get; }
        public long SizeBytes { get; }
        public string TypeName { get; }
        public string ModuleName { get; }

        public async Task WriteToAsync(TextWriter writer)
        {
            await writer.WriteAsync($"{SizeBytes,15:N0}");
            await writer.WriteAsync("  ");
            if (Count.HasValue)
            {
                await writer.WriteAsync($"{Count.Value,8:N0}");
                await writer.WriteAsync("  ");
            }
            else
            {
                await writer.WriteAsync($"{"",8}  ");
            }

            await writer.WriteAsync(TypeName ?? "<UNKNOWN>");
            var dllName = GetDllName(ModuleName ?? "");
            if (dllName.Length != 0)
            {
                await writer.WriteAsync("  ");
                await writer.WriteAsync('[');
                await writer.WriteAsync(GetDllName(ModuleName ?? ""));
                await writer.WriteAsync(']');
            }
            static string GetDllName(string input)
                => input.Substring(input.LastIndexOf(Path.DirectorySeparatorChar) + 1);
        }
        public void WriteTo(TextWriter writer)
        {
            WriteToAsync(writer).GetAwaiter().GetResult();
        }
    }

}
