using System.Collections;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Salud")]
    public int maxHealth = 200;
    public int currentHealth;
    public bool isDead = false;

    [Header("Retroceso")]
    public float knockbackForce = 2f;
    public float knockbackDuration = 0.15f;

    [Header("Flash de daño")]
    public float flashDuration = 0.1f;

    [Header("Muerte")]
    public float destroyDelay = 3f;

    private bool isKnockedBack = false;
    private Rigidbody2D rb;
    private Animator anim;

    // Todas las piezas del sprite (esqueleto IK)
    private SpriteRenderer[] allSprites;
    private Color[] originalColors;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Obtener TODOS los SpriteRenderers (el propio + todos los hijos)
        allSprites = GetComponentsInChildren<SpriteRenderer>();
        originalColors = new Color[allSprites.Length];

        for (int i = 0; i < allSprites.Length; i++)
        {
            originalColors[i] = allSprites[i].color;
        }
    }

    public void TakeDamage(int damage, Vector2 attackerPosition)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("[Boss] Recibió " + damage + " de daño. Vida actual: " + currentHealth + "/" + maxHealth);

        StartCoroutine(FlashRed());
        StartCoroutine(Knockback(attackerPosition));

        if (currentHealth <= 0) Die();
    }

    IEnumerator FlashRed()
    {
        // Pintar TODAS las piezas de rojo
        foreach (SpriteRenderer sr in allSprites)
        {
            if (sr != null) sr.color = Color.red;
        }

        yield return new WaitForSeconds(flashDuration);

        // Restaurar colores originales
        if (!isDead)
        {
            for (int i = 0; i < allSprites.Length; i++)
            {
                if (allSprites[i] != null)
                    allSprites[i].color = originalColors[i];
            }
        }
    }

    IEnumerator Knockback(Vector2 attackerPosition)
    {
        isKnockedBack = true;
        Vector2 dir = ((Vector2)transform.position - attackerPosition).normalized;
        rb.velocity = new Vector2(0f, rb.velocity.y);
        rb.AddForce(new Vector2(dir.x * knockbackForce, 0f), ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackDuration);
        if (!isDead) rb.velocity = new Vector2(0f, rb.velocity.y);
        isKnockedBack = false;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("[Boss] DERROTADO. Disparando trigger Death.");

        // Desactivar comportamiento y ataques
        BossBehaviour behaviour = GetComponent<BossBehaviour>();
        if (behaviour != null) behaviour.enabled = false;

        BossAttack attackScript = GetComponent<BossAttack>();
        if (attackScript != null) attackScript.enabled = false;

        // Congelar física para que no se caiga
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;          // que no caiga
            rb.constraints = RigidbodyConstraints2D.FreezeAll; // congelar todo
        }

        // Disparar animación de muerte
        if (anim != null)
        {
            anim.ResetTrigger("Attack");
            anim.SetInteger("attackIndex", 0);
            anim.SetTrigger("Death");
            Debug.Log("[Boss] anim.SetTrigger('Death') ejecutado");
        }

        // Desactivar colliders para que no reciba más golpes
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D c in colliders)
        {
            c.enabled = false;
        }

        StartCoroutine(DestroyAfterDelay());
    }

    IEnumerator DestroyAfterDelay()
    {
        // Esperar a que se reproduzca la animación de muerte completa
        yield return new WaitForSeconds(destroyDelay);

        // Fade out en TODAS las piezas
        float fadeDuration = 1f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            for (int i = 0; i < allSprites.Length; i++)
            {
                if (allSprites[i] != null)
                {
                    Color c = originalColors[i];
                    allSprites[i].color = new Color(c.r, c.g, c.b, alpha);
                }
            }

            yield return null;
        }

        Destroy(gameObject);
    }

    public bool IsKnockedBack() => isKnockedBack;
    public float HealthPercent() => (float)currentHealth / maxHealth;
}