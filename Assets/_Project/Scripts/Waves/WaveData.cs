using System;
using UnityEngine;

namespace SlavicTD.Waves
{
    [Serializable]
    public struct EnemySpawnGroup
    {
        public GameObject enemyPrefab;
        public int count;
        public float spawnInterval;
        [Tooltip("Index of the spawn point marker (e.g. 0: North, 1: East Swamps, 2: South Forest)")]
        public int spawnPointIndex;
    }

    /// <summary>
    /// ScriptableObject defining an enemy wave, including enemy composition,
    /// directions, and dynamic map evolution triggers (e.g. wave 3 or 6).
    /// </summary>
    [CreateAssetMenu(fileName = "Wave_", menuName = "SlavicTD/WaveData")]
    public class WaveData : ScriptableObject
    {
        public int waveNumber;
        public int actNumber;
        public EnemySpawnGroup[] spawnGroups;

        [Header("Dynamic Map Evolution")]
        public bool triggersMapEvolution;
        [Tooltip("Identifier for the MapEvolutionTrigger to fire upon wave completion")]
        public string evolutionEventId;
    }
}
