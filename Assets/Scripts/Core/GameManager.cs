using System.Collections.Generic;
using UnityEngine;

namespace EndlessPlane.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance => s_instance;
        static GameManager s_instance;

        List<IUpdateable> _updateables;
        float _deltaTime;
        bool _gameStarted = true;

        void Awake()
        {
            s_instance = this;

            // set target frame rate to 60
            Application.targetFrameRate = 60;
        }

        // Update is called once per frame
        void Update()
        {
            if (CanUpdate())
            {
                _deltaTime = Time.deltaTime;
                for (int i = 0; i < _updateables.Count; i++)
                {
                    _updateables[i].Update(_deltaTime);
                }
            }
        }

        public void RegisterUpdateable(IUpdateable updateable)
        {
            if (updateable == null)
                return;
            if (_updateables == null)
                _updateables = new List<IUpdateable>();
            if (_updateables.Contains(updateable))
                return;
            _updateables.Add(updateable);
        }

        bool CanUpdate()
        {
            if (_updateables == null)
                return false;
            if (_updateables.Count <= 0)
                return false;
            return _gameStarted;
        }
    }

    public interface IUpdateable
    {
        void Update(float deltaTime);
    }
}
