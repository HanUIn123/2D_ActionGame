using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class MonsterParent : MonoBehaviour
{
    [Header("Base Stats")]
    public float                    m_fHP = 50f;
    public float                    m_fMoveSpeed = 2.0f;
    public float                    m_fPushForce = 5.0f;
    public int                      m_iFloorIndex;

    [Header("사망 연출")]
    public GameObject               m_droppedHeadPrefab;

    protected Rigidbody2D           m_rb2D;
    protected Animator              m_animator;
    protected bool                  m_isDead = false;
    public Vector2                  m_v2FlockVelocity;

    [Header("드랍 재화 설정")]
    public int                      m_iDropGold = 50;
    public int                      m_iDropDiamond = 1;

    [Header("대열 유지용")]
    public float                    m_fRelativeX;

    [HideInInspector]
    public bool                     m_isBuffed = false; // 고블린 몹 버프 

    [Header("피격 연출용")]
    private List<SpriteRenderer>    m_listRenderers = new List<SpriteRenderer>();
    private Coroutine               m_flashRoutine;

    protected virtual void Awake()
    {
        m_rb2D = GetComponent<Rigidbody2D>();
        m_animator = GetComponentInChildren<Animator>();

        // 자식들의 SpriteRenderer가 있는 부품들을 싹 찾음.
        SpriteRenderer[] arrChildRenderers = GetComponentsInChildren<SpriteRenderer>();

        if (null != arrChildRenderers)
        {
            for (int i = 0; i < arrChildRenderers.Length; ++i)
            {
                // 그림자는 제외
                if (arrChildRenderers[i].gameObject.name.ToLower().Contains("shadow"))
                    continue;

                m_listRenderers.Add(arrChildRenderers[i]);
            }
        }
    }

    protected virtual void Start()
    {
        transform.localScale = new Vector3(-1, 1, 1);

        if (FlockingManager.Instance != null)
            FlockingManager.Instance.m_listMonsters.Add(this);
    }

    public void SetRelativeX()
    {
        m_fRelativeX = transform.position.x;
    }

    protected virtual void Update()
    {
        if (m_isDead)
            return;

        bool isBattleStarted = (FlockingManager.Instance != null) && FlockingManager.Instance.m_isBattleStarted;

        int iCurrentFloor = (GameDataManager.Instance != null) ? GameDataManager.Instance.m_iCurrentFloorIndex : 0;

        bool isMyFloor = (m_iFloorIndex == iCurrentFloor);

        UpdateAction();
    }

    protected abstract void UpdateAction();

    public virtual void TakeDamage(float _fDamage)
    {
        if (m_isDead)
            return;

        if (CameraShake.Instance != null)
            CameraShake.Instance.Shake(0.1f, 0.05f);

        float fFinalDamage = m_isBuffed ? _fDamage * 0.5f : _fDamage;

        m_fHP -= fFinalDamage;

        if (BattleEffectManager.Instance != null)
        {
            BattleEffectManager.Instance.ShowDamageText(transform.position, fFinalDamage);
        }

        if (m_flashRoutine != null)
            StopCoroutine(m_flashRoutine);

        m_flashRoutine = StartCoroutine(ColorFlashRoutine());

        if (m_rb2D != null)
            m_rb2D.AddForce(Vector2.right * 2f, ForceMode2D.Impulse);

        if (m_fHP <= 0)
            Die();
    }

    private IEnumerator ColorFlashRoutine()
    {
        Color flashColor = new Color(1f, 0.4f, 0.4f, 1f);

        for (int i = 0; i < m_listRenderers.Count; ++i)
        {
            m_listRenderers[i].color = flashColor;
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < m_listRenderers.Count; ++i)
        {
            m_listRenderers[i].color = Color.white;
        }
    }

    protected virtual void Die()
    {
        if (m_isDead)
            return;

        m_isDead = true;

        // 몬스터 죽을 때, 재화 정보 넘김. 
        if (BattleEffectManager.Instance != null)
        {
            BattleEffectManager.Instance.PlayCoinEffect(transform.position, m_iDropGold, m_iDropDiamond);
        }

        if (CameraShake.Instance != null) 
            CameraShake.Instance.Shake(0.2f, 0.2f);

        // 머리 or 몬스터마다의 드롭아이템 날리기 
        if (m_droppedHeadPrefab != null)
        {
            GameObject ObjHead = Instantiate(m_droppedHeadPrefab, transform.position, Quaternion.identity);

            Rigidbody2D HeadRb = ObjHead.GetComponent<Rigidbody2D>();

            if (HeadRb != null)
            {
                HeadRb.AddForce(new Vector2(Random.Range(-2f, 2f), 5f), ForceMode2D.Impulse);

                HeadRb.AddTorque(Random.Range(-5f, 5f), ForceMode2D.Impulse);
            }

            Destroy(ObjHead, 5f);
        }

        MonsterSpawner monsterSpawner= FindFirstObjectByType<MonsterSpawner>();

        if (monsterSpawner != null)
            monsterSpawner.OnMonsterDestroyed();

        Destroy(gameObject);
    }

    // 플레이어 밀쳐내기 공통 로직
    protected void OnCollisionStay2D(Collision2D _collision2D)
    {
        if (_collision2D.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = _collision2D.gameObject.GetComponent<Rigidbody2D>();

            if (playerRb != null)
            {
                playerRb.linearVelocity = new Vector2(-m_fPushForce, playerRb.linearVelocity.y);

                if (CameraShake.Instance != null) 
                    CameraShake.Instance.Shake(0.05f, 0.03f);
            }
        }
    }

    protected void OnDestroy()
    {
        if (FlockingManager.Instance != null)
            FlockingManager.Instance.m_listMonsters.Remove(this);
    }
}