using System.Collections;
using UnityEngine;


public class HeroHealth : MonoBehaviour
{
    [Header("Salud")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Retroceso")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;

    [Header("Invencibilidad")]
    public float invincibleDuration = 0.5f;
    private bool isInvincible = false;
    private bool isKnockedBack = false;

    private Rigidbody2D rb;
    private Animator anim;
    private HeroKnight heroKnight;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        heroKnight = GetComponent<HeroKnight>();
    }

    public void TakeDamage(int damage, Vector2 attackerPosition)
    {
        if (isInvincible) return;

        int previousHealth = currentHealth;
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Animacion de hurt
        CurrentHealth healthData = new CurrentHealth();
        healthData.current = currentHealth;
        healthData.previous = previousHealth;
        heroKnight.PlayHurtAnimationOnGotHit(healthData);

        StartCoroutine(InvincibilityFrames());
        StartCoroutine(Knockback(attackerPosition));

        if (currentHealth <= 0)
            Die();
    }

    IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
    }

    IEnumerator Knockback(Vector2 attackerPosition)
    {
        isKnockedBack = true;

        // Desactiva el control del HeroKnight durante el knockback
        HeroKnight hk = GetComponent<HeroKnight>();
        if (hk != null) hk.enabled = false;

        Vector2 knockbackDir = ((Vector2)transform.position - attackerPosition).normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        rb.velocity = new Vector2(0, rb.velocity.y);
        isKnockedBack = false;

        // Reactiva el control del HeroKnight
        if (hk != null) hk.enabled = true;
    }

    void Die()
    {
        heroKnight.OnIsAliveChanged(false);
        rb.velocity = Vector2.zero;
        this.enabled = false;
    }

    public bool IsKnockedBack() => isKnockedBack;
}


