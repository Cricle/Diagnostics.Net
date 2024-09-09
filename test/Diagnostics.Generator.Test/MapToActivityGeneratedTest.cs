using System.Diagnostics;

namespace Diagnostics.Generator.Test
{
    [TestClass]
    public class MapToActivityGeneratedTest
    {
        static MapToActivityGeneratedTest()
        {
            _ = new ActivitySource("null");//Activity the assembly
        }

        [TestMethod]
        public void GenerateMapToActivity()
        {
            var code = """
                using System.Diagnostics;
                using System.Diagnostics.Tracing;
                using Diagnostics.Generator.Core.Annotations;

                namespace evev
                {
                    [EventSourceGenerate(GenerateSingleton = true)]
                    [MapToActivity(typeof(TestActivity), WithEventSourceCall = true)]
                    internal unsafe partial class TestEventSource : EventSource
                    {
                        [Event(1)]
                        public partial void Event0(string? arg0);
                    }

                    internal unsafe partial class TestActivity
                    {
                        private static readonly ActivitySource source = new ActivitySource("test");
                    }
                }
                """;

            Compiler.CheckGeneratedMore(code, [
                (typeof(ActivityMapGenerator),"TestEventSource.ActivityMap.g.cs","MapToActivity.Event0.Activity.txt"),
                (typeof(EventSourceGenerator),"TestEventSource.g.cs","MapToActivity.Event0.Event.txt"),
                ]);
        }

        [TestMethod]
        public void GenerateMapToActivity_NoNamespace()
        {
            var code = """
                using System.Diagnostics;
                using System.Diagnostics.Tracing;
                using Diagnostics.Generator.Core.Annotations;

                [EventSourceGenerate(GenerateSingleton = true)]
                [MapToActivity(typeof(TestActivity), WithEventSourceCall = true)]
                internal unsafe partial class TestEventSource : EventSource
                {
                    [Event(1)]
                    public partial void Event0(string? arg0);
                }

                internal unsafe partial class TestActivity
                {
                    private static readonly ActivitySource source = new ActivitySource("test");
                }                
                """;

            Compiler.CheckGeneratedMore(code, [
                (typeof(ActivityMapGenerator),"TestEventSource.ActivityMap.g.cs","MapToActivity.NoNs.Event0.Activity.txt"),
                (typeof(EventSourceGenerator),"TestEventSource.g.cs","MapToActivity.NoNs.Event0.Event.txt"),
                ]);
        }
    }
}
