using UnityEngine;

namespace Melon.Game {
    public class GameManager : SingletonBehaviour<GameManager> {
        [Tooltip("���� ���� ������� ��� (0,1,2,...)")]
        public GameObject[] fruitPrefabs;

        [Tooltip("��ü �� ��¦ ���� ƨ��� ��")]
        public float mergeBounceForce = 2f;

        
        public bool CanUpgrade(int level) {
            return level + 1 < fruitPrefabs.Length;
        }

        public void MergeAt(Vector2 position, int level) {
            int nextLevel = Mathf.Min(level + 1, fruitPrefabs.Length - 1);
            var go = Instantiate(fruitPrefabs[nextLevel], position, Quaternion.identity);
            var rb = go.GetComponent<Rigidbody2D>();
            if (rb != null) {
                rb.AddForce(Vector2.up * mergeBounceForce, ForceMode2D.Impulse);
                rb.AddTorque(Random.Range(-5f, 5f), ForceMode2D.Impulse);
            }
        }
    }
}
