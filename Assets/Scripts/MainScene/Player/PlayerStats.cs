using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public float                            m_fBaseAtk = 10.0f;
    public float                            m_fWeaponAtk = 0.0f;
    public float                            TotalAtk => m_fBaseAtk + m_fWeaponAtk;

    [Header("생존 설정")]
    public int                              m_iLifeCount = 3;       // 기본 생명 3개
    public bool                             m_isGameOver = false;
    public Sprite                           m_SpriteCurrentWeapon;  // 현재 무기 기억해줌

    private void Awake()
    {
        if (Instance == null) 
        { 
            Instance = this; 
            DontDestroyOnLoad(gameObject);
        }
        else 
         Destroy(gameObject); 
    }

    // 무기 정보 갱신
    public void EquipWeapon(float _fPower, Sprite _SpriteNewWeapon)
    {
        m_fWeaponAtk = _fPower;
        m_SpriteCurrentWeapon = _SpriteNewWeapon;

        UpdatePlayerWeaponVisual();
    }

    // 지금 씬 플레이어가 들고있는 Weapon <- 얘가 플레이어 프리팹의 자식임.
    public void UpdatePlayerWeaponVisual()
    {
        GameObject objWeapon = GameObject.Find("Weapon");

        if (objWeapon != null)
        {
            SpriteRenderer spriteRenderer = objWeapon.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
                spriteRenderer.sprite = m_SpriteCurrentWeapon;
        }
    }

    // 무기를 교체하고, 이전에 들고 있던 정보를 반환함 ( C++ pair 같은 문법이라고 한다.. ) 
    public (float, Sprite) SwapWeapon(float _fNewPower, Sprite _spriteNew)
    {
        // 기존에 들고 있던 거 따로 저장
        float fOriginAtk = m_fWeaponAtk;
        Sprite spOriginWeapon = m_SpriteCurrentWeapon;

        // 새 데이터 교체
        m_fWeaponAtk = _fNewPower;
        m_SpriteCurrentWeapon = _spriteNew;

        UpdatePlayerWeaponVisual();

        return (fOriginAtk, spOriginWeapon);
    }

    // 생명 감소 함수
    public void DecreaseLife()
    {
        if (m_isGameOver) 
            return;

        m_iLifeCount--;

        BattleScene_UI bsUI = FindFirstObjectByType<BattleScene_UI>();

        if (bsUI != null)
            bsUI.UpdateLifeUI();

        if (m_iLifeCount <= 0)
        {
            m_isGameOver = true;
        }
    }

    // 재전투위한 플레이어 정보 초기화 시켜주는 함수
    public void ResetStatsForBattle()
    {
        m_iLifeCount = 3;      // 목숨 복구
        m_isGameOver = false;  // 게임오버 상태 해제

        BattleScene_UI bsUI = FindFirstObjectByType<BattleScene_UI>();

        if (null != bsUI)
        {
            bsUI.UpdateLifeUI();
        }
    }
}
