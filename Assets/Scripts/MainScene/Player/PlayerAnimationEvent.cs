using UnityEngine;

// 부모에서 찾아옴. -> Player 오브젝트에서, 컨트롤러 찾와서 이벤트 function 연결위한 cs
public class PlayerAnimationEvent : MonoBehaviour
{
    private PlayerController m_playerController;

    void Start()
    {
        m_playerController = GetComponentInParent<PlayerController>();
    }

    public void StartAttackFrame() 
    { 
        m_playerController.StartAtkCollision(); 
    }

    public void EndAttackFrame() 
    { 
        m_playerController.EndAtkCollision(); 
    }
}
