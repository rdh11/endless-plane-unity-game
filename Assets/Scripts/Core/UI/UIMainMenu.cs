using UnityEngine;

namespace EndlessPlane.Core.UI
{
    public class UIMainMenu : MonoBehaviour
    {
        [SerializeField]
        Canvas _canvas = null;
        [SerializeField]
        GameObject _controlsPanel = null;

        public void HideUI()
        {
            _canvas.enabled = false;
        }

        public void ShowControlsUI()
        {
            _controlsPanel.SetActive(true);
        }

        public void OnClick_PlayButton()
        {
            MainMenuManager.Instance.HandlePlay();
        }

        public void OnClick_ControlsButton()
        {
            MainMenuManager.Instance.HandleControls();
        }

        public void OnClick_ControlsCrossButton()
        {
            _controlsPanel.SetActive(false);
        }
    }
}
