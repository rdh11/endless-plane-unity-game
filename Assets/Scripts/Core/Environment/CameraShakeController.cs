using DG.Tweening;
using UnityEngine;

namespace EndlessPlane.Core.Environment
{
    public class CameraShakeController : MonoBehaviour
    {
        [SerializeField]
        Camera _mainCamera = null;
        [SerializeField]
        float _cameraShakeDuration = 0.5f;
        [SerializeField]
        Vector3 _cameraShakeStrength = Vector3.zero;
        [SerializeField]
        int _shakeVibrato = 10;
        [SerializeField]
        float _shakeRandomness = 90.0f;

        bool _cameraShakeInProgress = false;
        Vector3 _cameraInitialPos;

        public void ShakeCamera()
        {
            if (_cameraShakeInProgress)
                return;
            _cameraShakeInProgress = true;
            _cameraInitialPos = _mainCamera.transform.position;
            _mainCamera.DOShakePosition(_cameraShakeDuration, _cameraShakeStrength, _shakeVibrato, _shakeRandomness).onComplete = () => {
                _cameraShakeInProgress = false;
                _mainCamera.transform.position = _cameraInitialPos;
            };
        }
    }
}
