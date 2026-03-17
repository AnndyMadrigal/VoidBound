using System.Collections;
using UnityEngine;

public class HeroHealth : MonoBehaviour
{
    [Header("Salud")]
    public int maxHealth = 100;
    public int currentHealth;
    public bool isDead = false; // <-- NUEVA VARIABLE

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
        if (isDead) return;

        if (isInvincible) return;

        bool isBlocking = anim.GetBool("IdleBlock");

        if (isBlocking)
        {
            bool attackerIsInFront = IsAttackerInFront(attackerPosition);

            if (attackerIsInFront)
            {
                // Bloqueo exitoso
                StartCoroutine(Knockback(attackerPosition));
                return;
            }
        }

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
        
        //NUEVO: si el personaje murió durante el empujón, no hacemos nada más y salimos.
        if (isDead) yield break;
        
        rb.velocity = new Vector2(0, rb.velocity.y);
        isKnockedBack = false;

        if (hk != null) hk.enabled = true;
    }

    void Die()
    {
        isDead = true; //ya falleció para que TakeDamage lo ignore

        
        anim.SetBool("noBlood", false); //O true, según prefieras
        anim.SetTrigger("Death");

        //detenemos cualquier fuerza de empuje para que caiga en su lugar
        rb.velocity = Vector2.zero;

        //se desactiva los controles del jugador para que no pueda moverse cuando está muerto
        HeroKnight movementScript = GetComponent<HeroKnight>();
        if (movementScript != null) movementScript.enabled = false;

        HeroAttack attackScript = GetComponent<HeroAttack>();
        if (attackScript != null) attackScript.enabled = false;

        //NUEVO: se cambia la capa del personaje a "Default" (0) para que los enemigos no lo detecten y dejen de pegarle
        gameObject.layer = LayerMask.NameToLayer("Default");
        
        // NUEVO: Opcionalmente, quitamos su etiqueta para asegurar que la IA lo ignore
        gameObject.tag = "Untagged";
    }

    public bool IsKnockedBack() => isKnockedBack;
}


