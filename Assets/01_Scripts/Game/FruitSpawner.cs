using UnityEngine;
using UnityEngine.EventSystems;

namespace Melon.Game {
    public class FruitSpawner : MonoBehaviour {
        public Transform spawnPoint;          // ���� ������ ���� ��ġ(���⼭ x�� ������)
        public Camera cam;                    // ����θ� Camera.main ���
        public BoxCollider2D playArea;        // �¿� �Ѱ踦 ���� ����(�ɼ�). ���ηθ� �̵�
        public float xPadding = 0.3f;         // ������ ����
        public float moveLerp = 20f;          // �ε巴�� ������� ����
        public bool dropOnRelease = true;     // �� �� �� ���(����)

        void Awake() {
            if (cam == null)
                cam = Camera.main;
        }

        void Update() {
            // UI �������� �Է��� ����(�ִٸ�)
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            HandlePointer();   // ���콺/��ġ ���� ó��
        }

        void HandlePointer() {
            // --- ���콺 ---
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

            // --- ��ġ(�����) ---
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

            // x�� �ε巴�� ����
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
                // �÷��� ���� �ݶ��̴��� ���� ��: ī�޶� ���� ������ �뷫 ����
                float halfW = cam.orthographicSize * cam.aspect;
                float cx = cam.transform.position.x;
                minX = cx - halfW + xPadding;
                maxX = cx + halfW - xPadding;
            }

            return Mathf.Clamp(x, minX, maxX);
        }

        void SpawnFruit() {
            var prefab = GameManager.Instance.fruitPrefabs[0]; // Lv0 ����
            Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        }
    }
}