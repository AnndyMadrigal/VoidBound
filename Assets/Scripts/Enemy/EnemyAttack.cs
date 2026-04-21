using UnityEngine;
public class EnemyAttack : MonoBehaviour
{
    [Header("Ataque")]
    public int attackDamage = 10;
    public float attackRange = 1.5f;
    public LayerMask heroLayer;

    [Header("Punto de ataque")]
    public Transform attackPoint;

    [Header("Audio")]
    public AudioClip attackSound;
    private AudioSource audioSource;

    private bool isAttackActive = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void EnableAttack()
    {
        isAttackActive = true;
    }

    public void DisableAttack()
    {
        isAttackActive = false;
    }

    public void DealDamage()
    {
        if (!isAttackActive) return;

        if (attackSound != null) audioSource.PlayOneShot(attackSound);

        Collider2D[] hitHeroes = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            heroLayer
        );
        foreach (Collider2D hero in hitHeroes)
        {
            HeroHealth heroHealth = hero.GetComponent<HeroHealth>();
            if (heroHealth != null)
                heroHealth.TakeDamage(attackDamage, transform.position);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}