using UnityEngine;

public class BackGroundScroller : MonoBehaviour
{
    [Header("설정 (인스펙터에서 확인한 값)")]
    public float                            m_fScrollSpeed = 5f;
    public float                            m_fTotalHeight = 10.05f;    // 10.03 - (-0.02) = 10.05
    public float                            m_fInitialY = -0.02f;       // 배경 1번의 시작 Y값

    public Transform[]                      m_backgrounds;

    private bool                            m_isScrolling = false;
    private float                           m_fTargetY = 0f;
    public bool                             IsArrived { get; private set; } = false;
    public bool                             IsScrolling => m_isScrolling;

    void Update()
    {
        if (m_isScrolling)
        {
            float fMoveStep = m_fScrollSpeed * Time.deltaTime;

            for (int i = 0; i < m_backgrounds.Length; i++)
            {
                m_backgrounds[i].position += Vector3.down * fMoveStep;

                // 이미지 하나가 완전히 화면 아래로 내려갔을 때 (기준점 - 전체높이)
                if (m_backgrounds[i].position.y <= m_fInitialY - m_fTotalHeight)
                {
                    // 위로 두 칸 점프 
                    m_backgrounds[i].position += Vector3.up * m_fTotalHeight * 2;
                }
            }

            if (m_backgrounds[0].position.y <= m_fTargetY)
            {
                float fOffset = m_backgrounds[0].position.y - m_fTargetY;

                foreach (var bg in m_backgrounds)
                {
                    bg.position = new Vector3(bg.position.x, bg.position.y - fOffset, bg.position.z);
                }

                IsArrived = true;
            }
        }
    }

    public void StartScroll()
    {
        IsArrived = false;
        m_isScrolling = true;

        // 한 층의 높이는 전체 이미지의 1/3
        float fOneFloorHeight = m_fTotalHeight / 3.0f;

        m_fTargetY = m_backgrounds[0].position.y - fOneFloorHeight;
    }

    public void StopScroll()
    {
        m_isScrolling = false;
        IsArrived = false;

        if (m_backgrounds.Length >= 2)
        {
            m_backgrounds[0].position = new Vector3(0, m_fInitialY, m_backgrounds[0].position.z);
            m_backgrounds[1].position = new Vector3(0, m_fInitialY + m_fTotalHeight, m_backgrounds[1].position.z);
        }

        MonsterSpawner monsterSpawner = FindFirstObjectByType<MonsterSpawner>();

        if (monsterSpawner != null)
            monsterSpawner.AlignMonstersToFloor();
    }
}