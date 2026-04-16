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
            Debug.Log("[Hero] Trigger attack disparado");
        }
    }

    public void DealDamage()
    {
        Debug.Log("[Hero] DealDamage() llamado (Animation Event OK)");

        if (attackPoint == null)
        {
            Debug.LogError("[Hero] attackPoint NO está asignado en el Inspector");
            return;
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayer
        );

        Debug.Log("[Hero] Objetos detectados en rango: " + hitEnemies.Length);

        if (hitEnemies.Length == 0)
        {
            Debug.Log("[Hero] Ningún enemigo en rango. Revisa: posición del attackPoint, attackRange y capa del enemigo.");
            return;
        }

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("[Hero] Detectado: " + enemy.name + " (capa: " + LayerMask.LayerToName(enemy.gameObject.layer) + ")");

            // 1. Intentar pegar a un enemigo normal
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                Debug.Log("[Hero] Pegando a EnemyHealth: " + enemy.name);
                enemyHealth.TakeDamage(attackDamage, transform.position);
                continue;
            }

            // 2. Si no es enemigo normal, intentar pegar a un Boss
            BossHealth bossHealth = enemy.GetComponent<BossHealth>();
            if (bossHealth != null)
            {
                Debug.Log("[Hero] Pegando a BossHealth: " + enemy.name);
                bossHealth.TakeDamage(attackDamage, transform.position);
                continue;
            }

            Debug.LogWarning("[Hero] " + enemy.name + " está en la capa enemigo pero no tiene EnemyHealth ni BossHealth. Probablemente el collider que detecté es un hijo del objeto — asegúrate de que el componente Health esté en el GameObject con el collider, o que el collider esté en el padre.");
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}