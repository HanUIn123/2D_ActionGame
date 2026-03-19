using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class BossGoblin : MonsterParent
{
    [Header("보스 기믹 설정")]
    public bool         m_isGroggy = false;
    public float        m_fGroggyDuration = 2.0f;
    public Color        m_groggyColor = Color.gray;

    protected override void UpdateAction()
    {
        bool isBattleStarted = (FlockingManager.Instance != null) && FlockingManager.Instance.m_isBattleStarted;
        int iCurrentFloor = (null != GameDataManager.Instance) ? GameDataManager.Instance.m_iCurrentFloorIndex : 0;

        if (isBattleStarted && m_iFloorIndex == iCurrentFloor)
        {
            if (!m_isGroggy)
            {
                if (m_animator != null)
                {
                    m_animator.SetBool("IsRunning", true);
                    m_animator.SetBool("IsGroggy", false);
                }

                transform.Translate(Vector3.left * m_fMoveSpeed * Time.deltaTime);
            }
            else
            {
                if (m_animator != null)
                    m_animator.SetBool("IsRunning", false);
            }
        }
        else
        {
            if (m_animator != null) 
                m_animator.SetBool("IsRunning", false);
        }
    }

    public override void TakeDamage(float _fDamage)
    {
        if (m_isGroggy)
        {
            base.TakeDamage(_fDamage); 
        }
    }

    // playerCombat에서 이거 실행ㄱ 
    public void OnPushedByDefense()
    {
        if (!m_isGroggy && !m_isDead)
        {
            StartCoroutine(GroggyRoutine());
        }
    }

    private IEnumerator GroggyRoutine()
    {
        m_isGroggy = true;

        if (m_animator != null)
        {
            m_animator.SetBool("IsGroggy", true);
            m_animator.SetBool("IsRunning", false);
        }

        SpriteRenderer[] arrSpriteRenderer = GetComponentsInChildren<SpriteRenderer>();

        if(arrSpriteRenderer != null)
        {
            for(int i = 0; i < arrSpriteRenderer.Length; ++i)
            {
                arrSpriteRenderer[i].color = m_groggyColor;
            }
        }

        yield return new WaitForSeconds(m_fGroggyDuration);

        m_isGroggy = false;

        if (m_animator != null)
            m_animator.SetBool("IsGroggy", false);

        if (arrSpriteRenderer != null)
        {
            for (int i = 0; i < arrSpriteRenderer.Length; ++i)
            {
                arrSpriteRenderer[i].color = Color.white;
            }
        }
    }
}