using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace EndlessPlane.Core
{
    public class WaveDataParser
    {
        public static WaveDataParser Instance
        {
            get
            {
                if (s_instance == null)
                    s_instance = new WaveDataParser();
                return s_instance;
            }
        }
        static WaveDataParser s_instance;

        private WaveDataParser() { }

        public WaveData[] ParseWaveData(JToken data)
        {
            if (data == null)
                return null;
            var waveData = (JArray)data["wave-data"];
            if (waveData == null)
                return null;
            // Debug.Log("wave-data count: " + waveData.Count);
            if (waveData.Count <= 0)
                return null;
            WaveData[] waveDatas = new WaveData[waveData.Count];
            Dictionary<string, ObstacleSpecificPosition> obstacleSpecificPositions = new Dictionary<string, ObstacleSpecificPosition>();
            for (int i = 0; i < waveData.Count; i++)
            {
                waveDatas[i] = new WaveData();
                string obstacleHolderIdsRawData = waveData[i]["obstacle-holder-ids"].ToString();
                if (string.IsNullOrEmpty(obstacleHolderIdsRawData))
                    obstacleHolderIdsRawData = "2,4,6";
                List<int> obstacleHolderIds = new List<int>();
                if (obstacleHolderIdsRawData.Contains(','))
                {
                    string[] obstacleHolderIdsArray = obstacleHolderIdsRawData.Split(',');
                    for (int j = 0; j < obstacleHolderIdsArray.Length; j++)
                    {
                        // Debug.Log("obstacleHolderIdsArray[" + j + "]: " + obstacleHolderIdsArray[j]);
                        if (int.TryParse(obstacleHolderIdsArray[j], out int obstacleHolderId))
                        {
                            obstacleHolderIds.Add(obstacleHolderId);
                            continue;
                        }
                        obstacleHolderIds.Add(j);
                    }
                }
                else
                {
                    // Debug.Log("obstacleHolderIdsRawData: " + obstacleHolderIdsRawData);
                    if (int.TryParse(obstacleHolderIdsRawData, out int obstacleHolderId))
                    {
                        obstacleHolderIds.Add(obstacleHolderId);
                    }
                    else
                    {
                        obstacleHolderIds.Add(0);
                    }
                }
                waveDatas[i].ObstacleHolderIds = obstacleHolderIds;

                int totalSubWaves = waveData[i]["total-sub-waves"].ToObject<int>();
                // Debug.Log("totalSubWaves: " + totalSubWaves);
                if (totalSubWaves <= 0)
                    totalSubWaves = 1;
                waveDatas[i].TotalSubWaves = totalSubWaves;
                bool mandatoryObstacleIncluded = waveData[i]["mandatory-obstacle-included"].ToObject<bool>();
                // Debug.Log("mandatoryObstacleIncluded: " + mandatoryObstacleIncluded);
                var obstacles = (JArray)waveData[i]["obstacles"];
                if (obstacles == null)
                    continue;
                if (obstacles.Count <= 0)
                    continue;
                // Debug.Log("obstacles count: " + obstacles.Count);
                if (mandatoryObstacleIncluded)
                {
                    var firstToken = obstacles.First;
                    obstacles.Remove(firstToken);
                    ObstacleData obstacleData = new ObstacleData();
                    string obstacleId = firstToken["obstacle-id"].ToString();
                    // Debug.Log("obstacleId: " + obstacleId);
                    string obstaclePosRawData = firstToken["obstacle-pos"].ToString();
                    if (string.IsNullOrEmpty(obstaclePosRawData))
                        obstaclePosRawData = "0.0,0.0";
                    if (!obstaclePosRawData.Contains(','))
                        obstaclePosRawData = "0.0,0.0";
                    Vector2 obstacleSpawnPos = Vector2.zero;
                    string[] obstaclePosArray = obstaclePosRawData.Split(',');
                    if (obstaclePosArray.Length == 2)
                    {
                        if (float.TryParse(obstaclePosArray[0], out float obstaclePosX))
                            obstacleSpawnPos.x = obstaclePosX;
                        if (float.TryParse(obstaclePosArray[1], out float obstaclePosY))
                            obstacleSpawnPos.y = obstaclePosY;
                    }
                    obstacleData.ObstacleId = obstacleId;
                    if (obstacleSpecificPositions.ContainsKey(obstacleId))
                        obstacleSpecificPositions[obstacleId].Add(obstacleSpawnPos);
                    else
                        obstacleSpecificPositions.Add(obstacleId, new ObstacleSpecificPosition(obstacleSpawnPos));
                    waveDatas[i].MandatoryObstacle = obstacleData;
                }
                // Debug.Log("obstacles count: " + obstacles.Count);
                ObstacleData[] obstacleDatas = new ObstacleData[obstacles.Count];
                for (int k = 0; k < obstacles.Count; k++)
                {
                    obstacleDatas[k] = new ObstacleData();
                    string obstacleId = obstacles[k]["obstacle-id"].ToString();
                    // Debug.Log("obstacleId: " + obstacleId);
                    obstacleDatas[k].ObstacleId = obstacleId;
                    string obstaclePosRawData = obstacles[k]["obstacle-pos"].ToString();
                    if (string.IsNullOrEmpty(obstaclePosRawData))
                        obstaclePosRawData = "0.0,0.0";
                    if (!obstaclePosRawData.Contains(','))
                        obstaclePosRawData = "0.0,0.0";
                    Vector2 obstacleSpawnPos = Vector2.zero;
                    string[] obstaclePosArray = obstaclePosRawData.Split(',');
                    if (obstaclePosArray.Length != 2)
                        continue;
                    if (float.TryParse(obstaclePosArray[0], out float obstaclePosX))
                        obstacleSpawnPos.x = obstaclePosX;
                    if (float.TryParse(obstaclePosArray[1], out float obstaclePosY))
                        obstacleSpawnPos.y = obstaclePosY;
                    if (obstacleSpecificPositions.ContainsKey(obstacleId))
                        obstacleSpecificPositions[obstacleId].Add(obstacleSpawnPos);
                    else
                        obstacleSpecificPositions.Add(obstacleId, new ObstacleSpecificPosition(obstacleSpawnPos));
                }
                waveDatas[i].Obstacles = obstacleDatas;
            }
            WaveData.ObstacleSpecificPositions = obstacleSpecificPositions;
            // Debug.Log("_waveDatas.Length: " + waveDatas.Length);
            // for (int i = 0; i < waveDatas.Length; i++)
            // {
            //     Debug.Log("_waveDatas[" + i + "]: " + waveDatas[i]);
            // }
            // print obstacle specific positions
            // foreach (var item in obstacleSpecificPositions)
            // {
            //     Debug.Log("key: "+item.Key+" value: "+item.Value+"\n");
            // }
            return waveDatas;
        }
    }

    [Serializable]
    public class WaveData
    {
        [SerializeField]
        List<int> _obstacleHolderIds = null;
        [SerializeField]
        int _totalSubWaves = 0;
        [SerializeField]
        ObstacleData _mandatoryObstacle = null;
        [SerializeField]
        ObstacleData[] _obstacles = null;
        [SerializeField]
        static Dictionary<string, ObstacleSpecificPosition> _obstacleSpecificPositions = null;

        public List<int> ObstacleHolderIds { set => _obstacleHolderIds = value; }
        public int TotalSubWaves { get => _totalSubWaves; set => _totalSubWaves = value; }
        public ObstacleData MandatoryObstacle { set => _mandatoryObstacle = value; }
        public ObstacleData[] Obstacles { set => _obstacles = value; }
        public static Dictionary<string, ObstacleSpecificPosition> ObstacleSpecificPositions { set => _obstacleSpecificPositions = value; }

        int _previousWave = -1;
        int _currentIndex;

        public void ResetData()
        {
            _previousWave = -1;
            _currentIndex = 0;
        }

        public ObstacleData GetObstacleData(int obstacleHolderId, int currentWave)
        {
            if (_obstacleHolderIds == null)
                return null;
            if (_obstacleHolderIds.Count <= 0)
                return null;
            if (!_obstacleHolderIds.Contains(obstacleHolderId))
                return null;
            return GetObstacleData(currentWave);
        }

        ObstacleData GetObstacleData(int currentWave)
        {
            ObstacleData obstacleData;
            if (_previousWave != currentWave)
            {
                _previousWave = currentWave;
                if (_mandatoryObstacle != null && !string.IsNullOrEmpty(_mandatoryObstacle.ObstacleId))
                {
                    AssignPosition(_mandatoryObstacle);
                    return _mandatoryObstacle;
                }
                obstacleData = GetObstacle();
            }
            else
            {
                obstacleData = GetObstacle();
            }
            AssignPosition(obstacleData);
            return obstacleData;
        }

        ObstacleData GetObstacle()
        {
            if (_obstacles == null)
                return null;
            if (_obstacles.Length <= 0)
                return null;
            ObstacleData obstacleData;
            if (_currentIndex < _obstacles.Length)
            {
                obstacleData = _obstacles[_currentIndex];
                _currentIndex++;
            }
            else
            {
                ShuffleObstacles();
                _currentIndex = 0;
                obstacleData = _obstacles[_currentIndex];
                _currentIndex++;
            }
            return obstacleData;
        }

        void ShuffleObstacles()
        {
            if (_obstacles == null)
                return;
            if (_obstacles.Length <= 1)
                return;
            ObstacleData obstacleData = _obstacles[0];
            for (int i = 0; i < _obstacles.Length - 1; i++)
            {
                _obstacles[i] = _obstacles[i + 1];
            }
            _obstacles[_obstacles.Length - 1] = obstacleData;
        }

        void AssignPosition(ObstacleData obstacleData)
        {
            if (_obstacleSpecificPositions == null)
                obstacleData.SpawnPos = Vector2.zero;
            if (_obstacleSpecificPositions.Count <= 0)
                obstacleData.SpawnPos = Vector2.zero;
            if (_obstacleSpecificPositions[obstacleData.ObstacleId] == null)
                obstacleData.SpawnPos = Vector2.zero;
            obstacleData.SpawnPos = _obstacleSpecificPositions[obstacleData.ObstacleId].GetPos();
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("obstacleHolderIds: ");
            if (_obstacleHolderIds == null)
                stringBuilder.Append("null");
            else if (_obstacleHolderIds.Count <= 0)
                stringBuilder.Append("empty");
            else
            {
                int i = 0;
                foreach (var item in _obstacleHolderIds)
                {
                    stringBuilder.Append(item);
                    if (i != _obstacleHolderIds.Count - 1)
                        stringBuilder.Append(",");
                    i++;
                }
            }
            stringBuilder.AppendLine();
            stringBuilder.Append("totalSubWaves: ");
            stringBuilder.Append(_totalSubWaves);
            stringBuilder.AppendLine();
            stringBuilder.Append("mandatoryObstacle: ");
            if (_mandatoryObstacle != null)
                stringBuilder.Append(_mandatoryObstacle);
            else
                stringBuilder.Append("null");
            stringBuilder.AppendLine();
            stringBuilder.Append("Obstacles: [");
            if (_obstacles == null)
                stringBuilder.Append("null");
            else if (_obstacles.Length <= 0)
                stringBuilder.Append("empty");
            else
            {
                stringBuilder.AppendLine();
                for (int i = 0; i < _obstacles.Length; i++)
                {
                    stringBuilder.Append("obstacle[" + i + "]: ");
                    stringBuilder.Append(_obstacles[i]);
                    stringBuilder.AppendLine();
                }
            }
            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }
    }

    [Serializable]
    public class ObstacleData
    {
        [SerializeField]
        string _obstacleId = null;
        [SerializeField]
        Vector2 _spawnPos = Vector2.zero;

        public string ObstacleId { get => _obstacleId; set => _obstacleId = value; }
        public Vector2 SpawnPos { get => _spawnPos; set => _spawnPos = value; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("obstacleId: ");
            stringBuilder.Append(_obstacleId);
            stringBuilder.Append(" ");
            stringBuilder.Append("spawnPos: ");
            stringBuilder.Append(_spawnPos.x + "," + _spawnPos.y);
            return stringBuilder.ToString();
        }
    }

    [Serializable]
    public class ObstacleSpecificPosition
    {
        [SerializeField]
        List<Vector2> _spawnPositions = null;

        int _currentIndex;

        public ObstacleSpecificPosition(Vector2 pos)
        {
            if (_spawnPositions == null)
                _spawnPositions = new List<Vector2>();
            _spawnPositions.Add(pos);
        }

        public Vector2 GetPos()
        {
            if (_spawnPositions == null)
                return Vector2.zero;
            if (_spawnPositions.Count <= 0)
                return Vector2.zero;
            if (_currentIndex >= _spawnPositions.Count)
                _currentIndex = 0;
            Vector2 pos = _spawnPositions[_currentIndex];
            _currentIndex++;
            return pos;
        }

        public void Add(Vector2 pos)
        {
            if (_spawnPositions == null)
                _spawnPositions = new List<Vector2>();
            _spawnPositions.Add(pos);
        }

        public override string ToString()
        {
            if (_spawnPositions == null)
                return null;
            if (_spawnPositions.Count <= 0)
                return null;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Spawn positions");
            stringBuilder.AppendLine();
            stringBuilder.Append("{");
            stringBuilder.AppendLine();
            foreach (var item in _spawnPositions)
            {
                stringBuilder.Append("x:" + item.x + " y:" + item.y);
                stringBuilder.AppendLine();
            }
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }
    }
}
