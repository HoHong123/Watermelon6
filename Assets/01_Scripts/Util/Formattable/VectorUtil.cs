using UnityEngine;


namespace Util.Formattable {
    public static class VectorUtil {
        public static Vector2 GetRandomPositionWithin(RectTransform rectTransform) {
            Vector2 size = rectTransform.rect.size;

            float randomX = Random.Range(0, size.x) - size.x / 2;
            float randomY = Random.Range(0, size.y) - size.y / 2;

            return new Vector2(randomX, randomY);
        }

        public static Vector2 GetCanvasPosition(Transform _target, Camera _camera) {
            return _camera.WorldToScreenPoint(_target.position);
        }
    }
}