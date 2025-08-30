using UnityEngine;
using Sirenix.OdinInspector;
using Melon.Game;
using TMPro;

namespace Melon.UI {
    public class UiManager : SingletonBehaviour<UiManager> {
        [Title("Top UI")]
        [SerializeField]
        TMP_Text scoreTxt;

        private void Start() {
            GameManager game = GameManager.Instance;
            game.OnScoreChange += (score) => scoreTxt.text = score.ToString();
        }
    }
}
