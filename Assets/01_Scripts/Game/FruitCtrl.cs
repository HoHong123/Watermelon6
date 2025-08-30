using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Melon.Game {
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class FruitCtrl : MonoBehaviour, IComparer<FruitType>, IPoolable {
        [Title("Data")]
        [SerializeField]
        FruitType type;

        [Title("Physic")]
        [SerializeField]
        Rigidbody2D r2d;
        [SerializeField]
        CircleCollider2D circle2D;

        float mergeCooldown = 0.15f;
        float lastMergeTime = -999f;

        public FruitType Type => type;
        public int Level => (int)type;
        public int Compare(FruitType x, FruitType y) => x.CompareTo(y);
        public bool DoneSpawn { get; private set; }
        public bool IsMergeCooldownReady => (Time.time - lastMergeTime < mergeCooldown);

        public event Action OnDoneSpawn;


        #region Pool
        public void OnCreate() {
            gameObject.SetActive(false);
        }

        public void OnDispose() {
            Destroy(gameObject);
        }

        public void OnGet() {
            PhysicDeactive();
            gameObject.SetActive(true);
        }

        public void OnReturn() {
            PhysicDeactive();
            gameObject.SetActive(false);
        }
        #endregion

        #region Activation
        public void PhysicActive() {
            r2d.simulated = true;
            circle2D.enabled = true;
        }

        public void PhysicDeactive() {
            r2d.simulated = false;
            circle2D.enabled = false;
        }
        #endregion

        public void Bounce(float bounceForce) {
            r2d.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
            r2d.AddTorque(UnityEngine.Random.Range(-0.3f, 0.3f), ForceMode2D.Impulse);
        }


        private void OnCollisionEnter2D(Collision2D collision) {
            DoneSpawn = true;

            if (!collision.collider.CompareTag("Fruit")) return;

            FruitCtrl other = collision.collider.GetComponent<FruitCtrl>();

            if (other == null) return; // Check object
            if (Compare(type, other.type) != 0) return; // Check level
            if (IsMergeCooldownReady || other.IsMergeCooldownReady) return; // Check cooldown
            if (GetInstanceID() < other.GetInstanceID()) return; // Activate one of fruit controller
            if (!GameManager.Instance.CanUpgrade(type)) return; // Check level
            
            // Find mid point
            Vector2 mid = (Vector2)transform.position * 0.5f + (Vector2)other.transform.position * 0.5f;
            // Record cooldown
            lastMergeTime = Time.time;
            other.lastMergeTime = Time.time;

            // Merge
            GameManager.Instance.MergeAt(mid, type);

            // Return to pool
            GameManager.Instance.Return(other);
            GameManager.Instance.Return(this);
        }
    }
}