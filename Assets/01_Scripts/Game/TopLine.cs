using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Melon.Game {
    [RequireComponent(typeof(BoxCollider2D))]
    public class TopLine : MonoBehaviour {
        [Title("Rule")]
        [SerializeField, Min(0.5f)]
        float dangerSeconds = 5f;
        [ShowInInspector, ReadOnly]
        bool dangerActive;

        [Title("Physic")]
        [SerializeField]
        BoxCollider2D b2d;
        [ShowInInspector]
        readonly HashSet<FruitCtrl> overlaps = new();

        [Title("UI")]
        [SerializeField]
        GameObject lineRender;

        UniTaskCompletionSource<bool> signal;


        private void StartDanger() {
            if (dangerActive) return;
            signal = new UniTaskCompletionSource<bool>();
            dangerActive = true;
            lineRender.SetActive(true);
            Countdown().Forget();
        }

        private void CancelDanger() {
            dangerActive = false;
            lineRender.SetActive(false);
            signal?.TrySetResult(true);
            signal = null;
        }

        private async UniTaskVoid Countdown() {
            bool? res = await Util.Async.Timer.Wait(
                tcs: signal,
                seconds: dangerSeconds,
                ct: this.GetCancellationTokenOnDestroy(),
                timing: PlayerLoopTiming.Update);

            signal = null;
            dangerActive = false;

            if (res is null && overlaps.Count > 0) {
                GameManager.Instance.GameOver();
                return;
            }

            if (overlaps.Count > 0 && !dangerActive) {
                StartDanger();
            }
        }

        #region Physic Event
        private void OnTriggerEnter2D(Collider2D other) {
            if (!other.CompareTag("Fruit"))
                return;
            var fruit = other.GetComponent<FruitCtrl>();
            if (!overlaps.Contains(fruit)) {
                overlaps.Add(fruit);
                fruit.OnDoneSpawn += _CheckOverlapsSpawn;
                if (fruit.DoneSpawn && overlaps.Count > 1)
                    StartDanger();
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (!other.CompareTag("Fruit"))
                return;
            var fruit = other.GetComponentInParent<FruitCtrl>();
            if (overlaps.Contains(fruit)) {
                overlaps.Remove(fruit);
                fruit.OnDoneSpawn -= _CheckOverlapsSpawn;
                if (overlaps.Count == 0)
                    CancelDanger();
            }
        }

        private void _CheckOverlapsSpawn(FruitCtrl fruit) {
            if (!overlaps.Contains(fruit)) return;
            if (overlaps.Count > 0)
                StartDanger();
        }
        #endregion


#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            var bc = GetComponent<BoxCollider2D>();
            Gizmos.color = Color.red;
            var m = transform.localToWorldMatrix;
            var center = m.MultiplyPoint(bc.offset);
            var size = Vector3.Scale(bc.size, transform.lossyScale);
            Gizmos.DrawWireCube(center, size);
        }
#endif
    }
}
