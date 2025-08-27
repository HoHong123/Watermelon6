using System.Collections.Generic;
using UnityEngine;
using Util.Pooling;

namespace Melon.Game {
    public class GameManager : SingletonBehaviour<GameManager> {
        public FruitCtrl[] fruitPrefabs;
        public float mergeBounceForce = 2f;
        Dictionary<FruitType, ComponentPool<FruitCtrl>> fruitPools;


        private void Start() {
            foreach (var fruit in fruitPrefabs) {
                var pool = new ComponentPool<FruitCtrl>(
                    fruit,
                    0,
                    new GameObject($"{fruit.Type.ToString()}_FruitPool").transform,
                    (obj) => obj.OnCreate(),
                    (obj) => obj.OnGet(),
                    (obj) => obj.OnReturn(),
                    (obj) => obj.OnDispose()
                    );

                fruitPools.Add(fruit.Type, pool);
            }
        }

        public bool CanUpgrade(FruitType level) {
            return true;
            //return level + 1 < fruitPrefabs.Length;
        }

        public void MergeAt(Vector2 position, FruitType level) {
            int nextLevel = Mathf.Min((int)level + 1, fruitPrefabs.Length - 1);
            var go = Instantiate(fruitPrefabs[nextLevel], position, Quaternion.identity);
            var rb = go.GetComponent<Rigidbody2D>();
            if (rb != null) {
                rb.AddForce(Vector2.up * mergeBounceForce, ForceMode2D.Impulse);
                rb.AddTorque(Random.Range(-5f, 5f), ForceMode2D.Impulse);
            }
        }
    }
}
