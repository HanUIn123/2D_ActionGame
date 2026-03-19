using UnityEngine;
using TMPro;

public class ItemListPanel : MonoBehaviour
{
    public TextMeshProUGUI                      m_txtGold;
    public TextMeshProUGUI                      m_txtDiamond;

    private void Start()
    {
        if (GameDataManager.Instance != null)
        {
            UpdateCurrencyDisplay(GameDataManager.Instance.m_iGold, GameDataManager.Instance.m_iDiamond);
        }
    }

    // 데이터 매니저가 호출해줄 함수
    public void UpdateCurrencyDisplay(int _iGold, int _iDia)
    {
        m_txtGold.text = _iGold.ToString();
        m_txtDiamond.text = _iDia.ToString();
    }
}
