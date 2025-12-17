using UnityEngine;

namespace EndlessPlane.Core.Environment
{
    public class ArcHandler : MonoBehaviour, IUpdateable
    {
        [Header("Cubic Lerp")]
        [SerializeField]
        Transform _startAnchor = null;
        [SerializeField]
        Transform _firstHandle = null;
        [SerializeField]
        Transform _secondHandle = null;
        [SerializeField]
        Transform _endAnchor = null;
        [Header("Start Anchor movement")]
        [SerializeField]
        float _wait = 5.0f;
        [SerializeField]
        float _duration = 4.95f;
        [SerializeField]
        float _anchorPosMin = -2.0f;
        [SerializeField]
        float _anchorPosMax = 2.0f;

        public static ArcHandler Instance => s_instance;
        static ArcHandler s_instance;

        public Vector3 StartAndhorPosition => _startAnchor.position;
        public bool IsCubicLerpEnabled => _isCubicLerpEnabled;

        float _interpolationValue;
        float _interpolationMultiplier;
        float _anchorRotateElapsedTime = 0.0f;
        bool _isCubicLerpEnabled = false;
        float _elapsedTime = 0.0f;
        bool _canMoveStartAnchor = false;
        Vector3 _startAnchorInitial;
        Vector3 _startAnchorTarget;

        void Awake()
        {
            s_instance = this;
        }

        void Start()
        {
            // register for update callback
            GameManager.Instance.RegisterUpdateable(this);
        }

        public Vector3 CubicLerp(float interpolationValue, float interpolationMultiplier)
        {
            _interpolationValue = interpolationValue;
            _interpolationMultiplier = interpolationMultiplier;
            return CubicLerp(_startAnchor.position, _firstHandle.position, _secondHandle.position, _endAnchor.position);
        }

        Vector3 CubicLerp(Vector3 startAnchor, Vector3 firstHandle, Vector3 secondHandle, Vector3 endAnchor)
        {
            Vector3 quadraticLerp1 = QuadraticLerp(startAnchor, firstHandle, secondHandle);
            Vector3 quadraticLerp2 = QuadraticLerp(firstHandle, secondHandle, endAnchor);

            return Vector3.Lerp(quadraticLerp1, quadraticLerp2, _interpolationValue / _interpolationMultiplier);
        }

        Vector3 QuadraticLerp(Vector3 startPos, Vector3 midPos, Vector3 endPos)
        {
            Vector3 posBetween1And2 = Vector3.Lerp(startPos, midPos, _interpolationValue / _interpolationMultiplier);
            Vector3 posBetween2And3 = Vector3.Lerp(midPos, endPos, _interpolationValue / _interpolationMultiplier);

            return Vector3.Lerp(posBetween1And2, posBetween2And3, _interpolationValue / _interpolationMultiplier);
        }

        // void Update()
        void IUpdateable.Update(float deltaTime)
        {
            MoveStartAnchor(deltaTime);
            if (_canMoveStartAnchor)
            {
                if ((_elapsedTime += deltaTime) > _duration)
                {
                    // Debug.LogError("moving anchor done");
                    _canMoveStartAnchor = false;
                    _elapsedTime = 0.0f;
                    _startAnchor.position = _startAnchorTarget;
                    return;
                }
                _startAnchor.position = Vector3.Lerp(_startAnchorInitial, _startAnchorTarget, _elapsedTime / _duration);
            }
        }

        void MoveStartAnchor(float deltaTime)
        {
            if ((_anchorRotateElapsedTime += deltaTime) > _wait)
            {
                // Debug.LogError("start moving anchor");
                _isCubicLerpEnabled = true;
                _canMoveStartAnchor = true;
                _anchorRotateElapsedTime = 0.0f;
                _startAnchorInitial = _startAnchor.position;
                _startAnchorTarget = _startAnchorInitial;
                _startAnchorTarget.x = Random.Range(_anchorPosMin, _anchorPosMax);
                _startAnchorTarget.y = Random.Range(_anchorPosMin, _anchorPosMax);
                // Debug.LogError("_startAnchorTarget "+_startAnchorTarget);
            }
        }
    }
}
