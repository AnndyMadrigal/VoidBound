using System.Collections;
using UnityEngine;

public class BossBehaviour : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;

    [Header("Detección")]
    public float detectionRange = 10f;
    public float attackRange = 1.8f;
    public float chargeRange = 5f;
    public float chaseRunDistance = 5f;

    [Header("Movimiento")]
    public float walkSpeed = 2.5f;
    public float runSpeed = 5f;

    [Header("Tiempos entre ataques")]
    public float pauseShort = 1f;
    public float pauseMedium = 1.5f;
    public float pauseLong = 2.5f;

    [Header("Fase 2 (50% vida)")]
    [Range(0f, 1f)]
    public float phase2Threshold = 0.5f;
    public float phase2SpeedMultiplier = 1.35f;
    public float phase2CooldownMultiplier = 0.65f;
    private bool isPhase2 = false;

    private Rigidbody2D rb;
    private Animator anim;
    private BossHealth health;
    private bool isFacingRight = true;
    private bool isActing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        health = GetComponent<BossHealth>();
        rb.freezeRotation = true;
        StartCoroutine(AttackPattern());
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
            else return;
        }

        HeroHealth targetHealth = player.GetComponent<HeroHealth>();
        if (targetHealth != null && targetHealth.isDead)
        {
            StopMovement();
            anim.SetBool("isAttacking", false);
            return;
        }

        if (health != null && health.IsKnockedBack()) return;
        if (isActing) return;

        CheckPhaseTransition();
        FlipTowardsPlayer();

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > detectionRange)
        {
            StopMovement();
            anim.SetBool("isAttacking", false);
        }
        else if (distance > attackRange)
        {
            anim.SetBool("isAttacking", false);
            Chase(distance);
        }
        else
        {
            StopMovement();
        }

        anim.SetFloat("speed", Mathf.Abs(rb.velocity.x));
    }

  
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
        yield return StartCoroutine(DoAttack(1));
        yield return new WaitForSeconds(pauseShort);

        if (PlayerInRange(attackRange))
        {
            yield return StartCoroutine(DoAttack(1));
            yield return new WaitForSeconds(pauseShort);
        }

        yield return StartCoroutine(DoAttack(3));
        yield return new WaitForSeconds(pauseLong);
    }

    IEnumerator PatternPhase2()
    {
        yield return StartCoroutine(DoAttack(1));
        yield return new WaitForSeconds(pauseShort * phase2CooldownMultiplier);

        yield return StartCoroutine(DoAttack(3));
        yield return new WaitForSeconds(pauseShort * phase2CooldownMultiplier);

        if (PlayerInRange(attackRange))
        {
            yield return StartCoroutine(DoAttack(1));
            yield return new WaitForSeconds(pauseShort * phase2CooldownMultiplier);
        }

        yield return StartCoroutine(DoAttack(4));
        yield return new WaitForSeconds(pauseMedium * phase2CooldownMultiplier);

        yield return StartCoroutine(DoAttack(3));
        yield return new WaitForSeconds(pauseLong * phase2CooldownMultiplier);
    }

   
    IEnumerator DoAttack(int index)
    {
        isActing = true;
        StopMovement();
        FlipTowardsPlayer();

        anim.SetBool("isAttacking", false);
        anim.SetInteger("attackIndex", index);
        yield return null;
        anim.SetBool("isAttacking", true);

        yield return new WaitForSeconds(GetClipLength(index) * 0.95f);

        anim.SetBool("isAttacking", false);
        isActing = false;
    }

    float GetClipLength(int index)
    {
        switch (index)
        {
            case 1: return 1.0f;  
            case 3: return 1.0f;  
            case 4: return 1.0f;  
            default: return 0.8f;
        }
    }

    bool PlayerInRange(float range) =>
        player != null && Vector2.Distance(transform.position, player.position) <= range;

    bool IsPlayerDead()
    {
        if (player == null) return true;
        HeroHealth h = player.GetComponent<HeroHealth>();
        return h != null && h.isDead;
    }

    void Chase(float distance)
    {
        float speed = (distance > chaseRunDistance) ? runSpeed : walkSpeed;
        if (isPhase2) speed *= phase2SpeedMultiplier;
        float dir = (player.position.x > transform.position.x) ? 1f : -1f;
        rb.velocity = new Vector2(dir * speed, rb.velocity.y);
    }

    void StopMovement()
    {
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    void CheckPhaseTransition()
    {
        if (!isPhase2 && health != null &&
            health.currentHealth <= health.maxHealth * phase2Threshold)
        {
            isPhase2 = true;
            Debug.Log("[Boss] ¡Fase 2 activada!");
        }
    }

    void FlipTowardsPlayer()
    {
        if (player == null || isActing) return;
        bool shouldFaceRight = player.position.x > transform.position.x;
        if (shouldFaceRight == isFacingRight) return;
        isFacingRight = shouldFaceRight;
        Vector3 s = transform.localScale;
        transform.localScale = new Vector3(
            isFacingRight ? Mathf.Abs(s.x) : -Mathf.Abs(s.x), s.y, s.z
        );
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, chargeRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chaseRunDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}