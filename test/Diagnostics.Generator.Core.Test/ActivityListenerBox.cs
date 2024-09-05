using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Diagnostics.Generator.Core.Test
{
    [ExcludeFromCodeCoverage]
    internal class ActivityListenerBox : IDisposable
    {
        public ActivityListenerBox(string name)
        {
            Listener = new ActivityListener
            {
                ActivityStarted = activity => currentActivity = activity,
                ShouldListenTo = s => s.Name == name,
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
                SampleUsingParentId = (ref ActivityCreationOptions<string> _) => ActivitySamplingResult.AllData,
            };
            ActivitySource.AddActivityListener(Listener);
        }

        [ThreadStatic]
        private static Activity? currentActivity;

        public static Activity? CurrentActivity => currentActivity;

        public ActivityListener Listener { get; }

        public void Dispose()
        {
            Listener.Dispose();
            currentActivity = null;
        }
    }
}
