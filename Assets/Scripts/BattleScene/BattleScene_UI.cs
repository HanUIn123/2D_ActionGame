using UnityEngine;
using TMPro;
using UnityEngine.UI; // Image 쓰려면 이거 추가해야 함!
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic; // List 쓰려면 추가!

public class BattleScene_UI : MonoBehaviour
{
    [SerializeField] private TMP_Text m_textFloorTitle;
    [SerializeField] private TMP_Text m_textMonsterCount;

    [Header("보스 등장 연출 설정")]
    [SerializeField] private GameObject m_bossAsertObject;  // BossAsert 이미지 오브젝트
    [SerializeField] private float m_fFlashSpeed = 0.2f;    // 깜빡임 속도
    [SerializeField] private int m_iFlashCount = 3;         // 깜빡임 횟수

    [Header("패배 연출 설정")]
    [SerializeField] private GameObject m_defeatObject;     // 패배 자막이나 이미지 오브젝트

    [Header("라이프 설정")]
    [SerializeField] private List<Image> m_listHearts;      // 하트 이미지 3개 연결 012

    void Start()
    {
        UpdateFloorText();

        UpdateLifeUI(); 

        if (m_bossAsertObject != null) 
            m_bossAsertObject.SetActive(false);
    }

    public void UpdateLifeUI()
    {
        if (PlayerStats.Instance == null || m_listHearts == null) 
            return;

        // 현재 플레이어의 목숨 값 (3, 2, 1, 0)
        int iCurrentLife = PlayerStats.Instance.m_iLifeCount;

        for (int i = 0; i < m_listHearts.Count; i++)
        {
            // 목숨이 3개면 i=0,1,2 모두 True
            // 목숨이 2개면 i=0,1은 True, i=2는 False (오른쪽 하트 사라짐)
            if (i < iCurrentLife)
            {
                m_listHearts[i].gameObject.SetActive(true);
            }
            else
            {
                m_listHearts[i].gameObject.SetActive(false);
            }
        }
    }

    public void UpdateFloorText()
    {
        if (GameDataManager.Instance != null && m_textFloorTitle != null)
        {
            var floorInfo = GameDataManager.Instance.GetCurrentFloorInfo();

            m_textFloorTitle.text = floorInfo.m_strFloorName;
        }
    }

    public void UpdateMonsterCount(int alive, int total)
    {
        if (m_textMonsterCount != null)
        {
            m_textMonsterCount.text = $"{alive} / {total}";
        }
    }

    public void ShowBossAppearance()
    {
        if (m_bossAsertObject != null)
        {
            StopAllCoroutines();

            StartCoroutine(BossAppearanceRoutine());
        }
    }

    private IEnumerator BossAppearanceRoutine()
    {
        // 깜빡빡
        for (int i = 0; i < m_iFlashCount; i++)
        {
            m_bossAsertObject.SetActive(true);
            yield return new WaitForSeconds(m_fFlashSpeed);

            m_bossAsertObject.SetActive(false);
            yield return new WaitForSeconds(m_fFlashSpeed);
        }

        // 1.0초 보여주고 
        m_bossAsertObject.SetActive(true);
        yield return new WaitForSeconds(1.0f);

        m_bossAsertObject.SetActive(false);
    }


    // 패배 시 실행될 함수
    public void ShowGameOver()
    {
        StopAllCoroutines(); // 혹시 돌고 있을 보스 연출 중단

        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        if (m_defeatObject == null) 
            yield break;

        // 패배 오브젝트를 활성화하고 위치를 위로 옮김
        RectTransform rect = m_defeatObject.GetComponent<RectTransform>();

        CanvasGroup canvasGroup = m_defeatObject.GetComponent<CanvasGroup>();

        Vector2 v2StartPos = new Vector2(0, 800); // 시작 위치 (화면 위쪽)
        Vector2 v2TargetPos = Vector2.zero;       // 목표 위치 (정중앙)

        rect.anchoredPosition = v2StartPos;

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;    // 처음엔 투명하게

        m_defeatObject.SetActive(true);

        // 연출: 위에서 아래로 스르륵 떨어짐
        float fElapsed = 0.0f;
        float fDuration = 0.8f; // 떨어지는 시간

        while (fElapsed < fDuration)
        {
            fElapsed += Time.deltaTime;
            float fProgress = fElapsed / fDuration;

            float fCurve = Mathf.Sin(fProgress * Mathf.PI * 0.5f);

            rect.anchoredPosition = Vector2.Lerp(v2StartPos, v2TargetPos, fCurve);

            if (canvasGroup != null) 
                canvasGroup.alpha = fProgress; // 점점 진하게

            yield return null;
        }

        // 1.5ch rlek렷다가,
        yield return new WaitForSeconds(1.5f);

        // 메인 씬 이동
        OnClickBackToMain();
    }

    public void OnClickBackToMain()
    {
        SceneManager.LoadScene("MainScene");
    }
}