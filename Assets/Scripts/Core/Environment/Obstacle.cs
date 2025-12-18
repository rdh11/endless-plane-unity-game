using UnityEngine;

namespace EndlessPlane.Core.Environment
{
    public class Obstacle : MonoBehaviour
    {
        [SerializeField]
        protected Transform _transform = null;
        [SerializeField]
        float _maxHorizontalSpawn = 0.0f;
        [SerializeField]
        float _minHorizontalSpawn = 0.0f;
        [SerializeField]
        float _maxVerticalSpawn = 0.0f;
        [SerializeField]
        float _minVerticalSpawn = 0.0f;
        [SerializeField]
        float _forwardSpawn = 0.0f;
        [SerializeField]
        Renderer _meshRenderer = null;
        [SerializeField]
        GameObject _gameObject = null;
        [SerializeField]
        string _id = null;

        public bool IsAvailable { get => _isAvailable; set => _isAvailable = value; }
        public float MinHorizontalSpawn => _minHorizontalSpawn;
        public float MaxHorizontalSpawn => _maxHorizontalSpawn;
        public float MinVerticalSpawn => _minVerticalSpawn;
        public float MaxVerticalSpawn => _maxVerticalSpawn;
        public string Id => _id;

        Material _material;
        Color _materialColor;
        Quaternion _initialRotation;
        bool _isAvailable = true;

        protected virtual void Start()
        {
            _material = _meshRenderer.material;
            _materialColor = _material.color;
            _initialRotation = _transform.rotation;
        }

        public virtual void AssignPosition(Vector2 pos)
        {
            _transform.localPosition = new Vector3(pos.x, pos.y, _forwardSpawn);
            _transform.localRotation = _initialRotation;
        }

        public virtual void ResetPosition(Vector3 position)
        {
            _transform.localPosition = position;
            _transform.localRotation = _initialRotation;
        }

        public void SetParent(Transform transform)
        {
            _transform.SetParent(transform);
        }

        public void UpdateMaterialAlpha(float alpha)
        {
            _materialColor.a = alpha;
            _material.color = _materialColor;
            _meshRenderer.material = _material;
        }

        public void Activate(bool status)
        {
            _gameObject.SetActive(status);
        }
    }
}
