using UnityEngine;
using UnityEngine.EventSystems;

namespace Melon.Game {
    public class FruitSpawner : MonoBehaviour {
        public Transform spawnPoint;          // 위에 고정된 스폰 위치(여기서 x만 움직임)
        public Camera cam;                    // 비워두면 Camera.main 사용
        public BoxCollider2D playArea;        // 좌우 한계를 정할 영역(옵션). 내부로만 이동
        public float xPadding = 0.3f;         // 벽과의 여유
        public float moveLerp = 20f;          // 부드럽게 따라오는 정도
        public bool dropOnRelease = true;     // 손 뗄 때 드랍(권장)

        void Awake() {
            if (cam == null)
                cam = Camera.main;
        }

        void Update() {
            // UI 위에서의 입력은 무시(있다면)
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            HandlePointer();   // 마우스/터치 공통 처리
        }

        void HandlePointer() {
            // --- 마우스 ---
            if (Input.GetMouseButton(0)) {
                FollowPointerX(Input.mousePosition);
            }
            if (dropOnRelease) {
                if (Input.GetMouseButtonUp(0))
                    SpawnFruit();
            }
            else {
                if (Input.GetMouseButtonDown(0))
                    SpawnFruit();
            }

            // --- 터치(모바일) ---
            if (Input.touchCount > 0) {
                var t = Input.GetTouch(0);
                FollowPointerX(t.position);

                if (dropOnRelease && t.phase == TouchPhase.Ended)
                    SpawnFruit();
                else if (!dropOnRelease && t.phase == TouchPhase.Began)
                    SpawnFruit();
            }
        }

        void FollowPointerX(Vector3 screenPos) {
            Vector3 world = cam.ScreenToWorldPoint(screenPos);
            float targetX = ClampX(world.x);

            // x만 부드럽게 보간
            Vector3 p = spawnPoint.position;
            p.x = Mathf.Lerp(p.x, targetX, Time.deltaTime * moveLerp);
            spawnPoint.position = p;
        }

        float ClampX(float x) {
            float minX, maxX;

            if (playArea != null) {
                Bounds b = playArea.bounds;
                minX = b.min.x + xPadding;
                maxX = b.max.x - xPadding;
            }
            else {
                // 플레이 영역 콜라이더가 없을 때: 카메라 가시 범위로 대략 제한
                float halfW = cam.orthographicSize * cam.aspect;
                float cx = cam.transform.position.x;
                minX = cx - halfW + xPadding;
                maxX = cx + halfW - xPadding;
            }

            return Mathf.Clamp(x, minX, maxX);
        }

        void SpawnFruit() {
            var prefab = GameManager.Instance.fruitPrefabs[0]; // Lv0 과일
            Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        }
    }
}