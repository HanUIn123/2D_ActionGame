using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TowerMasterData", menuName = "MyGame/Tower Master Data")]
public class TowerMasterData : ScriptableObject
{
    // "무엇을 몇 마리" 담는 단위
    [System.Serializable]
    public struct SpawnUnit                     
    {   
        public GameObject               m_monsterPrefab;      // 소환할 몹 (해골, 고블린 등)
        public int                      m_count;              // 연속으로 소환할 마릿수
    }

    [System.Serializable]
    public struct FloorInfo
    {
        public string                   m_strFloorName;

        // [해골 5마리], [고블린 1마리], [슬라임 2마리] 순서로 등록
        public List<SpawnUnit>          m_spawnPatterns;

        public float                    m_fSpawnInterval;
        public float                    m_fHpMultiplier;
    }

    public List<FloorInfo>              m_listFloors = new List<FloorInfo>();
}