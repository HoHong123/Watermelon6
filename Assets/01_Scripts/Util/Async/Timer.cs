using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Util.Async {
    public class Timer {
        public static async UniTask<bool?> Wait(
            UniTaskCompletionSource<bool> tcs,
            float seconds,
            CancellationToken ct = default,
            bool unscaled = false,
            PlayerLoopTiming timing = PlayerLoopTiming.Update) {
            var signal = tcs.Task.AttachExternalCancellation(ct).Preserve();
            var delay = UniTask.Delay(
                TimeSpan.FromSeconds(seconds),
                delayType: unscaled ? DelayType.UnscaledDeltaTime : DelayType.DeltaTime,
                delayTiming: PlayerLoopTiming.Update,
                cancellationToken: ct);

            var (winner, taskResult) = await UniTask.WhenAny(signal, delay);

            return winner ? taskResult : null;
        }
    }
}