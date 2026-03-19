using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("공격 설정")]
    public Transform                        m_weaponTip;
    public float                            m_fAtkRadius = 0.8f;
    public LayerMask                        m_enemyLayer;

    [Header("방어 설정")]
    public Transform                        m_defPoint;
    public float                            m_fDefPushForce = 5.0f; 
    public float                            m_fDefRange = 1.5f;     

    private List<MonsterParent>             m_listHitMonsters= new List<MonsterParent>();

    void Start()
    {
        // 씬오면 무기 가져오기.
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.UpdatePlayerWeaponVisual();
        }
    }

    // playerController 에서 호출해줌 
    public void CheckRealtimeCollision()
    {
        Collider2D[] col2DHitEnemies = Physics2D.OverlapCircleAll(m_weaponTip.position, m_fAtkRadius, m_enemyLayer);

        foreach (Collider2D enemy in col2DHitEnemies)
        {
            MonsterParent monsterParent = enemy.GetComponent<MonsterParent>();

            if (monsterParent != null && !m_listHitMonsters.Contains(monsterParent))
            {
                monsterParent.TakeDamage(PlayerStats.Instance.TotalAtk);

                m_listHitMonsters.Add(monsterParent);
            }
        }
    }

    public void StartAtk() => m_listHitMonsters.Clear();

    // playerController 에서 호출해줌 
    public bool IsEnemyNear()
    {
        Vector3 v3CheckPos = (m_defPoint != null) ? m_defPoint.position : transform.position;

        return Physics2D.OverlapCircle(v3CheckPos, m_fDefRange, m_enemyLayer) != null;
    }

    public void PushEnemies()
    {
        if (FlockingManager.Instance != null)
            FlockingManager.Instance.PushBackAllMonsters(transform.position, m_fDefPushForce, m_fDefRange);

        // 2. [추가] 범위 내에 보스가 있는지 확인해서 그로기 신호 보내기
        Vector3 v3CheckPos = (m_defPoint != null) ? m_defPoint.position : transform.position;

        Collider2D[] col2DHitEnemies = Physics2D.OverlapCircleAll(v3CheckPos, m_fDefRange, m_enemyLayer);

        foreach (var enemy in col2DHitEnemies)
        {
            // BossGoblin 은 따로 처리해야함
            BossGoblin bossGoblin = enemy.GetComponentInParent<BossGoblin>();

            if (bossGoblin != null)
            {
                bossGoblin.OnPushedByDefense(); 
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (m_weaponTip != null) 
        { 
            Gizmos.color = Color.red; 

            Gizmos.DrawWireSphere(m_weaponTip.position, m_fAtkRadius); 
        }

        if (m_defPoint != null) 
        { 
            Gizmos.color = Color.blue; 
            Gizmos.DrawWireSphere(m_defPoint.position, m_fDefRange); 
        }
    }
}