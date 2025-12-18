using UnityEngine;

namespace EndlessPlane.Core.Environment
{
    public class ObstaclesManager : MonoBehaviour
    {
        [SerializeField]
        Obstacle[] _obstacles = null;
        [SerializeField]
        Transform _obstacleBaseParent = null;
        [SerializeField]
        Transform _obstacleBaseTransform = null;
        [SerializeField]
        ObstacleHolder[] _obstacleHolders = null;

        public bool LoadObstacles { get => _loadObstacles; set => _loadObstacles = value; }

        public static ObstaclesManager Instance => s_instance;
        static ObstaclesManager s_instance;

        int _totalObstacles;
        bool _loadObstacles = false;
        int _totalObstacleHolders;
        int _currentWave = 0;
        bool _skipNextWave = true;
        WaveData _waveData;
        WaveData[] _waveDatas;
        int _waveDataIndex;

        void Awake()
        {
            s_instance = this;
        }

        void Start()
        {
            Init();
        }

        void Init()
        {
            if (_obstacles == null)
            {
                Debug.LogError("Obstacles not initialized.");
                return;
            }
            if (_obstacles.Length <= 0)
            {
                Debug.LogError("No Obstacles.");
                return;
            }
            if (_obstacleHolders == null)
            {
                Debug.LogError("Obstacle Holders not initialized.");
                return;
            }
            if (_obstacleHolders.Length <= 0)
            {
                Debug.LogError("No Obstacle Holders.");
                return;
            }

            _totalObstacles = _obstacles.Length;
            _totalObstacleHolders = _obstacleHolders.Length;
        }

        public void InitObstacleWaves(WaveData[] waveDatas)
        {
            _waveDatas = waveDatas;
            UpdateWaveData();
        }

        public (Obstacle, Vector2) GetObstacle(int obstacleHolderId)
        {
            if (!_loadObstacles)
                return (null, Vector2.zero);
            ObstacleData obstacleData = GetObstacleData(obstacleHolderId);
            if (obstacleData == null)
                return (null, Vector2.zero);
            if (_totalObstacles <= 0)
                return (null, Vector2.zero);
            Obstacle obstacle = null;
            Vector2 pos = obstacleData.SpawnPos;
            foreach (var obstacleRef in _obstacles)
            {
                // Debug.Log("obstacleRef.Id: "+obstacleRef.Id+" obstacleData.ObstacleId: "+obstacleData.ObstacleId);
                if (obstacleRef.Id.Equals(obstacleData.ObstacleId))
                {
                    obstacle = obstacleRef;
                    break;
                }
            }
            if (obstacle.IsAvailable)
            {
                obstacle.IsAvailable = false;
                return (obstacle, pos);
            }
            return (null, pos);
        }

        public void ReturnObstacle(Obstacle obstacle)
        {
            if (obstacle == null)
                return;
            obstacle.SetParent(_obstacleBaseParent);
            obstacle.ResetPosition(_obstacleBaseTransform.position);
            obstacle.IsAvailable = true;
        }

        ObstacleData GetObstacleData(int obstacleHolderId)
        {
            if (obstacleHolderId == _totalObstacleHolders)
            {
                if (_skipNextWave)
                {
                    _skipNextWave = false;
                    return null;
                }
                else
                {
                    // PlayStatsHandler.Instance.UpdateSpawnedWaveCount();
                    ObstacleData obstacleData = _waveData.GetObstacleData(obstacleHolderId, _currentWave);
                    _currentWave++;
                    if (_currentWave == _waveData.TotalSubWaves)
                    {
                        _currentWave = 0;
                        _skipNextWave = true;
                        UpdateWaveData();
                    }
                    return obstacleData;
                }
            }
            if (_skipNextWave)
                return null;
            return _waveData.GetObstacleData(obstacleHolderId, _currentWave);
        }

        void UpdateWaveData()
        {
            if (_waveData != null)
                _waveData.ResetData();
            if (_waveDatas == null)
                return;
            if (_waveDatas.Length <= 0)
                return;
            if (_waveDataIndex < _waveDatas.Length)
            {
                _waveData = _waveDatas[_waveDataIndex];
                _waveDataIndex++;
            }
        }
    }
}
