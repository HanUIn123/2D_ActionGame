using UnityEngine;
using System.Collections.Generic;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    [Header("타워 데이터 시트")]
    public TowerMasterData                              m_towerMasterData;

    [Header("플레이어 재화 데이터")]
    public int                                          m_iGold = 5000;      // 테스트용 기본 골드
    public int                                          m_iDiamond = 100;    // 테스트용 기본 다이아

    // UI를 찾아서 갱신해주기 위한 참조
    private ItemListPanel                               m_ItemListPanel;

    [System.Serializable]
    public struct InvenItemData
    {
        public float                                    fPower;      // 아이템 공격력
        public Sprite                                   sprIcon;     // 아이템 이미지
    }

    [Header("인벤토리 실시간 데이터")]
    public List<InvenItemData>                          m_listInvenData = new List<InvenItemData>();

    [Header("플레이어 스탯 데이터")]
    public int                                          m_iLevel = 1;
    public float                                        m_fHp = 100.0f;
    public float                                        m_fMaxHp = 100.0f;
    public float                                        m_fAttack = 10.0f;

    [Header("진행 정보")]
    public int                                          m_iCurrentFloorIndex = 0;
    public int                                          m_iClearFloor = 0;

    [Header("타워 밸런스 데이터")]
    public float                                        m_fWorldFloorHeight = 3.11f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 인벤토리 슬롯의 데이터 갱신
    public void UpdateInventoryData(int _iIndex, float _fPower, Sprite _sprite)
    {
        if (_iIndex >= 0 && _iIndex < m_listInvenData.Count)
        {
            InvenItemData data;
            data.fPower = _fPower;
            data.sprIcon = _sprite;

            m_listInvenData[_iIndex] = data;
        }
    }

    // 현재 층의 정보를 구조체 통째로 반환 
    public TowerMasterData.FloorInfo GetCurrentFloorInfo()
    {
        if (m_towerMasterData == null)
        {
            return default;
        }

        // 인덱스가 리스트 범위를 벗어나지 않았는지 체크
        if (m_iCurrentFloorIndex >= 0 && m_iCurrentFloorIndex < m_towerMasterData.m_listFloors.Count)
        {
            return m_towerMasterData.m_listFloors[m_iCurrentFloorIndex];
        }

        return default;
    }

    // 다음 층으로 넘어갈 때 호출할 함수
    public void IncreaseFloor()
    {
        // 데이터 리스트 개수보다 인덱스가 커지지 않게 막음
        if (m_iCurrentFloorIndex < m_towerMasterData.m_listFloors.Count - 1)
        {
            m_iCurrentFloorIndex++;
            m_iClearFloor++;
        }
        else
        {
            // 다음 로직.. 


        }
    }

    // 골드 추가/차감 공용 함수
    public void UpdateGold(int _nAmount)
    {
        m_iGold += _nAmount;

        RefreshUI();
    }

    // 다이아몬드 업데이트 (새로 추가)
    public void UpdateDiamond(int _nAmount)
    {
        m_iDiamond += _nAmount;

        RefreshUI();
    }

    // UI 갱신 헬퍼 함수
    private void RefreshUI()
    {
        // 씬에서 CurrencyUI를 찾아서 텍스트를 업데이트해줌
        if (m_ItemListPanel == null)
            m_ItemListPanel = FindFirstObjectByType<ItemListPanel>();

        if (m_ItemListPanel != null)
        {
            m_ItemListPanel.UpdateCurrencyDisplay(m_iGold, m_iDiamond);
        }
    }
}
