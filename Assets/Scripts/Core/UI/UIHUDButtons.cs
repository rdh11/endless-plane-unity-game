using UnityEngine;

namespace EndlessPlane.Core.UI
{
    public class UIHUDButtons : MonoBehaviour
    {
        public void OnClick_PauseButton()
        {
            HUDButtonsManager.Instance.HandlePause();
        }
    }
}
