using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainScene_UI : MonoBehaviour
{
    [Header("메인씬 UI 버튼들")]
    [SerializeField] private Button             m_btnWorldMap;              // 월드 맵 버튼 (Center_Panel 내 위치)
    [SerializeField] private Button             m_btnInventory;             // 가방 버튼 (Bottom_Panel 내 위치)
    [SerializeField] private Button             m_btnItemBox;               // 보물상자 버튼 (Bottom_Panel 내 위치)

    [Header("메인씬 재화 판넬 관련 UI")]
    [SerializeField] private TMP_Text           m_textGoldPoint;
    [SerializeField] private TMP_Text           m_textDiaPoint;

    [Header("스테이지 선택 판넬 관련")]
    [SerializeField] private GameObject         m_objStageSelectPanel;      // 빨간 영역 판넬
    [SerializeField] private Button             m_btnClosePanel;            // 판넬 닫기 버튼 (있다면)
    [SerializeField] private Button[]           m_btnStageButtons;          // 11개 층수 버튼들


    [Header("준비 중 팝업 설정")]
    [SerializeField] private GameObject         m_objPrepareNotice;

    private void Start()
    {
        if (m_btnWorldMap != null)
            m_btnWorldMap.onClick.AddListener(OnClickWorldMap);

        if (m_btnClosePanel != null)
            m_btnClosePanel.onClick.AddListener(() => m_objStageSelectPanel.SetActive(false));

        for (int i = 0; i < m_btnStageButtons.Length; i++)
        {
            int iFloorIndex = i; 

            if (m_btnStageButtons[i] != null)
            {
                m_btnStageButtons[i].onClick.AddListener(() => OnClickEnterStage(iFloorIndex));
            }
        }

        if (m_objStageSelectPanel != null) 
            m_objStageSelectPanel.SetActive(false);

        UpdateCurrencyUI();
    }

    public void UpdateCurrencyUI()
    {
        if (GameDataManager.Instance != null)
        {
            m_textGoldPoint.text = GameDataManager.Instance.m_iGold.ToString();

            m_textDiaPoint.text = GameDataManager.Instance.m_iDiamond.ToString();
        }
    }

    // 월드 맵 버튼 클릭 시 실행
    public void OnClickWorldMap()
    {
        // 타워스테이지 선택  ui 오픈 
        if (m_objStageSelectPanel != null)
        {
            m_objStageSelectPanel.SetActive(true);
        }
    }

    // 실제 층수 버튼 눌렀을 때 호출되는 함수
    private void OnClickEnterStage(int _iIndex)
    {
        if (0 != _iIndex)
        {
            if (null != m_objPrepareNotice)
            {
                m_objPrepareNotice.SetActive(true);
            }
            return;
        }

        // 인덱스 계산 (1구역 누르면 0번, 2구역 누르면 10번 인덱스가 되게 설정)
        int iTargetIndex = _iIndex * 10;

        if (GameDataManager.Instance != null)
        {
            // 글자를 int꼴로해서 전달
            GameDataManager.Instance.m_iCurrentFloorIndex = iTargetIndex;
        }

        // 전투 씬으로 이동
        SceneManager.LoadScene("BattleScene");
    }
}
