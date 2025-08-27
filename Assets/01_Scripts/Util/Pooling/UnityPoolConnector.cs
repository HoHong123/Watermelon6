using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Util.Pooling {
    public class UnityPoolConnector : MonoBehaviour {
        [Title("Entities")]
        [InfoBox("Must use 'IPoolable' prefab", InfoMessageType.Warning)]
        [SerializeField]
        List<UnityPoolEntity> poolEntity = new();


        private void Awake() {
            foreach (var entity in poolEntity) {
                UnityPoolMaster.Register(entity);
            }
        }

        private void OnDestroy() {
            foreach (var entity in poolEntity) {
                UnityPoolMaster.Remove(entity);
            }
        }
    }
}

/* @Jason
 * Created at May. 29. 2025
 * 1. Create for example.
 */