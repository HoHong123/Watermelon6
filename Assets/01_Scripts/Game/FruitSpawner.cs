using UnityEngine;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using Unity.VisualScripting;

namespace Melon.Game {
    public class FruitSpawner : MonoBehaviour {
        [Title("Spawn")]
        [SerializeField]
        Transform spawnPoint;
        [SerializeField]
        BoxCollider2D spawnBoundary;
        [SerializeField, PropertyRange(1f, 2f)]
        float spawnDelay = 1.6f;
        [SerializeField]
        bool dropOnRelease = true;     // 손 뗄 때 드랍(권장)

        [Title("Camera")]
        [SerializeField]
        Camera cam = Camera.main;

        [Title("Movement")]
        [SerializeField]
        float xPadding = 0.3f;
        [SerializeField]
        float moveLerp = 20f;


        private void Update() {
            // Ignore UI interaction
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            _HandlePointer();
        }


        private void _HandlePointer() {
#if UNITY_ANDROID || UNITY_IOS
            // Touch
            if (Input.touchCount > 0) {
                var t = Input.GetTouch(0);
                _FollowPointerX(t.position);

                if (dropOnRelease && t.phase == TouchPhase.Ended)
                    SpawnFruit();
                else if (!dropOnRelease && t.phase == TouchPhase.Began)
                    SpawnFruit();
            }
#else
            // Mouse
            if (Input.GetMouseButton(0)) {
                _FollowPointerX(Input.mousePosition);
            }
            if (dropOnRelease) {
                if (Input.GetMouseButtonUp(0))
                    _SpawnFruit();
            }
            else {
                if (Input.GetMouseButtonDown(0))
                    _SpawnFruit();
            }
#endif
        }

        private void _FollowPointerX(Vector3 screenPos) {
            Vector3 world = cam.ScreenToWorldPoint(screenPos);
            float targetX = _ClampX(world.x);

            // x 보간
            Vector3 spawn = spawnPoint.position;
            spawn.x = Mathf.Lerp(spawn.x, targetX, Time.deltaTime * moveLerp);
            spawnPoint.position = spawn;
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
            var prefab = GameManager.Instance.fruitPrefabs[0];
            Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        }
    }
}