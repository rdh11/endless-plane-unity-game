using System;
using UnityEngine;

namespace EndlessPlane.Core.UI
{
    public class UIPausePanel : MonoBehaviour
    {
        [SerializeField]
        Canvas _canvas = null;

        Action<bool> _uiPausePanelActionCallback;

        public void ShowUI(Action<bool> callback = null)
        {
            _uiPausePanelActionCallback = callback;
            _canvas.enabled = true;
        }

        void HideUI(bool isReplay)
        {
            _canvas.enabled = false;
            _uiPausePanelActionCallback?.Invoke(isReplay);
        }

        public void OnClick_ReplayButton()
        {
            HideUI(true);
        }

        public void OnClick_ResumeButton()
        {
            SoundManager.Instance.PlayButtonSFX();
            HideUI(false);
        }
    }
}
