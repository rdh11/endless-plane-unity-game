using UnityEngine;
using UnityEngine.UI;

namespace EndlessPlane.Core
{
    public class AspectFOVHandler : MonoBehaviour
    {
        [SerializeField]
        Camera _camera = null;
        [SerializeField]
        float _baseHorizontalFov = 35.98339f;

        void Awake()
        {
            // calculate vertical fov with constant horizontal fov value & assign to camera fov
            float aspect = _camera.aspect;
            float calculatedVFov = Camera.HorizontalToVerticalFieldOfView(_baseHorizontalFov, aspect);
            _camera.fieldOfView = calculatedVFov;
        }
    }
}
