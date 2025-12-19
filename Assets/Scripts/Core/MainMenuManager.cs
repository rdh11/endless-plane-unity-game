using EndlessPlane.Core.UI;
using UnityEngine;

namespace EndlessPlane.Core
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField]
        UIMainMenu _uiMainMenu = null;

        public static MainMenuManager Instance => s_instance;
        static MainMenuManager s_instance;

        void Awake()
        {
            s_instance = this;
        }

        public void HandlePlay()
        {
            _uiMainMenu.HideUI();
            GameManager.Instance.StartGame();
        }

        public void HandleControls()
        {
            _uiMainMenu.ShowControlsUI();
        }
    }
}
