using System.Collections;
using UnityEngine;

public class BossBehaviour : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;

    [Header("Detección")]
    public float detectionRange = 10f;
    public float attackRange = 1.8f;
    public float chaseRunDistance = 5f;

    [Header("Movimiento")]
    public float walkSpeed = 2.5f;
    public float runSpeed = 5f;

    [Header("Patrulla")]
    public float patrolRadius = 3f;
    public float patrolChangeTime = 2f;

    [Header("Cooldowns")]
    public float pauseShort = 1f;
    public float pauseMedium = 1.5f;
    public float pauseLong = 2.5f;

    [Header("Fase 2")]
    [Range(0f, 1f)]
    public float phase2Threshold = 0.5f;
    public float phase2SpeedMultiplier = 1.35f;
    public float phase2CooldownMultiplier = 0.65f;

    private bool isPhase2 = false;
    private bool isFacingRight = true;
    private bool isActing = false;

    private Rigidbody2D rb;
    private Animator anim;
    private BossHealth health;
    private BossAttack bossAttack;

    private float patrolTimer = 0f;
    private float patrolDirection = 1f;
    private Vector2 patrolCenter;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        health = GetComponent<BossHealth>();
        bossAttack = GetComponent<BossAttack>();

        rb.freezeRotation = true;
        patrolCenter = transform.position;

        StartCoroutine(AttackPattern());
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            if (obj != null) player = obj.transform;
            else return;
        }

        if (IsPlayerDead())
        {
            StopMovement();
            return;
        }

        if (health != null && health.IsKnockedBack()) return;
        if (isActing) return;

        CheckPhaseTransition();

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > detectionRange)
        {
            Patrol();
        }
        else if (distance > attackRange)
        {
            FlipTowardsPlayer();
            Chase(distance);
        }
        else
        {
            FlipTowardsPlayer();
            StopMovement();
        }
    }

    // ================= ATAQUES =================

    IEnumerator AttackPattern()
    {
        while (true)
        {
            yield return new WaitUntil(() =>
                player != null &&
                Vector2.Distance(transform.position, player.position) <= attackRange &&
                !IsPlayerDead());

            if (isPhase2)
                yield return StartCoroutine(PatternPhase2());
            else
                yield return StartCoroutine(PatternPhase1());
        }
    }

    IEnumerator PatternPhase1()
    {
        yield return DoAttack(1);
        yield return new WaitForSeconds(pauseShort);

        if (PlayerInRange())
        {
            yield return DoAttack(1);
            yield return new WaitForSeconds(pauseShort);
        }

        yield return DoAttack(3);
        yield return new WaitForSeconds(pauseLong);
    }

    IEnumerator PatternPhase2()
    {
        yield return DoAttack(1);
        yield return new WaitForSeconds(pauseShort * phase2CooldownMultiplier);

        yield return DoAttack(3);
        yield return new WaitForSeconds(pauseShort * phase2CooldownMultiplier);

        if (PlayerInRange())
        {
            yield return DoAttack(1);
            yield return new WaitForSeconds(pauseShort * phase2CooldownMultiplier);
        }

        yield return DoAttack(4);
        yield return new WaitForSeconds(pauseMedium * phase2CooldownMultiplier);

        yield return DoAttack(3);
        yield return new WaitForSeconds(pauseLong * phase2CooldownMultiplier);
    }

    IEnumerator DoAttack(int index)
    {
        isActing = true;
        StopMovement();
        FlipTowardsPlayer();

        bossAttack.SetCurrentAttack(index);

        anim.ResetTrigger("Attack");
        anim.SetInteger("attackIndex", index);
        yield return null;
        anim.SetTrigger("Attack");

        float timer = 0f;
        float maxTime = 2f;

        yield return new WaitUntil(() =>
        {
            timer += Time.deltaTime;
            return isActing == false || timer >= maxTime;
        });

        anim.ResetTrigger("Attack");
        isActing = false;
    }

    // Animation Event - llamar al final de cada clip de ataque
    public void OnAttackEnd()
    {
        isActing = false;
    }

    // ================= MOVIMIENTO =================

    void Chase(float distance)
    {
        float speed = (distance > chaseRunDistance) ? runSpeed : walkSpeed;

        if (isPhase2)
            speed *= phase2SpeedMultiplier;

        float dir = (player.position.x > transform.position.x) ? 1f : -1f;

        rb.velocity = new Vector2(dir * speed, rb.velocity.y);
    }

    void StopMovement()
    {
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    void Patrol()
    {
        patrolTimer += Time.fixedDeltaTime;

        if (patrolTimer >= patrolChangeTime)
        {
            patrolTimer = 0f;
            patrolDirection = (Random.value > 0.5f) ? 1f : -1f;
        }

        float distFromCenter = transform.position.x - patrolCenter.x;

        if (Mathf.Abs(distFromCenter) >= patrolRadius)
            patrolDirection = -Mathf.Sign(distFromCenter);

        rb.velocity = new Vector2(patrolDirection * walkSpeed * 0.6f, rb.velocity.y);

        // Voltear hacia la dirección en la que camina
        FlipToDirection(patrolDirection);
    }

    // ================= FLIP =================

    void FlipTowardsPlayer()
    {
        if (player == null || isActing) return;
        ApplyFlip(player.position.x > transform.position.x);
    }

    void FlipToDirection(float dir)
    {
        if (isActing) return;
        if (Mathf.Abs(dir) < 0.01f) return;
        ApplyFlip(dir > 0f);
    }

    void ApplyFlip(bool shouldFaceRight)
    {
        if (shouldFaceRight == isFacingRight) return;

        isFacingRight = shouldFaceRight;

        Vector3 s = transform.localScale;
        transform.localScale = new Vector3(
            isFacingRight ? Mathf.Abs(s.x) : -Mathf.Abs(s.x),
            s.y,
            s.z
        );
    }

    // ================= UTIL =================

    bool PlayerInRange()
    {
        return player != null &&
               Vector2.Distance(transform.position, player.position) <= attackRange;
    }

    bool IsPlayerDead()
    {
        if (player == null) return true;
        HeroHealth h = player.GetComponent<HeroHealth>();
        return h != null && h.isDead;
    }

    void CheckPhaseTransition()
    {
        if (!isPhase2 && health != null &&
            health.currentHealth <= health.maxHealth * phase2Threshold)
        {
            isPhase2 = true;
        }
    }

    // ================= GIZMOS =================

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, chaseRunDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.green;
        Vector3 center = Application.isPlaying ? (Vector3)patrolCenter : transform.position;
        Gizmos.DrawWireSphere(center, patrolRadius);
    }
}