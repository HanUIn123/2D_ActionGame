using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [Header("슬롯 설정")]
    public int                      m_iSlotIndex;
    public float                    m_fWeaponPower;
    public Sprite                   m_weaponSprite;
    public Image                    m_slotImage; 

    private InventoryManager        m_invenManager;

    private void Start()
    {
        m_invenManager = FindFirstObjectByType<InventoryManager>();

        if (null != GameDataManager.Instance)
        {
            if (m_iSlotIndex < GameDataManager.Instance.m_listInvenData.Count)
            {
                var data = GameDataManager.Instance.m_listInvenData[m_iSlotIndex];

                m_fWeaponPower = data.fPower;
                m_weaponSprite = data.sprIcon;
            }
        }

        if (m_slotImage != null)
            m_slotImage.sprite = m_weaponSprite;
    }

    public void OnClickThisSlot()
    {
        if (PlayerStats.Instance != null)
        {
            Debug.Log("<color=green>[인벤토리]</color> 슈발!");

            // 손에 든 거랑 슬롯 거랑 바꿈
            var oldWeapon = PlayerStats.Instance.SwapWeapon(m_fWeaponPower, m_weaponSprite);

            // 이 슬롯에는 이전들고잇던걸로 넣기
            m_fWeaponPower = oldWeapon.Item1;
            m_weaponSprite = oldWeapon.Item2;

            if (null != GameDataManager.Instance)
            {
                GameDataManager.Instance.UpdateInventoryData(m_iSlotIndex, oldWeapon.Item1, oldWeapon.Item2);
            }

            // 무기 스왑 후 이미지 바꿈
            if (m_slotImage != null) 
                m_slotImage.sprite = m_weaponSprite;

            // 상단바 UI 갱신 (인벤토리 매니저 통해서)
            m_invenManager.m_DisplayStats.UpdateStatUI();
        }
    }
}