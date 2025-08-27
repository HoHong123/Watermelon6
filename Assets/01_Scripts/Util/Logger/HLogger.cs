using System;
using System.Collections.Generic;
using UnityEngine;


namespace Util.Logger {
    public class HLogger : MonoBehaviour {
        const int MAX_QUE_SIZE = 1000;

        static Queue<string> logQue = new Queue<string>();
        static Queue<string> warningQue = new Queue<string>();
        static Queue<string> errorQue = new Queue<string>();
        static Queue<string> fatalQue = new Queue<string>();

        static string utcNow => DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss zzz");


        public static void Log(string message, GameObject target = null, bool popupActivate = false) {
            string log = $"@1 [{utcNow}] {message}";

#if UNITY_EDITOR
            if (target == null) {
                Debug.Log(log);
            }
            else {
                Debug.Log(log, target);
            }
#endif
            if (popupActivate) {
                //PopupManager.Instance.AddAlert("Log", message);
            }

            logQue.Enqueue(log);
            if (logQue.Count > MAX_QUE_SIZE)
                logQue.Dequeue();
        }

        public static void Warning(string message, GameObject target = null, bool popupActivate = false) {
            string log = $"@2 [{utcNow}] {message}";

#if UNITY_EDITOR
            if (target == null) {
                Debug.LogWarning(log);
            }
            else {
                Debug.LogWarning(log, target);
            }
#endif
            if (popupActivate) {
                //PopupManager.Instance.AddAlert("Warning", message);
            }

            warningQue.Enqueue(message);
            if (warningQue.Count > MAX_QUE_SIZE)
                warningQue.Dequeue();
        }

        public static void Error(string message, GameObject target = null, bool showPopup = false, string debug = "") {
            string log =
                $"@3 [{utcNow}] {message}\n" +
                $"@3 Debug :: {debug}";

#if UNITY_EDITOR
            if (target == null) {
                Debug.LogError(log);
            }
            else {
                Debug.LogError(log, target);
            }
#endif

            if (showPopup) {
                //PopupManager.Instance.AddAlert("Error", message);
            }

            errorQue.Enqueue(message);
            if (errorQue.Count > MAX_QUE_SIZE)
                errorQue.Dequeue();

            // TODO :: Decide what to do with log stack
            // ...
        }

        public static void Exception(Exception ex, string extra = "") {
            string extraInfo = string.IsNullOrEmpty(extra) ? "" : $"{extra} ";
            string log =
                $"@4 [{utcNow}] {ex.Message}\n" +
                $"{extraInfo}";

#if UNITY_EDITOR
            Debug.LogError(log);
#endif

            fatalQue.Enqueue(log);
        }

        public static void Throw(Exception ex, string extra = "") {
            Exception(ex, extra);
            throw ex;
        }
    }
}
