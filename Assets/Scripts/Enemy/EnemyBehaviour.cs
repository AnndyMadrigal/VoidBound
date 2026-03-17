using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;

    [Header("Deteccion")]
    public float detectionRange = 5.0f;
    public float stopDistance = 2.0f;
    public float attackRange = 3.0f;      // Rango de ataque

    [Header("Movimiento")]
    public float moveSpeed = 2.0f;
    public float floatAmplitude = 0.5f;
    public float floatFrequency = 1.5f;
    public float heightAbovePlayer = 2.0f; // Altura sobre el jugador

    [Header("Ataque")]
    public float attackCooldown = 1.5f;
    private float lastAttackTime;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 startPosition;
    private int lastAttack = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        startPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(rb.position, player.position);

        FlipTowardsPlayer();

        if (distanceToPlayer <= detectionRange)
        {
            // Posicion objetivo: encima del jugador
            Vector2 targetPosition = new Vector2(
                player.position.x,
                player.position.y + heightAbovePlayer
            );

            float distanceToTarget = Vector2.Distance(rb.position, targetPosition);

            if (distanceToTarget > stopDistance)
            {
                // Moverse hacia arriba del jugador
                anim.SetBool("isAttacking", false);
                MoveToTarget(targetPosition);
            }
            else
            {
                // Ya esta arriba del jugador, flotar y atacar si esta en rango
                HoverInPlace();

                if (distanceToPlayer <= attackRange)
                    TryAttack();
                else
                    anim.SetBool("isAttacking", false);
            }
        }
        else
        {
            anim.SetBool("isAttacking", false);
            FloatIdle();
        }
    }

    void MoveToTarget(Vector2 target)
    {
        Vector2 direction = (target - rb.position).normalized;
        float floatOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude * Time.fixedDeltaTime;

        Vector2 newPos = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
        newPos.y += floatOffset;

        rb.MovePosition(newPos);
    }

    void FloatIdle()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        Vector2 idlePos = new Vector2(startPosition.x, newY);
        rb.MovePosition(Vector2.Lerp(rb.position, idlePos, Time.fixedDeltaTime * moveSpeed));
    }

    void HoverInPlace()
    {
        float floatOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + new Vector2(0f, floatOffset));
    }

    void FlipTowardsPlayer()
    {
        if (player == null) return;

        if (player.position.x < rb.position.x)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    void TryAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;

            // Fuerza alternar entre 1 y 2 sin repetir
            int attack = (lastAttack == 1) ? 2 : 1;
            lastAttack = attack;

            anim.SetBool("isAttacking", false);  // ← Reset primero
            anim.SetInteger("attackIndex", attack);

            // Pequeño delay para que el Animator procese el cambio
            StartCoroutine(TriggerAttack());
        }
    }

    IEnumerator TriggerAttack()
    {
        yield return null; // espera 1 frame
        anim.SetBool("isAttacking", true);
    }


    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(attackCooldown * 0.9f);
        anim.SetBool("isAttacking", false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Muestra la posicion objetivo encima del jugador
        if (player != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 targetPos = new Vector3(player.position.x, player.position.y + heightAbovePlayer, 0);
            Gizmos.DrawWireSphere(targetPos, 0.3f);
            Gizmos.DrawLine(transform.position, targetPos);
        }
    }
}