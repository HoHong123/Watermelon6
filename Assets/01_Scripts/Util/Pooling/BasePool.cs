using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Util.Logger;

namespace Util.Pooling {
    public abstract class BasePool<T> : IDisposable where T : class {
        [ShowInInspector, ReadOnly]
        protected readonly Stack<T> pool = new();
        [ShowInInspector, ReadOnly]
        protected readonly HashSet<T> activatedPool = new();

        protected Action<T> onCreate = null;
        protected Action<T> onGet = null;
        protected Action<T> onReturn = null;
        protected Action<T> onDispose = null;

        public int CountTotal => pool.Count + activatedPool.Count;
        public int CountAvaliable => pool.Count;
        public int CountActivated => activatedPool.Count;

#if UNITY_EDITOR
        public string CheckPoolData() {
            if (pool.Count + activatedPool.Count == 0) {
                return $"[{typeof(T)} Pool] No pool object found.";
            }
            else {
                return $"[{typeof(T)} Pool] Active objects :: \n" +
                    $"Totall :: {CountTotal}\n" +
                    $"Waiting :: {CountAvaliable}\n" +
                    $"Activated :: {CountActivated}\n";
            }
        }
#endif


        protected BasePool(
            Action<T> onCreate = null, Action<T> onGet = null,
            Action<T> onReturn = null, Action<T> onDispose = null) {
            this.onCreate = onCreate;
            this.onGet = onGet;
            this.onReturn = onReturn;
            this.onDispose = onDispose;
        }


        public virtual void Init(int capacity) {
            Dispose();
            for (int k = 0; k < capacity; k++) {
                pool.Push(Create());
            }
        }


        public virtual T Get() {
            if (pool.Count == 0) {
                pool.Push(Create());
            }

            var obj = pool.Pop();
            onGet?.Invoke(obj);
            activatedPool.Add(obj);
            return obj;
        }

        public virtual void Return(T obj) {
            if (!activatedPool.Contains(obj)) {
                HLogger.Warning("[Pool] None pool object try return.");
                return;
            }

            onReturn?.Invoke(obj);
            pool.Push(obj);
            activatedPool.Remove(obj);
        }


        public virtual void Dispose() {
            foreach (var obj in pool) {
                onDispose?.Invoke(obj);
            }
            foreach (var obj in activatedPool) {
                HLogger.Warning($"[Pool] Object of type {typeof(T).Name} was not returned before Dispose.");
                onDispose?.Invoke(obj);
            }
            pool.Clear();
            activatedPool.Clear();
        }


        protected abstract T Create();
    }
}


/* Dev Log
 * @Jason - PKH
 * 풀링 시스템의 유연성과 일관성을 생각하여 작성한 클래스입니다.
 * 원하는 어떠한 타입이든 모두 적용 가능한 것이 첫번째 목표였습니다.
 * 그리고 런타임 전, 미리 씬 혹은 백그라운드에 설정할 수 있는 기능을 위하여
 * 'UnityPoolEntity' 객체를 통해 원하는 값을 미리 정의하고 'PoolManager'가 시작함과 동시에
 * 해당 값을 토대로 풀링 객체들을 생성하여 키값으로 가져오는 것을 두번째 목표로 하였습니다.
 * ------------------------------------
 * 이를 베이스로 다음 3개의 파생 클래스가 작성되었습니다.
 * 1. 게임오브젝트(GameObjectPool)
 * + 초기 클래스로 수정 혹은 제거될 수 있습니다.
 * 2. 일반 C# 클래스(ClassPool)
 * 3. 풀링이 가능한('IPoolable'을 상속받은 'PoolableMono'클래스) 오브젝트 = 유니티 GUI 친화적 환경을 위한 클래스
 * + 일반 MonoBehaviour 타입을 풀링 타겟으로 설정시, 실제 풀링에 사용될 컴포넌트 추출이 어려워 생성되었습니다.
 * 
 * @Jason - PKH
 * 1. 최근 반환된 오브젝트가 메모리 캐시에 더 가깝게 존재하는 경향이 있어서 큐에서 스택으로 변환했습니다.
 * 2. HashSet으로 Return 과정의 중복성 검사를 진행합니다.
 */