using System.Collections;
using UnityEngine;

namespace EndlessPlane.Core
{
    public class Player : MonoBehaviour
    {
        [SerializeField]
        Transform _transform = null;
        [SerializeField]
        float _maxHorizontalMove = 0.0f;
        [SerializeField]
        float _minHorizontalMove = 0.0f;
        [SerializeField]
        float _maxVerticalMove = 0.0f;
        [SerializeField]
        float _minVerticalMove = 0.0f;

        Quaternion _initialRotation;
        Quaternion _cachedRotation;
        bool _lerpInProgress = false;
        bool _canInitialize = false;
        float _elapsedTime = 0.0f;

        void Start()
        {
            _initialRotation = _transform.rotation;
        }

        public void Move(float horizontalMove, float verticalMove)
        {
            if (horizontalMove == 0.0f && verticalMove == 0.0f)
            {
                if (_canInitialize)
                {
                    _canInitialize = false;
                    if (!_lerpInProgress)
                    {
                        _cachedRotation = _transform.rotation;
                        StartCoroutine(Coroutine_ApplyInitialRotation(_initialRotation));
                    }
                }
                return;
            }
            _canInitialize = true;
            Vector3 currentPosition = _transform.position;
            currentPosition.x += horizontalMove / PlayerController.Instance.MoveDivider;
            currentPosition.y += verticalMove / PlayerController.Instance.MoveDivider;
            if (currentPosition.x < _maxHorizontalMove && currentPosition.x > _minHorizontalMove)
            {
                if (currentPosition.y < _maxVerticalMove && currentPosition.y > _minVerticalMove)
                {
                    _transform.position = currentPosition;
                    Quaternion rotation = _transform.rotation;
                    Vector3 spawnEulerAngles = rotation.eulerAngles;
                    spawnEulerAngles.z = horizontalMove * PlayerController.Instance.PositionToAngleConversionRate;
                    rotation.eulerAngles = spawnEulerAngles;
                    _transform.rotation = rotation;
                }
                else
                {
                    currentPosition.y = _transform.position.y;
                    _transform.position = currentPosition;
                }
            }
            else
            {
                if (currentPosition.y < _maxVerticalMove && currentPosition.y > _minVerticalMove)
                {
                    currentPosition.x = _transform.position.x;
                    _transform.position = currentPosition;
                    Quaternion rotation = _transform.rotation;
                    Vector3 spawnEulerAngles = rotation.eulerAngles;
                    spawnEulerAngles.z = horizontalMove * PlayerController.Instance.PositionToAngleConversionRate;
                    rotation.eulerAngles = spawnEulerAngles;
                    _transform.rotation = rotation;
                }
            }
        }

        IEnumerator Coroutine_ApplyInitialRotation(Quaternion rotation)
        {
            _lerpInProgress = true;
            while (_elapsedTime <= PlayerController.Instance.InitialRotationDuration)
            {
                // Debug.LogError(""+_elapsedTime);
                _elapsedTime += Time.deltaTime;
                _transform.rotation = Quaternion.Lerp(_cachedRotation, rotation, _elapsedTime / PlayerController.Instance.InitialRotationDuration);
                yield return null;
            }
            _lerpInProgress = false;
            _elapsedTime = 0.0f;
            _transform.rotation = rotation;
        }

        public Vector3 GetPosition()
        {
            return _transform.position;
        }
    }
}
