using UnityEngine;

public class BossSkullBullet : MonoBehaviour
{
    public float                                m_fSpeed = 10f;
    public float                                m_fDamage = 20f;
    public float                                m_fLifeTime = 3f; // 3초 뒤 자동 삭제

    void Start()
    {
        Destroy(gameObject, m_fLifeTime);
    }

    void Update()
    {
        transform.Translate(Vector2.left * m_fSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (CameraShake.Instance != null)
            {
                CameraShake.Instance.Shake(0.2f, 0.1f);
            }

            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();

            if (playerRb != null)
            {
                float m_fPushForce = 5f;

                playerRb.linearVelocity = new Vector2(-m_fPushForce, playerRb.linearVelocity.y);
            }

            Destroy(gameObject);
        }
    }
}
