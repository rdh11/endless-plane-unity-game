using UnityEngine;

namespace EndlessPlane.Core.Environment
{
    public class ObstacleCollisionDetector : AbstractCollisionDetector
    {
        protected override void TriggerEntered(Collider other)
        {
            _obstacle.CollidedWithPlayer();
        }
    }
}
