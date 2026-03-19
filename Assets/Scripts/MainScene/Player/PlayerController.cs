using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator                                    m_animator;
    private PlayerCombat                                m_combat;

    [Header("대쉬 설정 (수치 고정)")]
    public float                                        m_fDashDistanceOffset = 1.0f; 
    public float                                        m_fDashSearchRange = 15.0f;   
    public float                                        m_fDashEffectInterval = 0.02f;

    private bool                                        m_isDashing = false;
    private bool                                        m_isDefending = false;
    private bool                                        m_isAtkCollisionActive = false;

    [Header("스킬 1 ( 참격) 설정")]
    public GameObject                                   m_slashPrefab;                  // 만든 검기 프리팹 등록
    public UnityEngine.UI.Image                         m_skillButton1Image;            // 인스펙터에서 SkillButton1 이미지 연결
    public float                                        m_fSkill1Cooldown = 5.0f;       // 쿨타임 5초
    private float                                       m_fSkill1Timer = 0f;            // 남은 시간 계산용


    [Header("스킬 2 (힐) 설정")]
    public UnityEngine.UI.Image                         m_healButtonImage;              // 인스펙터에서 힐 버튼 이미지 연결
    public float                                        m_fHealCooldown = 15.0f;        
    private float                                       m_fHealTimer = 0f;              // 남은 시간 계산용
    public int                                          m_iMaxLife = 3;                 // 최대 생명 수치

    [Header("힐 이펙트 (자식 오브젝트)")]
    public ParticleSystem                               m_healParticle; 

    public float                                        m_fDeadLineX = -2.75f;          // 마지노선 X 좌표 
    private bool                                        m_isHurting= false;

    [Header("사망 연출용 시체 프리팹")]
    public GameObject                                   m_playerDeadBodyPrefab;         // 시체

    private void Start()
    {
        m_animator = GetComponentInChildren<Animator>();
        m_combat = GetComponent<PlayerCombat>();
        transform.localScale = Vector3.one;

        m_isHurting = false;
    }

    private void Update()
    {
        // 이미 게임 오버거나 처리 중이면 패스
        if (PlayerStats.Instance.m_isGameOver || m_isHurting) 
            return;

        // 스킬1 쿨타임 계산 
        if (m_fSkill1Timer > 0)
        {
            m_fSkill1Timer -= Time.deltaTime;

            // 버튼 밝기 조절 (0.3f 어두운 상태 -> 1.0f 원래 밝기)
            float fProgress = 1.0f - (m_fSkill1Timer / m_fSkill1Cooldown);

            float fColorValue = Mathf.Lerp(0.2f, 1.0f, fProgress);

            if (m_skillButton1Image != null)
                m_skillButton1Image.color = new Color(fColorValue, fColorValue, fColorValue, 1.0f);
        }

        // 스킬 2 쿨타임 계산 
        if (m_fHealTimer > 0)
        {
            m_fHealTimer -= Time.deltaTime;

            float fProgress = 1.0f - (m_fHealTimer / m_fHealCooldown);

            float fColorValue = Mathf.Lerp(0.1f, 1.0f, fProgress);

            if (m_healButtonImage != null)
                m_healButtonImage.color = new Color(fColorValue, fColorValue, fColorValue, 1.0f);
        }

        // 플레이어 위치가 마지노선보다 왼쪽으로 가면
        if (transform.position.x < m_fDeadLineX)
        {
            StartCoroutine(HandlePlayerPushBackOut());
        }

        // PC 전용 입력 (K, Space, D, 1, 2)
        HandlePCInput();

        // 플레이어 마지노선 닿을 때의 코루틴함수
        IEnumerator HandlePlayerPushBackOut()
        {
            m_isHurting = true;

            PlayerStats.Instance.DecreaseLife();

            // 몬스터 군집 뒤로 밀기 
            if (FlockingManager.Instance != null)
            {
                FlockingManager.Instance.EmergencyPushBack(2.2f);
            }

            if (PlayerStats.Instance.m_iLifeCount > 0)
            {
                // 플레이어 위치보정보간해서..
                Vector3 v3StartPos = transform.position;

                Vector3 v3TargetPos = new Vector3(-2.07f, transform.position.y, 0); 

                float fElapsed = 0f;
                float fDuration = 0.5f; // 0.5초 동안 스르륵 복귀

                while (fElapsed < fDuration)
                {
                    fElapsed += Time.deltaTime;
                    transform.position = Vector3.Lerp(v3StartPos, v3TargetPos, fElapsed / fDuration);
                    yield return null;
                }

                transform.position = v3TargetPos;
                yield return new WaitForSeconds(0.5f); // 추가 정비 시간
                m_isHurting = false;
            }
            else // 생카, 3개에서 다 떨어지면.. 진짜죽은거
            {
                // 시체 objDeadBodyprefab 이거 생성
                if (BattleEffectManager.Instance != null)
                {
                    BattleEffectManager.Instance.PlayerDeathEffect(transform.position, m_playerDeadBodyPrefab);
                }

                // gameobject의 자식것들 기능 싹끔 ( 플레이어 <- 이안에 부품조립된 형식이라 이렇게 해줘야 하더라.. ) 
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(false);
                }

                // collider 도 끔
                Collider2D collider = GetComponent<Collider2D>();

                if (collider != null)
                    collider.enabled = false;

                yield return new WaitForSecondsRealtime(2.0f);

                // 2초후 게임 오버 UI 
                BattleScene_UI bsUI = FindFirstObjectByType<BattleScene_UI>();

                if (bsUI != null)
                    bsUI.ShowGameOver();
            }
        }

        //// 임시로 살려놓는중.테스트용
        //if (Input.GetKeyDown(KeyCode.Q)) 
        //    OnSkill1ButtonClick();

        //if (Input.GetKeyDown(KeyCode.D)) 
        //    OnDefenseButtonClick();

        //if (Input.GetKeyDown(KeyCode.Space)) 
        //    OnDashButtonClick();

        //if (Input.GetMouseButtonDown(0)) 
        //    m_animator.SetBool("IsAttacking", true);

        //if (Input.GetMouseButtonUp(0))
        //    m_animator.SetBool("IsAttacking", false);

        if (!m_isDashing && m_isAtkCollisionActive)
            m_combat.CheckRealtimeCollision();
    }

    // [PC 전용 입력 함수]
    private void HandlePCInput()
    {
        // 공격: K키 누르는 동안 공격 애니메이션 활성화
        if (Input.GetKeyDown(KeyCode.K))
        {
            m_animator.SetBool("IsAttacking", true);
        }

        if (Input.GetKeyUp(KeyCode.K))
        {
            m_animator.SetBool("IsAttacking", false);
        }

        // 대쉬: 스페이스바
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnDashButtonClick();
        }

        // 디펜스: D키
        if (Input.GetKeyDown(KeyCode.D))
        {
            OnDefenseButtonClick();
        }

        // 스킬 1 (참격): 숫자 1키
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            OnSkill1ButtonClick();
        }

        // 스킬 2 (힐): 숫자 2키
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            OnHealButtonClick();
        }
    }

    // 공격 버튼 눌렀을 때 (Pointer Down)
    public void OnPointerDownAttack()
    {
        if (null != m_animator)
        {
            m_animator.SetBool("IsAttacking", true);
        }
    }

    // 공격 버튼 뗐을 때 (Pointer Up)
    public void OnPointerUpAttack()
    {
        if (null != m_animator)
        {
            m_animator.SetBool("IsAttacking", false);
        }
    }

    // 물리적 위치 고정 수행 
    private void LateUpdate()
    {
        if (m_isHurting) 
            return;

        if (transform.position.x < m_fDeadLineX)
        {
            transform.position = new Vector3(m_fDeadLineX, transform.position.y, transform.position.z);

            Rigidbody2D rb = GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }
    }

    // 스킬 1 참격 소환
    void ExecuteExcalibur()
    {
        if (m_slashPrefab != null)
        {
            Instantiate(m_slashPrefab, new Vector3(transform.position.x, transform.position.y + 0.8f, 0), Quaternion.identity);

            if (CameraShake.Instance != null)
                CameraShake.Instance.Shake(0.1f, 0.2f);
        }
    }

    public void OnDashButtonClick() 
    { 
        if (!m_isDashing) 
            StartCoroutine(DashToClosestEnemy()); 
    }

    private IEnumerator DashToClosestEnemy()
    {
        Collider2D[] arrEnemiesCollider = Physics2D.OverlapCircleAll(transform.position, m_fDashSearchRange, m_combat.m_enemyLayer);

        Transform trTargetEnemy = null; float closestDist = float.MaxValue;

        foreach (var col in arrEnemiesCollider)
        {
            float fDist = Vector2.Distance(transform.position, col.transform.position);

            if (fDist < closestDist) 
            { 
                closestDist = fDist; 
                trTargetEnemy = col.transform; 
            }
        }

        if (trTargetEnemy == null) 
            yield break;

        m_isDashing = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null) 
            rb.linearVelocity = Vector2.zero;

        Vector3 v3DashTarget = new Vector3(trTargetEnemy.position.x - m_fDashDistanceOffset, transform.position.y, 0);

        float fElapsed = 0f; 
        float fGhostTimer = 0f; 
        float fDashDuration = 0.1f;

        Vector3 v3StartPos = transform.position;

        while (fElapsed < fDashDuration)
        {
            fElapsed += Time.deltaTime; fGhostTimer += Time.deltaTime;

            transform.position = Vector3.Lerp(v3StartPos, v3DashTarget, fElapsed / fDashDuration);

            if (fGhostTimer >= m_fDashEffectInterval) 
            { 
                PlayerEffects.Instance.CreateGhost(m_animator);
                fGhostTimer = 0f; 
            }

            yield return null;
        }

        transform.position = v3DashTarget;

        m_isDashing = false;
    }

    public void OnDefenseButtonClick()
    {
        if (m_isDefending || !m_combat.IsEnemyNear()) 
            return;

        m_isDefending = true;

        // defense 버튼 누르면 히트스탑 잠깐 해준다.
        StartCoroutine(PlayerEffects.Instance.DefenseImpactRoutine(0.15f, () => {
            m_combat.PushEnemies();
            m_isDefending = false;
        }));

        if (CameraShake.Instance != null) 
            CameraShake.Instance.Shake(0.15f, 0.1f);
    }

    public void OnSkill1ButtonClick()
    {
        // 쿨 타임 0 이하 되면.
        if (m_fSkill1Timer <= 0 && !m_isHurting)
        {
            ExecuteExcalibur();

            m_fSkill1Timer = m_fSkill1Cooldown;
        }
    }

    public void OnHealButtonClick()
    {
        if (m_fHealTimer <= 0 && PlayerStats.Instance.m_iLifeCount < m_iMaxLife && !m_isHurting)
        {
            ExecuteHeal();

            m_fHealTimer = m_fHealCooldown; 
        }
    }

    void ExecuteHeal()
    {
        PlayerStats.Instance.m_iLifeCount++;

        BattleScene_UI bsUI = FindFirstObjectByType<BattleScene_UI>();

        if (bsUI != null)
            bsUI.UpdateLifeUI();

        if (m_healParticle != null)
        {
            m_healParticle.Play(); 
        }
    }

    // 애니메이션 이벤트용
    public void StartAtkCollision() 
    { 
        m_isAtkCollisionActive = true; 

        m_combat.StartAtk(); 
    }

    public void EndAtkCollision() => m_isAtkCollisionActive = false;

    // 층 이동 로직 그대로 유지 ( 끝나면 오른쪽으로 나갓다가 다음층의 왼쪽으로 다시 나타남 ) 
    public IEnumerator ExitToRight() 
    { 
        m_isDashing = false; 

        m_animator.SetBool("IsAttacking", false); 

        float fExitX = 12.0f; 

        while (transform.position.x < fExitX)
        { 
            transform.position += Vector3.right * 10.0f * Time.deltaTime; 

            yield return null;
        } 
    }

    public IEnumerator EnterFromLeft() 
    {
        transform.position = new Vector3(-12.0f, transform.position.y, 0);
        
        float fBattleStartPointX = -2.07f;

        while (transform.position.x < fBattleStartPointX) 
        { 
            transform.position += Vector3.right * 8.0f * Time.deltaTime; 
            yield return null; 
        } 

        transform.position = new Vector3(fBattleStartPointX, transform.position.y, 0); }
}
