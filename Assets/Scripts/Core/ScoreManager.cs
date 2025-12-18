using EndlessPlane.Core.UI;
using UnityEngine;

namespace EndlessPlane.Core
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField]
        UIScore _uiScore = null;
        [SerializeField]
        int _scorePoints = 100;

        public static ScoreManager Instance => s_instance;
        static ScoreManager s_instance;

        int _totalScore;

        void Awake()
        {
            s_instance = this;
        }

        public void AddScore()
        {
            if (!GameManager.Instance.GameStarted)
                return;
            _totalScore += _scorePoints;
            _uiScore.UpdateScore(_totalScore.ToString());
        }
    }
}
