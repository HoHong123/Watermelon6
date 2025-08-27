using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Melon.Game {
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class FruitCtrl : MonoBehaviour, IComparer<FruitType>, IPoolable {
        [Title("Data")]
        [SerializeField]
        FruitType type;
        public FruitType Type => type;
        public int Level => (int)type;
        public int Compare(FruitType x, FruitType y) => x.CompareTo(y);

        [Title("Physic")]
        [SerializeField]
        BoxCollider2D box2D;

        public float mergeCooldown = 0.15f;

        float lastMergeTime = -999f;


        public void OnCreate() {
            gameObject.SetActive(false);
        }

        public void OnDispose() {
            Destroy(gameObject);
        }

        public void OnGet() {
            box2D.enabled = false;
            gameObject.SetActive(true);
        }

        public void OnReturn() {
            box2D.enabled = false;
            gameObject.SetActive(false);
        }


        private void OnCollisionEnter2D(Collision2D collision) {
            if (!collision.collider.CompareTag("Fruit")) return;

            FruitCtrl other = collision.collider.GetComponent<FruitCtrl>();
            if (other == null) return;
            if (Compare(type, other.type) < 1) return;

            // ��ٿ�(���� ��� �ֱٿ� ��ü�ߴٸ� ��ŵ)
            if (Time.time - lastMergeTime < mergeCooldown)
                return;
            if (Time.time - other.lastMergeTime < mergeCooldown)
                return;

            // �� �ʸ� ����(InstanceID�� ū ���� owner)
            if (GetInstanceID() < other.GetInstanceID())
                return;

            // ���� �����̸� �� �̻� ���׷��̵� �Ұ� (�׳� ƨ��⸸)
            if (!GameManager.Instance.CanUpgrade(type))
                return;

            // ��ü ��ġ = �� ������ �߰���
            Vector2 mid = (Vector2)transform.position * 0.5f + (Vector2)other.transform.position * 0.5f;

            // ��ü ����
            GameManager.Instance.MergeAt(mid, type);

            // ��ٿ� ��ŷ
            lastMergeTime = Time.time;
            other.lastMergeTime = Time.time;

            // ���� �� ����
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}