using UnityEngine;

public class HeroAttack : MonoBehaviour
{
    [Header("Ataque")]
    public int attackDamage = 20;
    public float attackRange = 1.5f;
    public float attackCooldown = 0.5f;
    public LayerMask enemyLayer;

    [Header("Punto de ataque")]
    public Transform attackPoint;

    private float lastAttackTime;
    private Animator anim;
    private HeroHealth heroHealth;

    void Start()
    {
        anim = GetComponent<Animator>();
        heroHealth = GetComponent<HeroHealth>();
    }

    void Update()
    {
        if (heroHealth.IsKnockedBack()) return;

        if (Input.GetKeyDown(KeyCode.Z) && Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            anim.SetTrigger("attack");
        }
    }

    public void DealDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayer
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
                enemyHealth.TakeDamage(attackDamage, transform.position);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}