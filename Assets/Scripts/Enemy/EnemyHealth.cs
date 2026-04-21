using System.Collections;
using UnityEngine;
public class EnemyHealth : MonoBehaviour
{
    [Header("Salud")]
    public int maxHealth = 50;
    public int currentHealth;
    [SerializeField] private string enemigoID;

    [Header("Retroceso")]
    public float knockbackForce = 4f;
    public float knockbackDuration = 0.3f;

    [Header("Flash de dano")]
    public float flashDuration = 0.15f;

    [Header("Audio")]
    public AudioClip hurtSound;
    private AudioSource audioSource;

    [Header("Drop")]
    public GameObject itemCuracion;
    public int cantidadDrops = 1;
    public float rangoDispersion = 2f;

    private bool isKnockedBack = false;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Color originalColor;

    void Start()
    {
        if (string.IsNullOrEmpty(enemigoID))
        {
            enemigoID = gameObject.name + "_" + GetInstanceID();
        }

        if (EnemyManager.Instance != null && EnemyManager.Instance.EstaEnemigoMuerto(enemigoID))
        {
            Destroy(gameObject);
            return;
        }

        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        audioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage(int damage, Vector2 attackerPosition)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (hurtSound != null) audioSource.PlayOneShot(hurtSound);

        StartCoroutine(FlashRed());
        StartCoroutine(Knockback(attackerPosition));

        if (currentHealth <= 0)
            Die();
    }

    IEnumerator FlashRed()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        sr.color = originalColor;
    }

    IEnumerator Knockback(Vector2 attackerPosition)
    {
        isKnockedBack = true;
        Vector2 knockbackDir = ((Vector2)transform.position - attackerPosition).normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackDuration);
        rb.velocity = Vector2.zero;
        isKnockedBack = false;
    }

    void Die()
    {
        Debug.Log("Enemy murio");

        EnemyBehaviour behaviour = GetComponent<EnemyBehaviour>();
        if (behaviour != null) behaviour.enabled = false;

        EnemyAttack attack = GetComponent<EnemyAttack>();
        if (attack != null) attack.SetDead();

        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.MarcarEnemigoMuerto(enemigoID);
        }

        DropearItems();
        StartCoroutine(DeathEffect());
    }

    void DropearItems()
    {
        if (itemCuracion == null)
        {
            Debug.LogWarning("Item de curacion no asignado en " + gameObject.name);
            return;
        }

        for (int i = 0; i < cantidadDrops; i++)
        {
            Vector3 posicionAleatoria = transform.position + new Vector3(
                Random.Range(-rangoDispersion, rangoDispersion),
                Random.Range(-1f, 0.5f),
                0
            );

            Instantiate(itemCuracion, posicionAleatoria, Quaternion.identity);
        }

        Debug.Log("Items dropeados: " + cantidadDrops);
    }

    IEnumerator DeathEffect()
    {
        EnemyBehaviour behaviour = GetComponent<EnemyBehaviour>();
        if (behaviour != null) behaviour.enabled = false;

        rb.gravityScale = 2f;
        rb.velocity = new Vector2(rb.velocity.x, -5f);

        float duration = 1.5f;
        float elapsed = 0f;
        Color originalColor = sr.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            transform.Rotate(0f, 0f, 80f * Time.deltaTime);
            yield return null;
        }

        Destroy(gameObject);
    }

    public bool IsKnockedBack() => isKnockedBack;
}