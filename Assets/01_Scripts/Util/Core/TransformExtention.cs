using UnityEngine;

public static class TransformExtension {
    public static void DestroyAllChildren(this Transform parent) {
        if (parent == null) return;
        for (int i = parent.childCount - 1; i >= 0; i--) {
            Object.Destroy(parent.GetChild(i).gameObject);
        }
    }
}
