using System;
using UnityEngine;

namespace EndlessPlane.Core.Environment
{
    public class GameCollisionHandler : MonoBehaviour
    {
        [SerializeField]
        CameraShakeController _cameraShakeController = null;

        public static GameCollisionHandler Instance => s_instance;
        static GameCollisionHandler s_instance;

        Action _playerCollisionCallback;

        void Awake()
        {
            s_instance = this;
        }

        public void HandleEnemyCollision()
        {
            _playerCollisionCallback?.Invoke();
            _cameraShakeController.ShakeCamera();
        }

        public void NotifyPlayerCollision(Action playerCollisionCallback)
        {
            _playerCollisionCallback = playerCollisionCallback;
        }
    }
}
