using UnityEngine;

public class BossAnimationEvent : MonoBehaviour
{
    private BossSkull m_parent;

    void Start()
    {
        m_parent = GetComponentInParent<BossSkull>();
    }

    public void FireSlashEvent()
    {
        if (m_parent != null) 
            m_parent.FireSlashEvent();
    }

    public void EndFireEvent()
    {
        if (m_parent != null) 
            m_parent.EndFireEvent();
    }
}
