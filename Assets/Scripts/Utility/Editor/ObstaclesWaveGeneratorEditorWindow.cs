using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using EndlessPlane.Core.Environment;

namespace EndlessPlane.Utility.Editor
{
    public class ObstaclesWaveGeneratorEditorWindow : EditorWindow
    {
        // ---------- Data Models (Editor-only) ----------

        enum ObstacleType
        {
            Obstacle,
            SmartObstacle
        }

        [Serializable]
        class SerializableWaveData
        {
            public string obstacleHolderIds = "2,4,6";
            public int totalSubWaves = 1;
            public bool mandatoryObstacleIncluded = false;
            public ObstacleType mandatoryObstacleType = ObstacleType.Obstacle;
        }

        [Serializable]
        class ObstacleData
        {
            [JsonProperty("obstacle-id")]
            public string ObstacleId;

            [JsonIgnore]
            public ObstacleType ObstacleType;

            [JsonProperty("obstacle-pos")]
            public string ObstaclePos;
        }

        [Serializable]
        class WaveData
        {
            [JsonProperty("obstacle-holder-ids")]
            public string ObstacleHolderIds;

            [JsonProperty("total-sub-waves")]
            public int TotalSubWaves;

            [JsonProperty("mandatory-obstacle-included")]
            public bool MandatoryObstacleIncluded;

            [JsonProperty("obstacles")]
            public List<ObstacleData> Obstacles;
        }

        [Serializable]
        class WaveDataResponse
        {
            [JsonProperty("wave-data")]
            public List<WaveData> WaveDataList;
        }

        // ---------- Editor State (NOT persistent) ----------

        [SerializeField] List<Obstacle> _obstaclePrefabs = new();
        [SerializeField] List<SmartObstacle> _smartObstaclePrefabs = new();
        [SerializeField] List<SerializableWaveData> _waves = new();

        Vector2 _scroll;

        static readonly System.Random _random = new();

        // ---------- Menu ----------

        [MenuItem("Tools/Wave Data Generator")]
        static void Open()
        {
            GetWindow<ObstaclesWaveGeneratorEditorWindow>(
                "Wave Data Generator"
            );
        }

        // ---------- UI ----------

        void OnGUI()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            DrawObstacleSection();
            EditorGUILayout.Space(15);
            DrawWaveSection();
            EditorGUILayout.Space(20);
            DrawGenerateButton();

            EditorGUILayout.EndScrollView();
        }

        void DrawObstacleSection()
        {
            EditorGUILayout.LabelField("Obstacle Prefabs", EditorStyles.boldLabel);
            DrawPrefabList(_obstaclePrefabs);

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Smart Obstacle Prefabs", EditorStyles.boldLabel);
            DrawPrefabList(_smartObstaclePrefabs);
        }

        void DrawPrefabList<T>(List<T> list) where T : UnityEngine.Object
        {
            int removeIndex = -1;

            for (int i = 0; i < list.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                list[i] = (T)EditorGUILayout.ObjectField(
                    list[i],
                    typeof(T),
                    false
                );

                if (GUILayout.Button("X", GUILayout.Width(22)))
                    removeIndex = i;

                EditorGUILayout.EndHorizontal();
            }

            if (removeIndex >= 0)
                list.RemoveAt(removeIndex);

            if (GUILayout.Button("+ Add"))
                list.Add(null);
        }

        void DrawWaveSection()
        {
            EditorGUILayout.LabelField("Waves", EditorStyles.boldLabel);

            int removeIndex = -1;

            for (int i = 0; i < _waves.Count; i++)
            {
                var wave = _waves[i];

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Wave {i}", EditorStyles.boldLabel);
                if (GUILayout.Button("Remove", GUILayout.Width(70)))
                    removeIndex = i;
                EditorGUILayout.EndHorizontal();

                wave.obstacleHolderIds =
                    EditorGUILayout.TextField("Obstacle Holder IDs", wave.obstacleHolderIds);

                wave.totalSubWaves =
                    Mathf.Max(1, EditorGUILayout.IntField("Total Sub Waves", wave.totalSubWaves));

                wave.mandatoryObstacleIncluded =
                    EditorGUILayout.Toggle("Mandatory Obstacle Included", wave.mandatoryObstacleIncluded);

                using (new EditorGUI.DisabledScope(!wave.mandatoryObstacleIncluded))
                {
                    wave.mandatoryObstacleType =
                        (ObstacleType)EditorGUILayout.EnumPopup(
                            "Mandatory Obstacle Type",
                            wave.mandatoryObstacleType
                        );
                }

                EditorGUILayout.EndVertical();
            }

            if (removeIndex >= 0)
                _waves.RemoveAt(removeIndex);

            if (GUILayout.Button("+ Add Wave"))
                _waves.Add(new SerializableWaveData());
        }

        void DrawGenerateButton()
        {
            GUI.enabled = _waves.Count > 0;

            if (GUILayout.Button("Generate WaveData.json", GUILayout.Height(35)))
            {
                GenerateJson();
            }

            GUI.enabled = true;
        }

        // ---------- Generation Logic ----------

        void GenerateJson()
        {
            if (_obstaclePrefabs.Count == 0 && _smartObstaclePrefabs.Count == 0)
            {
                EditorUtility.DisplayDialog(
                    "Error",
                    "No obstacle prefabs assigned.",
                    "OK"
                );
                return;
            }

            var waveDataList = new List<WaveData>();

            foreach (var wave in _waves)
            {
                var obstacles = GenerateInitialObstacles();
                Shuffle(obstacles);

                if (wave.mandatoryObstacleIncluded)
                {
                    var mandatory = obstacles.FirstOrDefault(
                        o => o.ObstacleType == wave.mandatoryObstacleType
                    );

                    if (mandatory == null)
                    {
                        EditorUtility.DisplayDialog(
                            "Error",
                            $"Mandatory obstacle of type {wave.mandatoryObstacleType} not found.",
                            "OK"
                        );
                        return;
                    }

                    obstacles.Remove(mandatory);
                    obstacles.Insert(0, mandatory);
                }

                waveDataList.Add(new WaveData
                {
                    ObstacleHolderIds = wave.obstacleHolderIds,
                    TotalSubWaves = wave.totalSubWaves,
                    MandatoryObstacleIncluded = wave.mandatoryObstacleIncluded,
                    Obstacles = obstacles
                });
            }

            var json = JsonConvert.SerializeObject(
                new WaveDataResponse { WaveDataList = waveDataList },
                Formatting.Indented
            );

            string folder = "Assets/Data";
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string path = Path.Combine(folder, "WaveData.json");
            File.WriteAllText(path, json);

            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog(
                "Success",
                "WaveData.json generated successfully.",
                "OK"
            );
        }

        List<ObstacleData> GenerateInitialObstacles()
        {
            var list = new List<ObstacleData>();

            foreach (var prefab in _obstaclePrefabs.Where(o => o != null))
            {
                list.Add(CreateObstacle(prefab.Id, ObstacleType.Obstacle, prefab));
            }

            foreach (var prefab in _smartObstaclePrefabs.Where(o => o != null))
            {
                list.Add(CreateObstacle(prefab.Id, ObstacleType.SmartObstacle, prefab));
            }

            return list;
        }

        ObstacleData CreateObstacle(string id, ObstacleType type, Obstacle data)
        {
            return new ObstacleData
            {
                ObstacleId = id,
                ObstacleType = type,
                ObstaclePos = GenerateRandomPosition(
                    data.MinHorizontalSpawn,
                    data.MaxHorizontalSpawn,
                    data.MinVerticalSpawn,
                    data.MaxVerticalSpawn
                )
            };
        }

        static string GenerateRandomPosition(float minX, float maxX, float minY, float maxY)
        {
            float x = (float)(_random.NextDouble() * (maxX - minX) + minX);
            float y = (float)(_random.NextDouble() * (maxY - minY) + minY);
            return $"{x:F2},{y:F2}";
        }

        static void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int k = _random.Next(i + 1);
                (list[i], list[k]) = (list[k], list[i]);
            }
        }
    }
}
