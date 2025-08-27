using UnityEngine;

namespace Melon.Game {
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class FruitCtrl : MonoBehaviour {
        [Tooltip("이 과일의 레벨 (0부터 시작)")]
        public int level = 0;

        [Tooltip("연속 충돌로 인한 중복 합체 방지 쿨다운(초)")]
        public float mergeCooldown = 0.15f;

        float lastMergeTime = -999f;

        void OnCollisionEnter2D(Collision2D collision) {
            if (!collision.collider.CompareTag("Fruit"))
                return;

            FruitCtrl other = collision.collider.GetComponent<FruitCtrl>();
            if (other == null)
                return;

            // 같은 레벨만 합체
            if (other.level != level)
                return;

            // 쿨다운(양쪽 모두 최근에 합체했다면 스킵)
            if (Time.time - lastMergeTime < mergeCooldown)
                return;
            if (Time.time - other.lastMergeTime < mergeCooldown)
                return;

            // 한 쪽만 진행(InstanceID가 큰 쪽이 owner)
            if (GetInstanceID() < other.GetInstanceID())
                return;

            // 최종 레벨이면 더 이상 업그레이드 불가 (그냥 튕기기만)
            if (!GameManager.Instance.CanUpgrade(level))
                return;

            // 합체 위치 = 두 과일의 중간점
            Vector2 mid = (Vector2)transform.position * 0.5f + (Vector2)other.transform.position * 0.5f;

            // 합체 실행
            GameManager.Instance.MergeAt(mid, level);

            // 쿨다운 마킹
            lastMergeTime = Time.time;
            other.lastMergeTime = Time.time;

            // 기존 둘 삭제
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}