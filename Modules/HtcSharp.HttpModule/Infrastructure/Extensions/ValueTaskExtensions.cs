using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace HtcSharp.HttpModule.Infrastructure.Extensions {
    internal static class ValueTaskExtensions {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task GetAsTask(this in ValueTask<FlushResult> valueTask) {
            // Try to avoid the allocation from AsTask
            if (valueTask.IsCompletedSuccessfully) {
                // Signal consumption to the IValueTaskSource
                valueTask.GetAwaiter().GetResult();
                return Task.CompletedTask;
            } else {
                return valueTask.AsTask();
            }
        }
    }
}