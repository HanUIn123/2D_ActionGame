using UnityEngine;

public class PlayerBladeSkill : MonoBehaviour
{
    [Header("검기 리소스 세팅")]
    public SpriteRenderer       m_spriteRenderer;

    [Header("스킬 스탯")]
    public float                m_fSpeed = 20f; 
    public float                m_fDamage = 150f;
    public float                m_fLifeTime = 2f;

    void Start()
    {
        Destroy(gameObject, m_fLifeTime);
    }

    void Update()
    {
        transform.Translate(Vector2.right * m_fSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            MonsterParent monsterParent = collision.GetComponent<MonsterParent>();

            if (monsterParent != null)
            {
                monsterParent.TakeDamage(m_fDamage);
            }
        }
    }
}
