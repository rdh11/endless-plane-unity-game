using System;
using UnityEngine;

namespace EndlessPlane.Core.UI
{
    public class UIGameOver : MonoBehaviour
    {
        [SerializeField]
        Canvas _canvas = null;

        Action _retryCallback;

        public void ShowGameOverScreen(Action retryCallback)
        {
            _canvas.enabled = true;
            _retryCallback = retryCallback;
        }

        public void OnClick_Retry()
        {
            _canvas.enabled = false;
            _retryCallback?.Invoke();
        }
    }
}
