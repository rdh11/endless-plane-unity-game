using System;
using System.Collections;
using EndlessPlane.Core.Environment;
using EndlessPlane.Core.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EndlessPlane.Core
{
    public class GameOverHandler : MonoBehaviour
    {
        [SerializeField]
        UIGameOver _uiGameOver = null;

        public static GameOverHandler Instance => s_instance;
        static GameOverHandler s_instance;

        Action _gameOverCallback;
        bool _gameOverTriggered;

        void Awake()
        {
            s_instance = this;
        }

        void Start()
        {
            GameCollisionHandler.Instance.NotifyPlayerCollision(PlayerCollision);
        }

        public void NotifyGameOver(Action gameOverCallback)
        {
            _gameOverCallback = gameOverCallback;
        }

        void PlayerCollision()
        {
            PlayerController.Instance.HidePlayer();
            TriggerGameOverCallback();
        }

        public void ShowGameOverScreen()
        {
            StartCoroutine(Coroutine_StopSoundAfterDelay());
            _uiGameOver.ShowGameOverScreen(() =>
            {
                // load current scene
                SceneManager.LoadScene(0);
            });
        }

        IEnumerator Coroutine_StopSoundAfterDelay()
        {
            yield return new WaitForSeconds(2.0f);
            SoundManager.Instance.StopAudios();
        }

        void TriggerGameOverCallback()
        {
            if (!_gameOverTriggered)
            {
                _gameOverTriggered = true;
                _gameOverCallback?.Invoke();
            }
        }
    }
}
