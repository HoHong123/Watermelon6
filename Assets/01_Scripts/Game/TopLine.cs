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
            Debug.Log($"StartDanger");
            if (dangerActive) return;
            signal = new UniTaskCompletionSource<bool>();
            dangerActive = true;
            lineRender.SetActive(true);
            Countdown().Forget();
        }

        private void CancelDanger() {
            Debug.Log($"CancelDanger");
            dangerActive = false;
            lineRender.SetActive(false);
            signal?.TrySetResult(true);
            signal = null;
        }

        private async UniTaskVoid Countdown() {
            Debug.Log($"Countdown");
            bool? res = await Util.Async.Timer.Wait(
                tcs: signal,
                seconds: dangerSeconds,
                ct: this.GetCancellationTokenOnDestroy(),
                unscaled: true,
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

        #region Update check
        private void FixedUpdate() {

        }

        #endregion

        #region Physic Event
        private void OnTriggerEnter2D(Collider2D other) => _CheckTriggerEnter(other);
        private void OnTriggerExit2D(Collider2D other) => _CheckTriggerExit(other);

        private void _CheckTriggerEnter(Collider2D other) {
            Debug.Log("Trigger Enter");
            if (!other.CompareTag("Fruit")) return;
            var fruit = other.GetComponent<FruitCtrl>();
            if (!overlaps.Contains(fruit)) {
                if (fruit.DoneSpawn && overlaps.Count > 1) {
                    Debug.Log("overlaps.Add");
                    overlaps.Add(fruit);
                    StartDanger();
                }
            }
        }

        private void _CheckTriggerExit(Collider2D other) {
            Debug.Log("Trigger Exit");
            if (!other.CompareTag("Fruit")) return;
            var fruit = other.GetComponentInParent<FruitCtrl>();
            if (overlaps.Contains(fruit)) {
                Debug.Log("overlaps.Remove");
                overlaps.Remove(fruit);
                if (overlaps.Count == 0)
                    CancelDanger();
            }
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
