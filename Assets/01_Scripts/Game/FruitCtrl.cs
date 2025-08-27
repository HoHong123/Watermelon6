using UnityEngine;

namespace Melon.Game {
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class FruitCtrl : MonoBehaviour {
        [Tooltip("�� ������ ���� (0���� ����)")]
        public int level = 0;

        [Tooltip("���� �浹�� ���� �ߺ� ��ü ���� ��ٿ�(��)")]
        public float mergeCooldown = 0.15f;

        float lastMergeTime = -999f;

        void OnCollisionEnter2D(Collision2D collision) {
            if (!collision.collider.CompareTag("Fruit"))
                return;

            FruitCtrl other = collision.collider.GetComponent<FruitCtrl>();
            if (other == null)
                return;

            // ���� ������ ��ü
            if (other.level != level)
                return;

            // ��ٿ�(���� ��� �ֱٿ� ��ü�ߴٸ� ��ŵ)
            if (Time.time - lastMergeTime < mergeCooldown)
                return;
            if (Time.time - other.lastMergeTime < mergeCooldown)
                return;

            // �� �ʸ� ����(InstanceID�� ū ���� owner)
            if (GetInstanceID() < other.GetInstanceID())
                return;

            // ���� �����̸� �� �̻� ���׷��̵� �Ұ� (�׳� ƨ��⸸)
            if (!GameManager.Instance.CanUpgrade(level))
                return;

            // ��ü ��ġ = �� ������ �߰���
            Vector2 mid = (Vector2)transform.position * 0.5f + (Vector2)other.transform.position * 0.5f;

            // ��ü ����
            GameManager.Instance.MergeAt(mid, level);

            // ��ٿ� ��ŷ
            lastMergeTime = Time.time;
            other.lastMergeTime = Time.time;

            // ���� �� ����
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}