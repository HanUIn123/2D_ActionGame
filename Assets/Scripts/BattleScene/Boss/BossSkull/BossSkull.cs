using UnityEngine;
using System.Collections;

public class BossSkull : MonsterParent
{
    [Header("보스 패턴 설정")]
    public GameObject                                       m_slashPrefab;
    public Transform                                        m_firePoint;
    public float                                            m_fSkillCoolTime = 4.0f;

    private float                                           m_fSkillTimer = 0f;

    protected override void UpdateAction()
    {
        bool isBattleStarted = (FlockingManager.Instance != null) && FlockingManager.Instance.m_isBattleStarted;
        int currentFloor = (GameDataManager.Instance != null) ? GameDataManager.Instance.m_iCurrentFloorIndex : 0;

        if (isBattleStarted && m_iFloorIndex == currentFloor)
        {
            m_fSkillTimer += Time.deltaTime;

            if (m_fSkillTimer >= m_fSkillCoolTime)
            {
                if (m_animator != null)
                    m_animator.SetBool("IsFiring", true);

                m_fSkillTimer = 0f;
            }
        }
    }

    // 이거 안씀;
    public void FireSlashEvent()
    {
        if (m_slashPrefab != null && m_firePoint != null)
        {
            Instantiate(m_slashPrefab, m_firePoint.position, Quaternion.identity);
        }
    }

    public void EndFireEvent()
    {
        if (m_animator != null)
            m_animator.SetBool("IsFiring", false);
    }
}
