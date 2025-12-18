using UnityEngine;

namespace EndlessPlane.Core.Environment
{
    public class SmartObstacle : Obstacle, IUpdateable
    {
        [SerializeField]
        float _followDistanceThreshold = 0.0f;
        [SerializeField]
        float _maxMoveDistance = 0.0f;

        bool _canFollow;
        Vector3 _playerPosition;
        Vector3 _currentPosition;
        float _distance;

        protected override void Start()
        {
            base.Start();
            // register for update callback
            GameManager.Instance.RegisterUpdateable(this);
        }

        public override void AssignPosition(Vector2 pos)
        {
            base.AssignPosition(pos);
            _playerPosition = PlayerController.Instance.GetPlayerPosition();
            _currentPosition = _transform.position;
            _currentPosition.x = _playerPosition.x;
            _currentPosition.y = _playerPosition.y;
            _transform.position = _currentPosition;
            StartFollow();
        }

        public override void ResetPosition(Vector3 position)
        {
            StopFollow();
            base.ResetPosition(position);
        }

        void StartFollow()
        {
            _canFollow = true;
        }

        void StopFollow()
        {
            _canFollow = false;
        }

        void IUpdateable.Update(float deltaTime)
        {
            if (_canFollow)
            {
                _playerPosition = PlayerController.Instance.GetPlayerPosition();
                _currentPosition = _transform.position;
                _distance = Vector3.Distance(_playerPosition, _currentPosition);
                if (_distance < _followDistanceThreshold)
                    return;
                _playerPosition.z = _currentPosition.z;
                _transform.position = Vector3.MoveTowards(_currentPosition, _playerPosition, _maxMoveDistance);
            }
        }
    }
}
