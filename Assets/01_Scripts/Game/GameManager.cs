using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;
using Util.Pooling;

namespace Melon.Game {
    public class GameManager : SingletonBehaviour<GameManager> {
        // ¸â¹ö ¿µ¿ª ¾îµò°¡¿¡ Ãß°¡
        [Title("State")]
        [ShowInInspector, ReadOnly]
        public bool IsGameOver { get; private set; }

        [Title("Prefabs")]
        [SerializeField]
        FruitCtrl[] fruitPrefabs;
        [ShowInInspector]
        readonly HashSet<FruitCtrl> activeFruits = new();
        readonly Dictionary<FruitType, ComponentPool<FruitCtrl>> fruitPools = new();

        [Title("Score")]
        [SerializeField]
        int score = 0;

        [Title("Play Area")]
        [SerializeField]
        BoxCollider2D playBoundary;
        [SerializeField]
        float mergeBounceForce = 2f;
        
        public Bounds PlayBounds => playBoundary.bounds;

        public event Action<int> OnScoreChange;

        public event Action OnGameOver;
        public event Action OnGameOverPPO; // Post process over


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

            fruitPrefabs = null;

            OnGameOver += _OnGameOver;
        }


        public FruitCtrl Get(FruitType type) {
            var fruit = fruitPools[type].Get();
            activeFruits.Add(fruit);
            score += 5;
            OnScoreChange?.Invoke(score);
            return fruit;
        }
        public void Return(FruitCtrl fruit) {
            activeFruits.Remove(fruit);
            fruitPools[fruit.Type].Return(fruit);
        }
        public FruitCtrl GetRandomBasedOnScore() => Get(_SelectRandomFruit());
        public bool CanUpgrade(FruitType level) => (int)level < fruitPools.Count;
        public void MergeAt(Vector2 position, FruitType level) {
            FruitType nextLevel = (FruitType)Mathf.Min((int)level + 1, fruitPools.Count);
            var fruit = Get(nextLevel);
            fruit.transform.position = position;
            fruit.PhysicActive();
            fruit.Bounce(mergeBounceForce);
            _AddScore(nextLevel);
        }

        
        public void GameOver() {
            if (IsGameOver) return;
            IsGameOver = true;
            OnGameOver?.Invoke();
        }


        private async void _OnGameOver() {
            Debug.Log("[GameManager] GAME OVER");

            foreach (var fruit in activeFruits) {
                _AddScore(fruit.Type);
                fruit.PhysicDeactive();
                fruit.gameObject.SetActive(false);
                await UniTask.Delay(400);
            }

            OnGameOverPPO?.Invoke();
        }

        private FruitType _SelectRandomFruit() {
            int max = 3;
            if (score > 1200) max = 6; // Max 5
            else if (score > 800) max = 5; // Max 4
            else if(score > 300) max = 4;
            return (FruitType)Random.Range(1, max);
        }

        private void _AddScore(FruitType type) {
            switch (type) {
            case FruitType.Cherry: score += 10; break;
            case FruitType.Lemon: score += 20; break;
            case FruitType.Peach: score += 30; break;
            case FruitType.Apple: score += 50; break;
            case FruitType.Pear: score += 70; break;
            case FruitType.Orange: score += 80; break;
            case FruitType.Mango: score += 100; break;
            case FruitType.Melon: score += 150; break;
            case FruitType.Pineapple: score += 200; break;
            case FruitType.Watermelon: score += 500; break;
            }

            OnScoreChange?.Invoke(score);
        }
    }
}
