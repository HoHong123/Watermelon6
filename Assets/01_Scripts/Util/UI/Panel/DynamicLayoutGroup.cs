using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Util.UI.Panel {
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("Layout/Proportional Vertical Layout Group")]
    public class DynamicLayoutGroup : LayoutGroup {
        public override void CalculateLayoutInputVertical() {
            throw new NotImplementedException();
        }

        public override void SetLayoutHorizontal() {
            throw new NotImplementedException();
        }

        public override void SetLayoutVertical() {
            throw new NotImplementedException();
        }
    }
}