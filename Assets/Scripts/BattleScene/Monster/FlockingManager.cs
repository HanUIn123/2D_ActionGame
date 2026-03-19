using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingManager : MonoBehaviour
{
    public static FlockingManager Instance;
    public bool                                         m_isBattleStarted = false;
    public List<MonsterParent>                          m_listMonsters= new List<MonsterParent>();

    [Header("대열 이동 설정")]
    public float                                        m_fTotalMoveOffset = 0f; // 대열 전체가 왼쪽으로 이동한 총 거리
    public float                                        m_fGlobalSpeed = 2.0f;   // 기차 전진 속도

    void Awake() => Instance = this;

    // MonsterSpawner 에서 불러줌.
    public void StartBattle() 
    {
        m_isBattleStarted = true;
        m_fTotalMoveOffset = 0f; // 이동량 초기화

        int iCurrentFloor = (GameDataManager.Instance != null) ? GameDataManager.Instance.m_iCurrentFloorIndex : 0;

        foreach (MonsterParent mon in m_listMonsters)
        {
            if (mon == null || mon.m_iFloorIndex != iCurrentFloor) 
                continue;

            mon.SetRelativeX();
        }
    }

    void Update()
    {
        if (!m_isBattleStarted || m_listMonsters.Count == 0) 
            return;

        bool isBossGroggy = false;

        float fCurrentSpeed = m_fGlobalSpeed; // 기본 속도

        int iCurrentFloor = (GameDataManager.Instance != null) ? GameDataManager.Instance.m_iCurrentFloorIndex : 0;

        foreach (MonsterParent mon in m_listMonsters)
        {
            // 내 층에 있는 보스 찾기 (BossSkull이나 BossGoblin)
            if (mon != null && mon.m_iFloorIndex == iCurrentFloor && (mon is BossGoblin || mon is BossSkull))
            {
                // 보스가 그로기인지 확인
                if (mon is BossGoblin boss && boss.m_isGroggy)
                {
                    isBossGroggy = true;
                }

                fCurrentSpeed = mon.m_fMoveSpeed;

                break;
            }
        }

        if (!isBossGroggy)
        {
            m_fTotalMoveOffset -= Time.deltaTime * fCurrentSpeed;
        }

        // 몬스터 위치 고정
        CalculateFlocking();
    }

    void CalculateFlocking()
    {
        int iCurrentFloor = (GameDataManager.Instance != null) ? GameDataManager.Instance.m_iCurrentFloorIndex : 0;

        foreach (MonsterParent mon in m_listMonsters)
        {
            if (mon == null || mon.m_iFloorIndex != iCurrentFloor)
                continue;

            // 원래 자기 위치 + 대열 전체의 이동값
            Vector3 v3TargetPos = mon.transform.position;

            v3TargetPos.x = mon.m_fRelativeX + m_fTotalMoveOffset;

            mon.transform.position = v3TargetPos;
        }
    }

    public void PushBackAllMonsters(Vector3 _v3PlayerPos, float _fPushForce, float _fDefRange)
    {
        int iCurrentFloor = GameDataManager.Instance.m_iCurrentFloorIndex;

        // 범위 안에 한 마리라도 있는지 체크
        bool isAnyInRange = false;

        foreach (MonsterParent monster in m_listMonsters)
        {
            if (monster == null || monster.m_iFloorIndex != iCurrentFloor) 
                continue;

            float fDist = Vector2.Distance(_v3PlayerPos, monster.transform.position);

            if (fDist <= _fDefRange)
            {
                isAnyInRange = true;
                break;
            }
        }

        // 범위 안이면 "대열 전체 이동값"을 뒤로 밀어버림 
        if (isAnyInRange)
        {
            StopAllCoroutines(); // 이전 밀기 멈춤

            StartCoroutine(PushAllRoutine(_fPushForce));
        }
    }

    private IEnumerator PushAllRoutine(float _fForce)
    {
        float fElapsed = 0f;
        float fDuration = 0.15f;
        float fStartOffset = m_fTotalMoveOffset;
        float fTargetOffset = m_fTotalMoveOffset + _fForce;

        while (fElapsed < fDuration)
        {
            fElapsed += Time.deltaTime;

            // 이동량(Offset) 자체를 부드럽게 뒤로 보냄
            m_fTotalMoveOffset = Mathf.Lerp(fStartOffset, fTargetOffset, fElapsed / fDuration);

            yield return null;
        }
    }

    // 적들 순간 밀어줌(마지노선 닿을 때) 
    public void EmergencyPushBack(float _fPushDistance)
    {
        // 진행 중인 일반 밀기 코루틴(PushAllRoutine 등)이 있다면 중단
        StopAllCoroutines();

        // 보간된 후진 코루틴 실행
        StartCoroutine(EmergencyPushRoutine(_fPushDistance));
    }

    private IEnumerator EmergencyPushRoutine(float _fDistance)
    {
        float fElapsed = 0f;
        float fDuration = 0.3f; // 0.3초 동안 빠르게 뒤로 밀림
        float fStartOffset = m_fTotalMoveOffset;
        float fTargetOffset = m_fTotalMoveOffset + _fDistance;

        while (fElapsed < fDuration)
        {
            fElapsed += Time.deltaTime;

            float progress = fElapsed / fDuration;

            // 부드럽게 뒤로 밀리는 연출 (감속 커브를 주면 더 찰짐)
            float fCurve = Mathf.Sin(progress * Mathf.PI * 0.5f);

            m_fTotalMoveOffset = Mathf.Lerp(fStartOffset, fTargetOffset, fCurve);

            // Offset이 변하면 CalculateFlocking이 Update에서 위치를 계속 갱신해줌!
            yield return null;
        }

        m_fTotalMoveOffset = fTargetOffset;
    }
}