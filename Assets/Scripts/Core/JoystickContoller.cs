using UnityEngine;

namespace EndlessPlane.Core
{
    public class JoystickContoller : MonoBehaviour, IUpdateable
    {
        [SerializeField]
        Joystick _joystick = null;
        [SerializeField]
        PlayerController _playerController = null;

        void Start()
        {
            // register for update callback
            GameManager.Instance.RegisterUpdateable(this);
        }

        // void Update()
        void IUpdateable.Update(float deltaTime)
        {
            // Debug.Log(""+_joystick.Horizontal+" "+_joystick.Vertical+" "+_joystick.Direction);
            _playerController.MovePlayer(_joystick.Horizontal, _joystick.Vertical);
        }
    }
}
