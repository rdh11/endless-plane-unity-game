using EndlessPlane.Core.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EndlessPlane.Core
{
    public class HUDButtonsManager : MonoBehaviour
    {
        [SerializeField]
        UIPausePanel _uiPausePanel = null;

        public static HUDButtonsManager Instance => s_instance;
        static HUDButtonsManager s_instance;

        void Awake()
        {
            s_instance = this;
        }

        public void HandlePause()
        {
            GameManager.Instance.GameStarted = false;
            _uiPausePanel.ShowUI(isReplay =>
            {
                // Debug.Log("isReplay: " + isReplay);
                if (isReplay)
                {
                    // load current scene
                    SceneManager.LoadScene(0);
                }
                GameManager.Instance.GameStarted = !isReplay;
            });
        }
    }
}
