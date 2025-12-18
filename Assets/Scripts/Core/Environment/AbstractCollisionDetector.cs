using UnityEngine;

namespace EndlessPlane.Core.Environment
{
    public abstract class AbstractCollisionDetector : MonoBehaviour
    {
        [SerializeField]
        protected Obstacle _obstacle = null;

        void OnTriggerEnter(Collider other)
        {
            TriggerEntered(other);
        }

        protected abstract void TriggerEntered(Collider other);
    }
}
