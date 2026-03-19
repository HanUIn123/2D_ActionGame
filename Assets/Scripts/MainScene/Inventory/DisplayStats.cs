using UnityEngine;
using TMPro;

public class DisplayStats : MonoBehaviour
{
    public TextMeshProUGUI          m_txtAttack;

    private void OnEnable()
    {
        UpdateStatUI();
    }

    // 장착 시 호출 ( 아템 슬롯 버튼 의 : SelectWeapon 에서 ) 
    public void UpdateStatUI()
    {
        if (PlayerStats.Instance == null) 
            return;

        m_txtAttack.text = $"현재 공격력 : {PlayerStats.Instance.TotalAtk}";
    }
}
