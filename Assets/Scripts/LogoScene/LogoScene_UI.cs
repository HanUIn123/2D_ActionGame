using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class LogoScene_UI : MonoBehaviour
{
    [Header("로고씬 UI")]
    [SerializeField] private CanvasGroup            CG_GameTitle;                   // 게임 타이틀 
    [SerializeField] private CanvasGroup            CG_GameStartButton;             // 게임 스타트 버튼

    [Header("효과 세팅값")]
    [SerializeField] private float                  m_fFadeDuration = 1.0f;       // 페이드 속도
    [SerializeField] private float                  m_fBlinkSpeed = 1.0f;         // 깜빡임 속도

    private void Start()
    {
        // 초기 투명도 설정
        CG_GameTitle.alpha = 0;
        CG_GameStartButton.alpha = 0;

        CG_GameStartButton.interactable = false;
        CG_GameStartButton.blocksRaycasts = false;

        StartCoroutine(StartLogoSequence());
    }

    private IEnumerator StartLogoSequence()
    {
        // 게임 제목 서서히 나타남
        yield return StartCoroutine(FadeCanvasGroup(CG_GameTitle, 0.0f, 1.0f, m_fFadeDuration));
        yield return new WaitForSeconds(1.0f);

        // 버튼 서서히 나타남
        yield return StartCoroutine(FadeCanvasGroup(CG_GameStartButton, 0.0f, 1.0f, m_fFadeDuration));

        // 버튼 연출이 끝났으므로 클릭 가능하게 변경
        CG_GameStartButton.interactable = true;
        CG_GameStartButton.blocksRaycasts = true;

        // 버튼 깜빡임 시작 (무한 루프)
        StartCoroutine(BlinkText(CG_GameStartButton));
    }

    // 깜빡거리는 효과 코루틴
    private IEnumerator BlinkText(CanvasGroup _canvasGroup)
    {
        while (true)
        {
            // 서서히 어두워짐
            yield return StartCoroutine(FadeCanvasGroup(_canvasGroup, 1f, 0.0f, m_fBlinkSpeed));

            // 서서히 밝아짐
            yield return StartCoroutine(FadeCanvasGroup(_canvasGroup, 0.0f, 1f, m_fBlinkSpeed));
        }
    }

    // 공용 페이드 함수 (수정 완료)
    private IEnumerator FadeCanvasGroup(CanvasGroup _canvasGroup, float _fStart, float _fEnd, float _fBlinkSpeed)
    {
        float fTimer = 0.0f;

        while (fTimer < _fBlinkSpeed)
        {
            fTimer += Time.deltaTime;

            _canvasGroup.alpha = Mathf.Lerp(_fStart, _fEnd, fTimer / _fBlinkSpeed);

            yield return null;
        }

        _canvasGroup.alpha = _fEnd;
    }

    // 버튼 눌렀을 때 실행될 함수
    public void OnClickStart()
    {
        // "MainScene"으로 전환
        SceneManager.LoadScene("MainScene");
    }
}
