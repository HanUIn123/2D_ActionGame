using UnityEngine;
using System.Collections;

public class MonsterGoblin : MonsterParent
{
    [Header("고블린 스킬 설정")]
    public GameObject                       m_goblinAuraEffect;
    public float                            m_fBuffCoolTime = 4.0f;
    public float                            m_fBuffDuration = 2.0f;
    public float                            m_fBuffRange = 4.0f;

    private float                           m_fBuffTimer = 0f;
    private bool                            m_isDancing = false;

    protected override void Start()
    {
        base.Start();
        if (m_goblinAuraEffect != null) m_goblinAuraEffect.SetActive(false);
    }

    protected override void UpdateAction()
    {
        bool isBattleStarted = (FlockingManager.Instance != null) && FlockingManager.Instance.m_isBattleStarted;
        int iCurrentFloor = (GameDataManager.Instance != null) ? GameDataManager.Instance.m_iCurrentFloorIndex : 0;
        bool isMyFloor = (m_iFloorIndex == iCurrentFloor);

        if (isBattleStarted && isMyFloor)
        {
            UpdateGoblinSkill();
        }
    }

    void UpdateGoblinSkill()
    {
        m_fBuffTimer += Time.deltaTime;

        if (!m_isDancing)
        {
            if (m_fBuffTimer >= m_fBuffCoolTime)
            {
                m_isDancing = true;
                m_fBuffTimer = 0f;
            }
            ResetBuffs();
        }
        else
        {
            if (m_fBuffTimer >= m_fBuffDuration)
            {
                m_isDancing = false;
                m_fBuffTimer = 0f;
            }
            else
            {
                ApplyDefenseBuffToAllies();
            }
        }
        UpdateGoblinAnimation();
    }

    void UpdateGoblinAnimation()
    {
        if (m_animator == null) 
            return;

        m_animator.SetBool("IsDancing", m_isDancing);

        if (m_goblinAuraEffect != null)
        {
            var ps = m_goblinAuraEffect.GetComponent<ParticleSystem>();

            if (ps != null)
            {
                if (m_isDancing && !ps.isPlaying) 
                { 
                    m_goblinAuraEffect.SetActive(true); 
                    ps.Play(); 
                }
                else if (!m_isDancing && ps.isPlaying) 
                { 
                    ps.Stop(); 
                }
            }
        }
    }

    private void ApplyDefenseBuffToAllies()
    {
        if (FlockingManager.Instance == null) 
            return;

        foreach (var otherMon in FlockingManager.Instance.m_listMonsters)
        {
            if (otherMon != this && otherMon.m_iFloorIndex == this.m_iFloorIndex)
            {
                float fDistance = Vector2.Distance(transform.position, otherMon.transform.position);

                otherMon.m_isBuffed = (fDistance <= m_fBuffRange);
            }
        }
    }

    private void ResetBuffs()
    {
        if (FlockingManager.Instance == null) 
            return;

        foreach (var mon in FlockingManager.Instance.m_listMonsters)
        {
            if (mon.m_iFloorIndex == this.m_iFloorIndex) 
                mon.m_isBuffed = false;
        }
    }
}