using UnityEngine;

public class DashEffect : MonoBehaviour
{
    private SpriteRenderer[]                m_renderers;
    public float                            m_fDestroyTime = 0.3f;
    private float                           m_fTimer = 0f;

    void Start()
    {
        m_renderers = GetComponentsInChildren<SpriteRenderer>();

        Destroy(gameObject, m_fDestroyTime);
    }

    void Update()
    {
        m_fTimer += Time.deltaTime;

        float fAlpha = Mathf.Lerp(0.5f, 0f, m_fTimer / m_fDestroyTime);

        foreach (var sr in m_renderers)
        {
            if (sr != null)
            {
                Color c = sr.color;
                c.a = fAlpha;
                sr.color = c;
            }
        }
    }
}
