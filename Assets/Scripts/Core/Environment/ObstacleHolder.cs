using System.Collections;
using UnityEngine;

namespace EndlessPlane.Core.Environment
{
    public class ObstacleHolder : MonoBehaviour
    {
        [SerializeField]
        MovableBox _movableBox = null;
        [SerializeField]
        Transform _transform = null;
        [SerializeField]
        int _id = 0;

        Obstacle _obstacle;
        bool _shouldChangeColorAlpha = false;

        void Start()
        {
            _movableBox.NotifyWhenReachedTarget(TargetReached);
            _movableBox.NotifyColorAlpha(ColorAlphaCallback);
            _movableBox.NotifyAssignObstacle(AssignObstacle);
        }

        void TargetReached()
        {
            // Debug.LogError("TargetReached");
            // return currently assigned obstacle to its manager
            if (_obstacle != null)
            {
                ObstaclesManager.Instance.ReturnObstacle(_obstacle);
                _obstacle = null;
            }
        }

        void ColorAlphaCallback(float alpha)
        {
            if (_shouldChangeColorAlpha)
            {
                if (alpha > 0.5f)
                {
                    _shouldChangeColorAlpha = false;
                    _obstacle.UpdateMaterialAlpha(1.0f);
                    return;
                }
                _obstacle.UpdateMaterialAlpha(alpha * 2.0f);
            }
        }

        void AssignObstacle()
        {
            (Obstacle, Vector2) obstacle = ObstaclesManager.Instance.GetObstacle(_id);
            if (obstacle.Item1 == null)
                return;
            // Debug.Log("AssignObstacle obstacle id: "+obstacle.Item1.Id);
            obstacle.Item1.Activate(false);
            obstacle.Item1.SetParent(_transform);
            obstacle.Item1.AssignPosition(obstacle.Item2);
            _shouldChangeColorAlpha = true;
            _obstacle = obstacle.Item1;
            StartCoroutine(Coroutine_ActivateAfterFrames());
        }

        IEnumerator Coroutine_ActivateAfterFrames()
        {
            yield return null;
            yield return null;
            if (_obstacle != null)
                _obstacle.Activate(true);
        }
    }
}
