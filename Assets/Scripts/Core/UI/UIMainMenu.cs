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
            SoundManager.Instance.PlayButtonSFX();
            MainMenuManager.Instance.HandlePlay();
        }

        public void OnClick_ControlsButton()
        {
            SoundManager.Instance.PlayButtonSFX();
            MainMenuManager.Instance.HandleControls();
        }

        public void OnClick_ControlsCrossButton()
        {
            SoundManager.Instance.PlayButtonSFX();
            _controlsPanel.SetActive(false);
        }
    }
}
