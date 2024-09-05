namespace Diagnostics.Generator.Test
{
    [TestClass]
    public class EventSourceGeneratedTest
    {
        [TestMethod]
        public void GenerateSingleton()
        {
            var code = """
                using System.Diagnostics.Tracing;
                using Diagnostics.Generator.Core.Annotations;

                namespace evev
                {       
                    [EventSourceGenerate(GenerateSingleton = true)]
                    internal unsafe partial class TestEventSource : EventSource
                    {
                    }
                }
                """;

            Compiler.CheckGeneratedSingle(code, "TestEventSource.Singleton.txt");
        }
        [TestMethod]
        public void GenerateSingleton_NoNamespace()
        {
            var code = """
                using System.Diagnostics.Tracing;
                using Diagnostics.Generator.Core.Annotations;

                [EventSourceGenerate(GenerateSingleton = true)]
                internal unsafe partial class TestEventSource : EventSource
                {
                }
                """;

            Compiler.CheckGeneratedSingle(code, "TestEventSource.NoNs.Singleton.txt");
        }

        [TestMethod]
        public void GenerateEmptyEvent()
        {
            var code = """
                using System.Diagnostics.Tracing;
                using Diagnostics.Generator.Core.Annotations;

                namespace evev
                {
                    [EventSourceGenerate]
                    internal unsafe partial class TestEventSource : EventSource
                    {
                        [Event(1)]
                        public partial void Event0();
                    }
                }
                """;

            Compiler.CheckGeneratedSingle(code, "TestEventSource.Event0.txt");
        }

        [TestMethod]
        public void GenerateEmptyEvent_NoNamespace()
        {
            var code = """
                using System.Diagnostics.Tracing;
                using Diagnostics.Generator.Core.Annotations;

                [EventSourceGenerate]
                internal unsafe partial class TestEventSource : EventSource
                {
                    [Event(1)]
                    public partial void Event0();
                }
                """;

            Compiler.CheckGeneratedSingle(code, "TestEventSource.NoNs.Event0.txt");
        }

        [TestMethod]
        public void GenerateOnePrimitive()
        {
            var code = """
                using System.Diagnostics.Tracing;
                using Diagnostics.Generator.Core.Annotations;

                namespace evev
                {
                    [EventSourceGenerate]
                    internal unsafe partial class TestEventSource : EventSource
                    {
                        [Event(1)]
                        public partial void Event0(int arg0);
                    }
                }
                """;

            Compiler.CheckGeneratedSingle(code, "TestEventSource.Event1.txt");
        }

        [TestMethod]
        public void GenerateOnePrimitive_NoNamespace()
        {
            var code = """
                using System.Diagnostics.Tracing;
                using Diagnostics.Generator.Core.Annotations;

                [EventSourceGenerate]
                internal unsafe partial class TestEventSource : EventSource
                {
                    [Event(1)]
                    public partial void Event0(int arg0);
                }                
                """;

            Compiler.CheckGeneratedSingle(code, "TestEventSource.NoNs.Event1.txt");
        }

        [TestMethod]
        public void GenerateTwoPrimitive()
        {
            var code = """
                using System.Diagnostics.Tracing;
                using Diagnostics.Generator.Core.Annotations;

                namespace evev
                {
                    [EventSourceGenerate]
                    internal unsafe partial class TestEventSource : EventSource
                    {
                        [Event(1)]
                        public partial void Event0(int arg0, double arg1);
                    }
                }
                """;

            Compiler.CheckGeneratedSingle(code, "TestEventSource.Event2.txt");
        }

        [TestMethod]
        public void GenerateTwoPrimitive_NoNamespace()
        {
            var code = """
                using System.Diagnostics.Tracing;
                using Diagnostics.Generator.Core.Annotations;

                [EventSourceGenerate]
                internal unsafe partial class TestEventSource : EventSource
                {
                    [Event(1)]
                    public partial void Event0(int arg0, double arg1);
                }
                """;

            Compiler.CheckGeneratedSingle(code, "TestEventSource.NoNs.Event2.txt");
        }

        [TestMethod]
        public void GenerateOneString()
        {
            var code = """
                using System.Diagnostics.Tracing;
                using Diagnostics.Generator.Core.Annotations;

                namespace evev
                {
                    [EventSourceGenerate]
                    internal unsafe partial class TestEventSource : EventSource
                    {
                        [Event(1)]
                        public partial void Event0(string arg0);
                    }
                }
                """;

            Compiler.CheckGeneratedSingle(code, "TestEventSource.Event3.txt");
        }

        [TestMethod]
        public void GenerateOneString_NoNamespace()
        {
            var code = """
                using System.Diagnostics.Tracing;
                using Diagnostics.Generator.Core.Annotations;

                [EventSourceGenerate]
                internal unsafe partial class TestEventSource : EventSource
                {
                    [Event(1)]
                    public partial void Event0(string arg0);
                }                
                """;

            Compiler.CheckGeneratedSingle(code, "TestEventSource.NoNs.Event3.txt");
        }

        [TestMethod]
        public void GenerateTwoString()
        {
            var code = """
                using System.Diagnostics.Tracing;
                using Diagnostics.Generator.Core.Annotations;

                namespace evev
                {
                    [EventSourceGenerate]
                    internal unsafe partial class TestEventSource : EventSource
                    {
                        [Event(1)]
                        public partial void Event0(string arg0, string arg1);
                    }
                }
                """;

            Compiler.CheckGeneratedSingle(code, "TestEventSource.Event4.txt");
        }

        [TestMethod]
        public void GenerateTwoString_NoNamespace()
        {
            var code = """
                using System.Diagnostics.Tracing;
                using Diagnostics.Generator.Core.Annotations;

                [EventSourceGenerate]
                internal unsafe partial class TestEventSource : EventSource
                {
                    [Event(1)]
                    public partial void Event0(string arg0, string arg1);
                }                
                """;

            Compiler.CheckGeneratedSingle(code, "TestEventSource.NoNs.Event4.txt");
        }
    }
}
