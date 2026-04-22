using System.Collections;
using UnityEngine;
public class HeroHealth : MonoBehaviour
{
    [Header("Salud")]
    public int maxHealth = 100;
    public int currentHealth;
    public bool isDead = false;

    [Header("Retroceso")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;

    [Header("Invencibilidad")]
    public float invincibleDuration = 0.5f;
    private bool isInvincible = false;
    private bool isKnockedBack = false;

    [Header("Audio")]
    public AudioClip hurtSound;
    public AudioClip blockSound;
    private AudioSource audioSource;

    private Rigidbody2D rb;
    private Animator anim;
    private HeroKnight heroKnight;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        heroKnight = GetComponent<HeroKnight>();
        audioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage(int damage, Vector2 attackerPosition)
    {
        if (isDead) return;
        if (isInvincible) return;

        bool isBlocking = anim.GetBool("IdleBlock");

        if (isBlocking)
        {
            bool attackerIsInFront = IsAttackerInFront(attackerPosition);
            if (attackerIsInFront)
            {
                if (blockSound != null) audioSource.PlayOneShot(blockSound);
                StartCoroutine(Knockback(attackerPosition));
                return;
            }
        }

        if (hurtSound != null) audioSource.PlayOneShot(hurtSound);

        int previousHealth = currentHealth;
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        CurrentHealth healthData = new CurrentHealth();
        healthData.current = currentHealth;
        healthData.previous = previousHealth;
        heroKnight.PlayHurtAnimationOnGotHit(healthData);

        StartCoroutine(InvincibilityFrames());
        StartCoroutine(Knockback(attackerPosition));

        if (currentHealth <= 0)
            Die();
    }

    bool IsAttackerInFront(Vector2 attackerPosition)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        bool facingRight = !sr.flipX;
        bool attackerIsToTheRight = attackerPosition.x > transform.position.x;
        return (facingRight && attackerIsToTheRight) || (!facingRight && !attackerIsToTheRight);
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

        HeroKnight hk = GetComponent<HeroKnight>();
        if (hk != null) hk.enabled = false;

        Vector2 knockbackDir = ((Vector2)transform.position - attackerPosition).normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        if (isDead) yield break;

        rb.velocity = new Vector2(0, rb.velocity.y);
        isKnockedBack = false;

        if (hk != null) hk.enabled = true;
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("Jugador curado. Vida actual: " + currentHealth);
    }

    void Die()
    {
        isDead = true;

        anim.SetBool("noBlood", false);
        anim.SetTrigger("Death");

        rb.velocity = Vector2.zero;

        HeroKnight movementScript = GetComponent<HeroKnight>();
        if (movementScript != null) movementScript.enabled = false;

        HeroAttack attackScript = GetComponent<HeroAttack>();
        if (attackScript != null) attackScript.enabled = false;

        gameObject.layer = LayerMask.NameToLayer("Default");
        gameObject.tag = "Untagged";

        StartCoroutine(HandleDeath());

    }

    IEnumerator HandleDeath()
    {
        yield return new WaitForSecondsRealtime(1.5f);

        PantallaMuerte gom = FindObjectOfType<PantallaMuerte>();

        if (gom != null)
        {
            gom.ShowGameOver();
        }
        else
        {
            Debug.LogError("No se encontró PantallaMuerte en la escena");
        }
    }

    public bool IsKnockedBack() => isKnockedBack;
}