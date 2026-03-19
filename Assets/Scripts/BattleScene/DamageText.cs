using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float        m_fMoveSpeed = 2f;    // 위로 솟구치는 속도
    public float        m_fAlphaSpeed = 2f;   // 사라지는 속도
    public float        m_fDestroyTime = 1f;  // 파괴 시간
    private TMP_Text    m_text;

    void Awake()
    {
        m_text = GetComponent<TMP_Text>();

        Destroy(gameObject, m_fDestroyTime); // 1초 뒤 자동 삭제
    }

    void Update()
    {
        transform.Translate(Vector3.up * m_fMoveSpeed * Time.deltaTime);
    }

    // 외부에서 데미지 수치를 넣어주는 함수
    public void SetDamage(float _fDamage)
    {
        if (m_text == null) 
            m_text = GetComponent<TMP_Text>();

        m_text.text = Mathf.FloorToInt(_fDamage).ToString();
    }
}