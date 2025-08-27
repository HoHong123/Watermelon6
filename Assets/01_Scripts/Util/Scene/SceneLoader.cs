using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util.Logger;

namespace Util.Scene {
    public static class SceneLoader {
        public static event Action OnSceneLoaded;
        public static event Action OnSceneUnloaded;

        public static async UniTask LoadSceneAsync(
            string sceneName,
            LoadSceneMode mode = LoadSceneMode.Single,
            Action<float> onProgress = null,
            Action onComplete = null,
            string loadingScene = null) {
            if (!string.IsNullOrEmpty(loadingScene) && mode == LoadSceneMode.Single) {
                await SceneManager.LoadSceneAsync(loadingScene);
            }

            var asyncOp = SceneManager.LoadSceneAsync(sceneName, mode);
            asyncOp.allowSceneActivation = false;

            while (asyncOp.progress < 0.9f) {
                onProgress?.Invoke(asyncOp.progress);
                await UniTask.Yield();
            }

            onProgress?.Invoke(1f);
            asyncOp.allowSceneActivation = true;

            await asyncOp.ToUniTask();
            onComplete?.Invoke();
            OnSceneLoaded?.Invoke();
        }

        public static async UniTask UnloadSceneAsync(
            string sceneName,
            Action<float> onProgress = null,
            Action onComplete = null) {
            if (!SceneManager.GetSceneByName(sceneName).isLoaded) {
                HLogger.Warning($"Scene '{sceneName}' is not loaded.");
                return;
            }

            var unloadOp = SceneManager.UnloadSceneAsync(sceneName);

            while (!unloadOp.isDone) {
                onProgress?.Invoke(unloadOp.progress);
                await UniTask.Yield();
            }

            onComplete?.Invoke();
            OnSceneUnloaded?.Invoke();
        }
    }
}
