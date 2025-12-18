using System;
using UnityEngine;

namespace EndlessPlane.Core.Environment
{
    public class MovableBox : MonoBehaviour
    {
        [SerializeField]
        protected Transform _transformRef = null;
        [SerializeField]
        protected SpriteRenderer _spriteRenderer = null;
        [SerializeField]
        protected float _rangeTillNoColorRendered = 0.2f;

        protected float _interpolationValue = 0.0f;
        protected float _interpolationMultiplier;
        protected Vector3 _initialPos;
        protected Quaternion _initialRot;
        protected Vector3 _targetPos;
        protected Quaternion _targetRot;
        protected BoxMovementHandler _boxMovementHandler;
        protected float _elapsedTime = 0.0f;
        protected Color _spriteColor;
        protected bool _forceStop = false;
        protected Action _targetReachedCallback;
        protected Action<float> _colorAlphaCallback;
        protected Action _assignObstacleCallback;

        public void Init(float interpolationMultiplier, Vector3 targetPos, Quaternion targetRot, BoxMovementHandler boxMovementHandler)
        {
            _Init(interpolationMultiplier, targetPos, targetRot, boxMovementHandler);
        }

        protected virtual void _Init(float interpolationMultiplier, Vector3 targetPos, Quaternion targetRot, BoxMovementHandler boxMovementHandler)
        {
            _SetInterpolationMultiplier(interpolationMultiplier);
            _initialPos = _transformRef.position;
            _initialRot = _transformRef.rotation;
            _targetPos = targetPos;
            _targetRot = targetRot;
            _boxMovementHandler = boxMovementHandler;
            _spriteColor = _spriteRenderer.color;
        }

        void _SetInterpolationMultiplier(float interpolationMultiplier)
        {
            _interpolationMultiplier = interpolationMultiplier;
        }

        public void MoveAndRotate(float deltaTime, bool isCubicLerpEnabled)
        {
            _MoveAndRotate(deltaTime, isCubicLerpEnabled);
        }

        protected virtual void _MoveAndRotate(float deltaTime, bool isCubicLerpEnabled)
        {
            // dont proceed if force stop
            if (_forceStop)
                return;
            // check if box reached target & reposition
            if (IsTargetReached())
            {
                ResetData();
                // assign new spawn position and rotation
                AssignPositionAndRotation();
                // set interpolation multiplier
                _SetInterpolationMultiplier(_boxMovementHandler.GetInterpolationMultiplier(this));
                // notify listner - reached target
                _targetReachedCallback?.Invoke();
                // enable force stop till notify from Box movement handler to begin
                _forceStop = true;
                _boxMovementHandler.NotifyMovementStatus(this, () => {
                    _forceStop = false;
                    _assignObstacleCallback?.Invoke();
                });
                return;
            }
            _interpolationValue += deltaTime;
            _elapsedTime += Time.deltaTime;
            if (isCubicLerpEnabled)
            {
                _transformRef.position = ArcHandler.Instance.CubicLerp(_interpolationValue, _interpolationMultiplier);
            }
            else
            {
                _transformRef.position = Vector3.Lerp(_initialPos, _targetPos, _interpolationValue / _interpolationMultiplier);
            }
            _transformRef.rotation = Quaternion.Lerp(_initialRot, _targetRot, _interpolationValue / _interpolationMultiplier);

            // update sprite color
            UpdateColor();
        }

        protected virtual bool IsTargetReached()
        {
            return _transformRef.position.Equals(_targetPos) &&
                _transformRef.rotation.Equals(_targetRot);
        }

        protected virtual void ResetData()
        {
            _interpolationValue = 0.0f;
            _elapsedTime = 0.0f;
            _spriteColor.a = 0.0f;
            _spriteRenderer.color = _spriteColor;
            _colorAlphaCallback?.Invoke(_spriteColor.a);
        }

        public Vector3 GetPosition()
        {
            return _transformRef.position;
        }

        void AssignPositionAndRotation()
        {
            _initialPos = _transformRef.position = MovableBoxTransformProvider.Instance.GetSpawnPosition();
            _initialRot = _transformRef.rotation = MovableBoxTransformProvider.Instance.GetSpawnRotation();
        }

        void UpdateColor()
        {
            _spriteColor.a = (_interpolationValue / _interpolationMultiplier - _rangeTillNoColorRendered) / (1 - _rangeTillNoColorRendered);
            _spriteRenderer.color = _spriteColor;
            _colorAlphaCallback?.Invoke(_spriteColor.a);
        }

        public void NotifyWhenReachedTarget(Action targetReachedCallback)
        {
            _targetReachedCallback += targetReachedCallback;
        }

        public void NotifyColorAlpha(Action<float> colorAlphaCallback)
        {
            _colorAlphaCallback = colorAlphaCallback;
        }

        public void NotifyAssignObstacle(Action assignObstacleCallback)
        {
            _assignObstacleCallback = assignObstacleCallback;
        }
    }
}
