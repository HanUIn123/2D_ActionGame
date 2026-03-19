using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject                   m_objInventoryPanel; 
    public DisplayStats                 m_DisplayStats; 

    // 인벤토리 판넬 껏/켜
    public void OnBagButtonClick()
    {
        if (m_objInventoryPanel != null)
        {
            bool isActive = m_objInventoryPanel.activeSelf;

            m_objInventoryPanel.SetActive(!isActive);
        }
    }

    // 뒤버튼 으로 끔
    public void CloseInventory()
    {
        if (m_objInventoryPanel != null)
        {
            m_objInventoryPanel.SetActive(false);
        }
    }

    // 슬롯 버튼들 누르면 호출댐 
    public void SelectWeapon(float _fPower, Sprite _spriteWeapon)
    {
        PlayerStats.Instance.EquipWeapon(_fPower, _spriteWeapon);

        if (m_DisplayStats != null) 
            m_DisplayStats.UpdateStatUI();
    }
}
