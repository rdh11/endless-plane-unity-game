using UnityEngine;

namespace EndlessPlane.Core
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        Player _player = null;
        [SerializeField]
        float _moveDivider = 100.0f;
        [SerializeField]
        float _positionToAngleConversionRate = -10.0f;
        [SerializeField]
        float _initialRotationDuration = 0.3f;

        public static PlayerController Instance => s_instance;
        static PlayerController s_instance;

        public float MoveDivider => _moveDivider;
        public float PositionToAngleConversionRate => _positionToAngleConversionRate;
        public float InitialRotationDuration => _initialRotationDuration;

        void Awake()
        {
            s_instance = this;
        }

        public void MovePlayer(float horizontalMove, float verticalMove)
        {
            _player.Move(horizontalMove, verticalMove);
        }

        public Vector3 GetPlayerPosition()
        {
            return _player.GetPosition();
        }
    }
}
