using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

namespace Util.UI.Popup {
    public class AlertPopup : BasePopupUi, IPoolReturn, IPoolDispose {
        [Title("Texts")]
        [SerializeField]
        TMP_Text titleTxt;
        [SerializeField]
        TMP_Text descriptionTxt;


        public void SetUi(string title, string message) {
            titleTxt.text = title;
            descriptionTxt.text = message;
        }


        public void OnReturn() {
            titleTxt.text = string.Empty;
            descriptionTxt.text = string.Empty;
        }

        public void OnDispose() {
            Destroy(panel);
        }
    }
}