using UnityEngine;

public class MonsterSkull : MonsterParent
{
    protected override void UpdateAction()
    {
        bool isBattleStarted = (FlockingManager.Instance != null) && FlockingManager.Instance.m_isBattleStarted;

        int iCurrentFloor = (GameDataManager.Instance != null) ? GameDataManager.Instance.m_iCurrentFloorIndex : 0;

        bool isMyFloor = (m_iFloorIndex == iCurrentFloor);

        if (m_animator != null)
            m_animator.SetBool("IsSkullAttacking", isBattleStarted && isMyFloor);
    }
}