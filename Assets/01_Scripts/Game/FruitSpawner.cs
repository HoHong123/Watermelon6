using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

namespace Melon.Game {
    public class FruitSpawner : MonoBehaviour {
        [Title("UI")]
        [SerializeField]
        List<GameObject> spawnIcons;

        [Title("Boundary")]
        [SerializeField]
        Transform spawnPoint;
        [SerializeField]
        BoxCollider2D spawnBoundary;
        [SerializeField]
        bool dropOnRelease = true;     // 손 뗄 때 드랍(권장)

        [Title("Delay")]
        [SerializeField, PropertyRange(0, 2f)]
        float spawnDelay = 1.6f;
        [SerializeField]
        bool spawnStop = false;

        [Title("Movement")]
        [SerializeField]
        float xPadding = 0.3f;
        [SerializeField]
        float moveLerp = 20f;

        Camera cam;
        FruitCtrl currentFruit;


        private void Start() {
            cam = Camera.main;
            GameManager.Instance.OnGameOver += _OnGameOver;
        }

        private void Update() {
            // Ignore UI interaction
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
            _HandlePointer();
        }


        private void _HandlePointer() {
#if UNITY_EDITOR
            // Mouse
            if (Input.GetMouseButton(0)) {
                _FollowPointerX(Input.mousePosition);
            }

            if (spawnStop) return;

            if (Input.GetMouseButtonDown(0) && currentFruit == null) {
                _SpawnFruit();
            }

            if (Input.GetMouseButtonUp(0)) {
                _ActiveFruit();
            }
#elif UNITY_ANDROID || UNITY_IOS
            // Touch
            if (Input.touchCount > 0) {
                var t = Input.GetTouch(0);
                _FollowPointerX(t.position);
                
                if (spawnStop) return;

                if (t.phase == TouchPhase.Ended)
                    _ActiveFruit();
            }
#endif
        }


        private void _OnGameOver() {
            enabled = false;
        }

        private void _FollowPointerX(Vector3 screenPos) {
            Vector3 world = cam.ScreenToWorldPoint(screenPos);
            float targetX = _ClampX(world.x);

            // x 보간
            Vector3 spawn = spawnPoint.position;
            spawn.x = Mathf.Lerp(spawn.x, targetX, Time.deltaTime * moveLerp);
            spawnPoint.position = spawn;
            currentFruit.transform.position = spawn;
        }

        private float _ClampX(float x) {
            float minX, maxX;

            if (spawnBoundary != null) {
                Bounds boundary = spawnBoundary.bounds;
                minX = boundary.min.x + xPadding;
                maxX = boundary.max.x - xPadding;
            }
            else {
                float halfWidth = cam.orthographicSize * cam.aspect;
                float camX = cam.transform.position.x;
                minX = camX - halfWidth + xPadding;
                maxX = camX + halfWidth - xPadding;
            }

            return Mathf.Clamp(x, minX, maxX);
        }

        private void _SpawnFruit() {
            //currentFruit = GameManager.Instance.GetRandomBasedOnScore();
            currentFruit = GameManager.Instance.Get(FruitType.Pineapple);
            currentFruit.transform.position = spawnPoint.position;
            currentFruit.transform.eulerAngles = Vector3.zero;
        }

        private void _ActiveFruit() {
            _SpawnDelay();
            currentFruit.PhysicActive();
            currentFruit = null;
        }

        private async void _SpawnDelay() {
            spawnStop = true;
            await UniTask.Delay(TimeSpan.FromSeconds(spawnDelay));
            spawnStop = false;
        }
    }
}