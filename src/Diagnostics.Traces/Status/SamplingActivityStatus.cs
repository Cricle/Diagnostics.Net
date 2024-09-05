using System.Collections.Concurrent;
using System.Diagnostics;

namespace Diagnostics.Traces.Status
{
    public class SamplingActivityStatus : ActivityStatus, IDisposable
    {
        private readonly ConcurrentDictionary<Activity, Activity> buffer;

        public int BufferSize => buffer.Count;

        public IEnumerable<Activity> Buffer => buffer.Keys;

        public SamplingActivityStatus(ActivitySource source)
            : base(source)
        {
            buffer = new ConcurrentDictionary<Activity, Activity>();
        }

        public SamplingActivityStatus(string sourceName)
            : base(sourceName)
        {
            buffer = new ConcurrentDictionary<Activity, Activity>();
        }

        public SamplingActivityStatus(Func<ActivitySource, bool> sourceFun)
            : base(sourceFun)
        {
            buffer = new ConcurrentDictionary<Activity, Activity>();
        }

        public bool Contains(Activity activity)
        {
            return buffer.ContainsKey(activity);
        }

        protected override void OnActivityStarted(Activity activity)
        {
            buffer[activity] = activity;
        }

        protected override void OnActivityStoped(Activity activity)
        {
            buffer.TryRemove(activity, out _);
        }
    }
}
