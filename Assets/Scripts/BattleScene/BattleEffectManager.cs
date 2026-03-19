using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleEffectManager : MonoBehaviour
{
    public static BattleEffectManager Instance;

    [Header("프리팹 설정")]
    public GameObject               m_objCoinPrefab; 

    [Header("참조 설정")]
    public Canvas                   m_uiCanvas;         
    public RectTransform            m_rcBag;             

    [Header("피니시 연출 설정")]
    public float                    m_fSlowTimeScale = 0.2f;    // 얼마나 느리게,
    public float                    m_fFinishDuration = 1.5f;   // 몇초동안?
    public float                    m_fTargetZoomSize = 3.5f;   // 줌 인 시 어느정도 사이즈로? 

    private float                   m_fOriginalZoomSize;
    private Camera                  m_mainCam;

    [Header("다음 층 안내 UI")]
    public GameObject               m_nextFloorUI; 

    [Header("데미지 텍스트 설정")]
    public GameObject               m_objDamageText; 

    private void Awake()
    {
        Instance = this;

        m_mainCam = Camera.main;

        m_fOriginalZoomSize = m_mainCam.orthographicSize;
    }

    // 몬스터 부모 클래스에서 Die 공통 함수로 호출
    public void PlayCoinEffect(Vector3 _v3MonsterWorldPos, int _iGoldAmount, int _iDiaAmount)
    {
        if (_iGoldAmount > 0)
            StartCoroutine(CoinEffectRoutine(_v3MonsterWorldPos, _iGoldAmount, true));

        if (_iDiaAmount > 0)
            StartCoroutine(CoinEffectRoutine(_v3MonsterWorldPos, _iDiaAmount, false));
    }

    private IEnumerator CoinEffectRoutine(Vector3 _v3WorldPos, int _iTotalAmount, bool _isGold)
    {
        int iVisualCount = 5;
        int iAmountPerCoin = _iTotalAmount / iVisualCount;
        int iRemainder = _iTotalAmount % iVisualCount;

        for (int i = 0; i < iVisualCount; i++)
        {
            GameObject objCoin = Instantiate(m_objCoinPrefab, m_uiCanvas.transform);

            RectTransform rcCoin= objCoin.GetComponent<RectTransform>();

            // Screen Space - Camera 전용 좌표 변환 ( 월드 좌표를 스크린 좌표로 먼저 바꿈 )
            Vector2 v2ScreenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, _v3WorldPos);

            Vector2 v2LocalPos;

            // 스크린 좌표를 캔버스(부모)의 로컬 좌표로 다시 변환!
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                m_uiCanvas.GetComponent<RectTransform>(),
                v2ScreenPoint,
                m_uiCanvas.worldCamera,
                out v2LocalPos
            );

            // 위치 적용
            rcCoin.anchoredPosition = v2LocalPos;
            rcCoin.localPosition = new Vector3(rcCoin.localPosition.x, rcCoin.localPosition.y, 0.0f); // Z축 고정
            rcCoin.localScale = Vector3.one;

            // 팡 터지는 연출
            Vector2 v2RandomPop = new Vector2(Random.Range(-80.0f, 80.0f), Random.Range(-80.0f, 80.0f));
            rcCoin.anchoredPosition += v2RandomPop;

            int iFinalValue = (i == iVisualCount - 1) ? iAmountPerCoin + iRemainder : iAmountPerCoin;

            StartCoroutine(MoveToBag(rcCoin, iFinalValue, _isGold));

            yield return new WaitForSeconds(0.03f);
        }
    }

    private IEnumerator MoveToBag(RectTransform _rcCoin, int _iValue, bool _isGold)
    {
        float fElapsed = 0.0f;
        float fDuration = 0.7f;

        // 절대 좌표인 position 사용
        Vector3 v3StartPos = _rcCoin.position;

        while (fElapsed < fDuration)
        {
            fElapsed += Time.deltaTime;

            float t = fElapsed / fDuration;

            float fCurve = t * t;

            // 가방으로 직접 이동
            _rcCoin.position = Vector3.Lerp(v3StartPos, m_rcBag.position, fCurve);

            _rcCoin.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.3f, fCurve);

            yield return null;
        }

        // 가방 정중앙에 도착한 후 데이터 갱신
        if (GameDataManager.Instance != null)
        {
            if (_isGold) 
                GameDataManager.Instance.UpdateGold(_iValue);
            else 
                GameDataManager.Instance.UpdateDiamond(_iValue);
        }

        StartCoroutine(BagPunchRoutine());

        Destroy(_rcCoin.gameObject);
    }

    // 가방 펌핑 들썩들썩 연출
    private IEnumerator BagPunchRoutine()
    {
        m_rcBag.localScale = Vector3.one * 1.2f;

        yield return new WaitForSeconds(0.05f);

        m_rcBag.localScale = Vector3.one;
    }

    public void PlayFinishSlowMotion()
    {
        StartCoroutine(FinishSlowRoutine());
    }

    // 스테이지 클리어시 나오는 연출 함수.
    private IEnumerator FinishSlowRoutine()
    {
        // 시간 느리게 & 줌인 (기존 로직)
        Time.timeScale = m_fSlowTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        float fElapsed = 0f;

        float fHalfDuration = m_fFinishDuration * 0.5f;

        while (fElapsed < fHalfDuration)
        {
            fElapsed += Time.unscaledDeltaTime;

            m_mainCam.orthographicSize = Mathf.Lerp(m_fOriginalZoomSize, m_fTargetZoomSize, fElapsed / fHalfDuration);

            yield return null;
        }

        // 잠시 멈춤고, GOGO 소환.
        if (m_nextFloorUI != null)
        {
            m_nextFloorUI.SetActive(true);
        }
        yield return new WaitForSecondsRealtime(0.5f);

        // 줌아웃, 시간 복구
        fElapsed = 0f;

        while (fElapsed < fHalfDuration)
        {
            fElapsed += Time.unscaledDeltaTime;

            m_mainCam.orthographicSize = Mathf.Lerp(m_fTargetZoomSize, m_fOriginalZoomSize, fElapsed / fHalfDuration);

            Time.timeScale = Mathf.Lerp(m_fSlowTimeScale, 1.0f, fElapsed / fHalfDuration);

            yield return null;
        }

        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f;
        m_mainCam.orthographicSize = m_fOriginalZoomSize;
    }

    public void HideNextFloorUI()
    {
        if (m_nextFloorUI != null)
            m_nextFloorUI.SetActive(false);
    }

    public void PlayerDeathEffect(Vector3 _v3PlayerPos, GameObject _objDeadBody)
    {
        // 플레이어 죽으면 죽음전용 프리팹 가져와서 생성
        GameObject objDeadBody = Instantiate(_objDeadBody, _v3PlayerPos, Quaternion.identity);

        // 자식 조각들(Head, Body 등) 하나하나에 힘 전달
        foreach (Transform part in objDeadBody.transform)
        {
            Rigidbody2D rb = part.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // 위 + 좌우 힘주기.
                float fForceX = Random.Range(0.0f, 5f);
                float fForceY = Random.Range(3.5f, -3.5f);

                rb.AddForce(new Vector2(fForceX, fForceY), ForceMode2D.Impulse);

                // 빙글빙글 회전 추가
                rb.AddTorque(Random.Range(-50f, 50f), ForceMode2D.Impulse);
            }

            Destroy(part.gameObject, 3f);
        }

        // 바구니 자체도 3초 뒤에 파괴
        Destroy(objDeadBody, 3.1f);
    }

    public void ShowDamageText(Vector3 _v3WorldPos, float _fDamage)
    {
        Vector3 v3HeadPos = _v3WorldPos + new Vector3(0, 0.8f, 0);

        GameObject objText = Instantiate(m_objDamageText, m_uiCanvas.transform);

        // 좌표 변환
        Vector2 v2ScreenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, v3HeadPos);

        Vector2 v2LocalPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            m_uiCanvas.GetComponent<RectTransform>(),
            v2ScreenPoint,
            m_uiCanvas.worldCamera,
            out v2LocalPos
        );

        RectTransform rc = objText.GetComponent<RectTransform>();

        rc.anchoredPosition = v2LocalPos;

        // 좌우 오프셋
        rc.anchoredPosition += new Vector2(Random.Range(-20f, 20f), 0);

        DamageText damageText = objText.GetComponent<DamageText>();

        if (damageText != null)
            damageText.SetDamage(_fDamage);

        //rc.localPosition = new Vector3(rc.localPosition.x, rc.localPosition.y, 0f);
    }
}