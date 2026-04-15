using System.Collections;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Salud")]
    public int maxHealth = 200;
    public int currentHealth;

    [Header("Retroceso")]
    public float knockbackForce = 2f;
    public float knockbackDuration = 0.15f;

    [Header("Flash de daño")]
    public float flashDuration = 0.1f;

    private bool isKnockedBack = false;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;
    }

    public void TakeDamage(int damage, Vector2 attackerPosition)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        StartCoroutine(FlashRed());
        StartCoroutine(Knockback(attackerPosition));

        if (currentHealth <= 0) Die();
    }

    IEnumerator FlashRed()
    {
        if (sr == null) yield break;
        sr.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        sr.color = originalColor;
    }

    IEnumerator Knockback(Vector2 attackerPosition)
    {
        isKnockedBack = true;
        Vector2 dir = ((Vector2)transform.position - attackerPosition).normalized;
        rb.velocity = new Vector2(0f, rb.velocity.y);
        rb.AddForce(new Vector2(dir.x * knockbackForce, 0f), ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackDuration);
        rb.velocity = new Vector2(0f, rb.velocity.y);
        isKnockedBack = false;
    }

    void Die()
    {
        Debug.Log("[Boss] Derrotado");
        StartCoroutine(DeathEffect());
    }

    IEnumerator DeathEffect()
    {
        BossBehaviour behaviour = GetComponent<BossBehaviour>();
        if (behaviour != null) behaviour.enabled = false;

        BossAttack attack = GetComponent<BossAttack>();
        if (attack != null) attack.enabled = false;

        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetBool("isAttacking", false);
            anim.SetFloat("speed", 0f);
            anim.SetBool("isDead", true);
        }

        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.5f);

        float duration = 2f;
        float elapsed = 0f;
        Color baseColor = sr != null ? originalColor : Color.white;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (sr != null)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                sr.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            }
            yield return null;
        }

        Destroy(gameObject);
    }

    public bool IsKnockedBack() => isKnockedBack;
    public float HealthPercent() => (float)currentHealth / maxHealth;
}