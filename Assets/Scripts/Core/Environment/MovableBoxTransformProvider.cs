using UnityEngine;

namespace EndlessPlane.Core.Environment
{
    public class MovableBoxTransformProvider : MonoBehaviour
    {
        [SerializeField]
        Transform _spawnRef = null;
        [SerializeField]
        float _positionToAngleConversionRate = -10.0f;

        public static MovableBoxTransformProvider Instance => s_instance;

        static MovableBoxTransformProvider s_instance;

        void Awake()
        {
            s_instance = this;
        }

        public Vector3 GetSpawnPosition()
        {
            return _spawnRef.position;
        }

        public Quaternion GetSpawnRotation()
        {
            Quaternion rotation = _spawnRef.rotation;
            Vector3 spawnEulerAngles = rotation.eulerAngles;
            spawnEulerAngles.z = ArcHandler.Instance.StartAndhorPosition.x * _positionToAngleConversionRate;
            spawnEulerAngles.x = ArcHandler.Instance.StartAndhorPosition.y * _positionToAngleConversionRate;
            rotation.eulerAngles = spawnEulerAngles;
            _spawnRef.rotation = rotation; 
            return _spawnRef.rotation;
        }
    }
}
