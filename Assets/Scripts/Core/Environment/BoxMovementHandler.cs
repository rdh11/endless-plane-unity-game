using System;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessPlane.Core.Environment
{
    public class BoxMovementHandler : MonoBehaviour, IUpdateable
    {
        [SerializeField]
        Transform _movableBoxTarget = null;
        [SerializeField]
        MovableBox[] _movableBoxes = null;
        [SerializeField]
        float _speedMultiplier = 1.0f;
        [SerializeField]
        float _time = 240.0f;
        [SerializeField]
        float _maxSpeed = 5.0f;
        [SerializeField]
        float _minSpeed = 1.0f;
        [SerializeField]
        float _waitingBoxZThreshold = 1.5f;

        bool _moveBoxes = false;
        int _totalMovableBoxes;
        float _nearestMovableBoxDistance;
        float _elapsedTime = 0.0f;
        MovableBox _farthestBox;
        MovableBox _waitingBox;
        Queue<MovableBox> _waitingBoxes;
        Action _movementStatusCallback;
        Queue<Action> _movementStatusCallbacks;
        bool _isNotifyMovementStatus = false;

        void Start()
        {
            Init();
        }

        void Init()
        {
            if (_movableBoxes == null)
            {
                Debug.LogError("Movable boxes not initialized.");
                return;
            }
            if (_movableBoxes.Length <= 0)
            {
                Debug.LogError("No movable boxes.");
                return;
            }
            // register for update callback
            GameManager.Instance.RegisterUpdateable(this);

            _totalMovableBoxes = _movableBoxes.Length;
            _moveBoxes = true;
            Vector3 targetPos = _movableBoxTarget.position;
            Quaternion targetRot = _movableBoxTarget.rotation;
            
            // get neares movable box distance
            _nearestMovableBoxDistance = GetNearestMovableBoxDistance();

            // initialize movable boxes
            for (int i = 0; i < _totalMovableBoxes; i++)
            {
                _movableBoxes[i].Init(Vector3.Distance(_movableBoxes[i].GetPosition(), targetPos) / _nearestMovableBoxDistance,
                                    targetPos,
                                    targetRot,
                                    this);
            }
        }

        float GetNearestMovableBoxDistance()
        {
            // calculate min distance for 1x interpolation multiplier
            float minDistance = float.MaxValue;
            float maxDistance = float.MinValue;
            float currentDistance;
            Vector3 targetPos = _movableBoxTarget.position;
            for (int i = 0; i < _totalMovableBoxes; i++)
            {
                currentDistance = Vector3.Distance(_movableBoxes[i].GetPosition(), targetPos);
                if (currentDistance < minDistance)
                    minDistance = currentDistance;
                if (currentDistance >= maxDistance)
                {
                    maxDistance = currentDistance;
                    _farthestBox = _movableBoxes[i];
                }
            }
            return minDistance;
        }

        public float GetInterpolationMultiplier(MovableBox movableBox)
        {
            return Vector3.Distance(movableBox.GetPosition(), _movableBoxTarget.position) / _nearestMovableBoxDistance;
        }

        public void NotifyMovementStatus(MovableBox movableBox, Action moveCallback)
        {
            _isNotifyMovementStatus = true;
            if (_waitingBox != null && _waitingBox.GetPosition().z >= 36f)  // this is hard coded - last movables z-value
            {
                if (_waitingBoxes == null)
                    _waitingBoxes = new Queue<MovableBox>();
                _waitingBoxes.Enqueue(movableBox);
                if (_movementStatusCallbacks == null)
                    _movementStatusCallbacks = new Queue<Action>();
                _movementStatusCallbacks.Enqueue(moveCallback);
                // Debug.Log("total waiting boxes: "+_waitingBoxes.Count);
            }
            else
            {
                if (_waitingBoxes != null && _waitingBoxes.Count > 0)
                {
                    _waitingBoxes.Enqueue(movableBox);
                    _movementStatusCallbacks.Enqueue(moveCallback);
                    _waitingBox = _waitingBoxes.Dequeue();
                    _movementStatusCallback = _movementStatusCallbacks.Dequeue();
                    // Debug.Log("total waiting boxes after dequeue: "+_waitingBoxes.Count);
                }
                else
                {
                    // Debug.Log("direct assign to waiting box");
                    _waitingBox = movableBox;
                    _movementStatusCallback = moveCallback;
                }
            }
        }

        // void Update()
        void IUpdateable.Update(float deltaTime)
        {
            if (_moveBoxes)
            {
                // float deltaTime = Time.deltaTime;
                // increase speed gradually
                IncreaseSpeed(deltaTime);

                for (int i = 0; i < _totalMovableBoxes; i++)
                {
                    _movableBoxes[i].MoveAndRotate(deltaTime * _speedMultiplier, ArcHandler.Instance.IsCubicLerpEnabled);
                }
            }
            if (_isNotifyMovementStatus)
            {
                // Debug.Log("waitingBox: "+_waitingBox.name+" "+_waitingBox.GetPosition().z+" _farthestBox: "+_farthestBox.name+" "+_farthestBox.GetPosition().z);
                if (_waitingBox.GetPosition().z - _farthestBox.GetPosition().z >= _waitingBoxZThreshold)
                {
                    _farthestBox = _waitingBox;
                    _movementStatusCallback?.Invoke();
                    if (_waitingBoxes != null && _waitingBoxes.Count > 0)
                    {
                        _waitingBox = _waitingBoxes.Dequeue();
                        _movementStatusCallback = _movementStatusCallbacks.Dequeue();
                    }
                    else
                    {
                        _isNotifyMovementStatus = false;
                    }
                }
            }
        }

        void IncreaseSpeed(float deltaTime)
        {
            if ((_elapsedTime += deltaTime) > _time)
                return;
            _speedMultiplier = _minSpeed + (_elapsedTime / _time * (_maxSpeed - _minSpeed));
        }
    }
}
