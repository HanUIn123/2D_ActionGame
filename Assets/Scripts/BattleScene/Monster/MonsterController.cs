//using UnityEngine;

//public class MonsterController : MonoBehaviour
//{
//    public enum MonsterType { Skull, Goblin }
//    [Header("타입 설정")]
//    public MonsterType m_type = MonsterType.Skull;

//    [Header("능력치 설정")]
//    public float m_fHP = 50f;
//    public float m_fMoveSpeed = 2.0f;
//    public float m_fPushForce = 5.0f;

//    [Header("사망 연출")]
//    public GameObject m_droppedHeadPrefab;

//    [Header("고블린 연출/스킬 설정")]
//    public GameObject m_goblinAuraEffect;
//    public float m_fBuffCoolTime = 4.0f;    // 춤추는 주기 (7초마다)
//    public float m_fBuffDuration = 2.0f;    // 춤추는 시간 (3초 동안)
//    public float m_fBuffRange = 4.0f;       // 버프 범위

//    private float m_fBuffTimer = 0f;        // 쿨타임 계산용 타이머
//    private bool m_isDancing = false;       // 현재 춤추는 중인가?

//    [Header("군집 연산용")]
//    public int m_iFloorIndex;
//    public Vector2 m_vFlockVelocity;
//    private Rigidbody2D m_rb2D;
//    private Animator m_animator;

//    private bool m_isBuffed = false;        // 해골들이 버프 받았는지 체크용
//    private bool m_isDead = false;

//    void Start()
//    {
//        m_rb2D = GetComponent<Rigidbody2D>();
//        m_animator = GetComponentInChildren<Animator>();
//        transform.localScale = new Vector3(-1, 1, 1);

//        if (FlockingManager.Instance != null)
//            FlockingManager.Instance.m_monsters.Add(this);

//        if (m_goblinAuraEffect != null) m_goblinAuraEffect.SetActive(false);
//    }

//    void Update()
//    {
//        bool isBattleStarted = (FlockingManager.Instance != null) && FlockingManager.Instance.m_isBattleStarted;
//        int currentFloor = (GameDataManager.Instance != null) ? GameDataManager.Instance.m_iCurrentFloorIndex : 0;
//        bool isMyFloor = (m_iFloorIndex == currentFloor);

//        // 1. 물리 이동 (전투 중이고 내 층일 때만 이동)
//        if (m_rb2D != null)
//        {
//            m_rb2D.linearVelocity = (isBattleStarted && isMyFloor) ? m_vFlockVelocity : Vector2.zero;
//        }

//        // 2. 고블린 스킬 로직 (쿨타임 방식)
//        if (m_type == MonsterType.Goblin && isBattleStarted && isMyFloor)
//        {
//            UpdateGoblinSkill();
//        }

//        // 3. 애니메이션 업데이트
//        UpdateAnimations(isBattleStarted && isMyFloor);
//    }

//    void UpdateGoblinSkill()
//    {
//        m_fBuffTimer += Time.deltaTime;

//        if (!m_isDancing)
//        {
//            // 춤 안 추는 중일 때: 쿨타임 다 찼나 확인
//            if (m_fBuffTimer >= m_fBuffCoolTime)
//            {
//                m_isDancing = true;
//                m_fBuffTimer = 0f; // 타이머 리셋 (이제 지속시간 측정용으로 씀)
//            }
//            // 춤 안 출 때는 버프 해제
//            ResetBuffs();
//        }
//        else
//        {
//            // 춤추는 중일 때: 지속시간 다 됐나 확인
//            if (m_fBuffTimer >= m_fBuffDuration)
//            {
//                m_isDancing = false;
//                m_fBuffTimer = 0f; // 타이머 리셋 (다시 쿨타임 측정용)
//            }
//            else
//            {
//                // 춤추는 동안 버프 뿌리기
//                ApplyDefenseBuffToAllies();
//            }
//        }
//    }

//    void UpdateAnimations(bool _isActive)
//    {
//        if (m_animator == null) return;

//        if (m_type == MonsterType.Skull)
//        {
//            m_animator.SetBool("IsSkullAttacking", _isActive);
//        }
//        else if (m_type == MonsterType.Goblin)
//        {
//            // 고블린은 _isActive가 켜져 있어도 m_isDancing일 때만 춤춤!
//            bool shouldDance = _isActive && m_isDancing;
//            m_animator.SetBool("IsDancing", shouldDance);

//            // 파티클 제어
//            if (m_goblinAuraEffect != null)
//            {
//                var ps = m_goblinAuraEffect.GetComponent<ParticleSystem>();
//                if (ps != null)
//                {
//                    if (shouldDance && !ps.isPlaying) { m_goblinAuraEffect.SetActive(true); ps.Play(); }
//                    else if (!shouldDance && ps.isPlaying) { ps.Stop(); }
//                }
//            }
//        }
//    }


//    private void ApplyDefenseBuffToAllies()
//    {
//        if (FlockingManager.Instance == null) return;
//        foreach (var otherMon in FlockingManager.Instance.m_monsters)
//        {
//            if (otherMon != this && otherMon.m_iFloorIndex == this.m_iFloorIndex)
//            {
//                float distance = Vector2.Distance(transform.position, otherMon.transform.position);
//                otherMon.m_isBuffed = (distance <= m_fBuffRange);
//            }
//        }
//    }

//    private void ResetBuffs()
//    {
//        if (FlockingManager.Instance == null) return;
//        foreach (var mon in FlockingManager.Instance.m_monsters)
//        {
//            if (mon.m_iFloorIndex == this.m_iFloorIndex) mon.m_isBuffed = false;
//        }
//    }

//    // [다시 추가] 몬스터와 플레이어가 충돌 중일 때 실행됨
//    private void OnCollisionStay2D(Collision2D collision)
//    {
//        if (collision.gameObject.CompareTag("Player"))
//        {
//            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
//            if (playerRb != null)
//            {
//                // 1. 플레이어를 왼쪽(-X)으로 밀어냄
//                playerRb.linearVelocity = new Vector2(-m_fPushForce, playerRb.linearVelocity.y);

//                // 2. [연출] 플레이어가 밀릴 때 아주 미세한 카메라 진동 (0.05초 동안 0.03 강도)
//                if (CameraShake.Instance != null)
//                    CameraShake.Instance.Shake(0.05f, 0.03f);
//            }
//        }
//    }

//    public void TakeDamage(float _damage)
//    {
//        if (m_isDead) return;

//        // [추가] 몬스터가 맞을 때 잔잔한 카메라 쉐이크 (0.1초 동안 0.05 강도)
//        if (CameraShake.Instance != null)
//            CameraShake.Instance.Shake(0.1f, 0.05f);


//        float finalDamage = m_isBuffed ? _damage * 0.5f : _damage;
//        m_fHP -= finalDamage;
//        if (m_rb2D != null) m_rb2D.AddForce(Vector2.right * 2f, ForceMode2D.Impulse);
//        if (m_fHP <= 0) Die();
//    }

//    private void Die()
//    {
//        if (m_isDead) return;
//        m_isDead = true;

//        // [추가] 몬스터 처치 시 강한 카메라 쉐이크 (0.2초 동안 0.2 강도)
//        if (CameraShake.Instance != null)
//            CameraShake.Instance.Shake(0.2f, 0.2f);

//        ResetBuffs(); // 죽을 때 버프 끄기

//        if (m_droppedHeadPrefab != null)
//        {
//            GameObject head = Instantiate(m_droppedHeadPrefab, transform.position, Quaternion.identity);
//            Rigidbody2D headRb = head.GetComponent<Rigidbody2D>();
//            if (headRb != null)
//            {
//                headRb.AddForce(new Vector2(Random.Range(-2f, 2f), 5f), ForceMode2D.Impulse);
//                headRb.AddTorque(Random.Range(-5f, 5f), ForceMode2D.Impulse);
//            }
//            Destroy(head, 5f);
//        }
//        MonsterSpawner spawner = FindFirstObjectByType<MonsterSpawner>();
//        if (spawner != null) spawner.OnMonsterDestroyed();
//        Destroy(gameObject);
//    }

//    private void OnDestroy()
//    {
//        if (FlockingManager.Instance != null && FlockingManager.Instance.m_monsters.Contains(this))
//            FlockingManager.Instance.m_monsters.Remove(this);
//    }
//}