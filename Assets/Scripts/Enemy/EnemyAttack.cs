using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Ataque")]
    public int attackDamage = 10;
    public float attackRange = 1.5f;
    public LayerMask heroLayer;

    [Header("Punto de ataque")]
    public Transform attackPoint;

    private bool isAttackActive = false; // Solo hace dano durante la animacion

    // Llamado desde Animation Event al INICIAR el frame de dano
    public void EnableAttack()
    {
        isAttackActive = true;
    }

    // Llamado desde Animation Event al TERMINAR el frame de dano
    public void DisableAttack()
    {
        isAttackActive = false;
    }

    // Llamado desde Animation Event en el frame exacto de impacto
    public void DealDamage()
    {
        if (!isAttackActive) return;

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
