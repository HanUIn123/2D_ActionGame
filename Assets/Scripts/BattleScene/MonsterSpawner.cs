using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("타워 배치 설정")]
    public float                                m_fFloorHeight = 3.34f;
    public float                                m_fBaseY = -1.18f;
    public float                                m_fFirstMonsterX = -2.0f;
    public float                                m_fGapX = 1.5f;

    [Header("상태 확인")]
    private int                                 m_iTotalMaxCount = 0;
    private int                                 m_iAliveMonsterCount = 0;
    private bool                                m_isWaitingInput = false;
    private bool                                m_isStageClearing = false;

    [Header("참조 연동")]
    public BackGroundScroller                   m_bgScroller;
    public BattleScene_UI                       m_battleUI;
    public PlayerController                     m_player;

    void Start()
    {
        // 씬 재진입 시 플레이어 상태 초기화
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.ResetStatsForBattle();
        }

        if (GameDataManager.Instance != null) 
            GameDataManager.Instance.m_iCurrentFloorIndex = 0;

        BuildAllFloors();

        InitFloor();

        AlignMonstersToFloor();

        StartCoroutine(ReadyToBattleRoutine());
    }

    void Update()
    {
        if (m_bgScroller != null && m_bgScroller.IsScrolling)
        {
            float fOneFloorHeight = m_bgScroller.m_fTotalHeight / 3f;
            float fWorldFloorHeight = m_fFloorHeight;
            float fFrameScrollRatio = (m_bgScroller.m_fScrollSpeed * Time.deltaTime) / fOneFloorHeight;
            float fWorldMoveAmount = fFrameScrollRatio * fWorldFloorHeight;

            // MonsterParent 타입으로 찾음
            MonsterParent[] arrMonsters = FindObjectsByType<MonsterParent>(FindObjectsSortMode.None);

            foreach (var monster in arrMonsters)
            {
                monster.transform.position += Vector3.down * fWorldMoveAmount;
            }
        }
    }

    public void OnMonsterDestroyed()
    {
        m_iAliveMonsterCount--;

        if (m_battleUI != null)
            m_battleUI.UpdateMonsterCount(m_iAliveMonsterCount, m_iTotalMaxCount);

        if (m_iAliveMonsterCount <= 0 && !m_isStageClearing)
        {
            m_isStageClearing = true;

            // 피니시 슬로우 모션 & 줌인 연출 시작!
            if (BattleEffectManager.Instance != null)
            {
                BattleEffectManager.Instance.PlayFinishSlowMotion();
            }

            StartCoroutine(NextFloorRoutine());
        }
    }

    IEnumerator NextFloorRoutine()
    {
        // 피니시 슬로우 진행 동안 잠깐 기달
        // Time.timeScale이 낮아져 있으므로 무조건 'Realtime'을 써야 한다고 한다.
        yield return new WaitForSecondsRealtime(1.5f);

        if (FlockingManager.Instance != null)
            FlockingManager.Instance.m_isBattleStarted = false;

        m_isWaitingInput = true;

        while (m_isWaitingInput)
        {
            // 화면을 터치(클릭)하면 대기 끝!
            if (Input.GetMouseButtonDown(0))
                m_isWaitingInput = false;

            yield return null;
        }

        // GOGO 닫기 호출해줌
        if (BattleEffectManager.Instance != null)
        {
            BattleEffectManager.Instance.HideNextFloorUI();
        }

        GameDataManager.Instance.IncreaseFloor();

        if (m_player != null) 
            yield return StartCoroutine(m_player.ExitToRight());

        if (m_bgScroller != null)
            m_bgScroller.StartScroll();

        while (m_bgScroller != null && m_bgScroller.IsArrived == false) 
            yield return null;

        if (m_bgScroller != null) 
            m_bgScroller.StopScroll();

        if (m_battleUI != null) 
            m_battleUI.UpdateFloorText();

        if (m_player != null) 
            yield return StartCoroutine(m_player.EnterFromLeft());

        InitFloor();

        m_isStageClearing = false;

        StartCoroutine(ReadyToBattleRoutine());
    }

    IEnumerator ReadyToBattleRoutine()
    {
        if (FlockingManager.Instance != null)
            FlockingManager.Instance.m_isBattleStarted = false;

        // 보스 UI 체크 (기존 코드)
        CheckAndShowBossUI();

        // 플레이어의 입력을 기다림 (화면 클릭 시 전투 시작)
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
                break;

            yield return null;
        }

        // 여기서 StartBattle을 호출
        if (FlockingManager.Instance != null)
        {
            // 이 함수가 실행되면서 현재 층 몬스터들의 번호표(RelativeX)를 새로 찍어줌!
            FlockingManager.Instance.StartBattle();
        }
    }

    void InitFloor()
    {
        var floorInfo = GameDataManager.Instance.GetCurrentFloorInfo();

        if (string.IsNullOrEmpty(floorInfo.m_strFloorName))
            return;

        int iTotalCount = 0;

        foreach (var pattern in floorInfo.m_spawnPatterns)
        {
            iTotalCount += pattern.m_count;
        }

        m_iTotalMaxCount = iTotalCount;
        m_iAliveMonsterCount = m_iTotalMaxCount;

        if (m_battleUI != null)
        {
            m_battleUI.UpdateFloorText();
            m_battleUI.UpdateMonsterCount(m_iAliveMonsterCount, m_iTotalMaxCount);
        }
    }

    void BuildAllFloors()
    {
        var towerData = GameDataManager.Instance.m_towerMasterData;

        if (towerData == null) 
            return;

        for (int i = 0; i < towerData.m_listFloors.Count; i++)
        {
            var floorInfo = towerData.m_listFloors[i];

            float fFloorYOffset = i * m_fFloorHeight;
            int iGlobalIndex = 0;

            foreach (var pattern in floorInfo.m_spawnPatterns)
            {
                if (pattern.m_monsterPrefab == null) 
                    continue;

                for (int k = 0; k < pattern.m_count; k++)
                {
                    Vector3 v3SpawnPos = new Vector3(m_fFirstMonsterX + (iGlobalIndex * m_fGapX), m_fBaseY + fFloorYOffset, 0);

                    GameObject objMonster = Instantiate(pattern.m_monsterPrefab, v3SpawnPos, Quaternion.identity);

                    MonsterParent monsterParent = objMonster.GetComponent<MonsterParent>();

                    if (monsterParent != null)
                    {
                        monsterParent.m_iFloorIndex = i;
                        monsterParent.m_fHP *= floorInfo.m_fHpMultiplier;
                    }

                    iGlobalIndex++;
                }
            }
        }
    }

    public void AlignMonstersToFloor()
    {
        MonsterParent[] arrMonsters = FindObjectsByType<MonsterParent>(FindObjectsSortMode.None);

        int iCurrentFloor = GameDataManager.Instance.m_iCurrentFloorIndex;

        foreach (var monster in arrMonsters)
        {
            float fTargetY = m_fBaseY + (monster.m_iFloorIndex - iCurrentFloor) * m_fFloorHeight;

            monster.transform.position = new Vector3(monster.transform.position.x, fTargetY, 0);
        }
    }

    private void CheckAndShowBossUI()
    {
        int iCurrentFloor = GameDataManager.Instance.m_iCurrentFloorIndex;

        // 현재 맵에 깔린 몬스터 중 내 층에 있고 + BossSkull 이나 보스고블린 컴포 잇는거 찾자
        MonsterParent[] arrMonsters = FindObjectsByType<MonsterParent>(FindObjectsSortMode.None);

        foreach (var mon in arrMonsters)
        {
            if (mon.m_iFloorIndex == iCurrentFloor && (mon is BossSkull || mon is BossGoblin))
            {
                if (m_battleUI != null)
                {
                    m_battleUI.ShowBossAppearance(); // 아까 만든 깜빡이 호출!
                }
                break;
            }
        }
    }
}