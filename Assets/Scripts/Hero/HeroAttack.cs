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

    [Header("Audio")]
    public AudioClip attackSound;
    private AudioSource audioSource;

    private float lastAttackTime;
    private Animator anim;
    private HeroHealth heroHealth;

    void Start()
    {
        anim = GetComponent<Animator>();
        heroHealth = GetComponent<HeroHealth>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (heroHealth.IsKnockedBack()) return;
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Z)) && Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            anim.SetTrigger("attack");
            if (attackSound != null) audioSource.PlayOneShot(attackSound);
            Debug.Log("[Hero] Trigger attack disparado");
        }
    }

    public void DealDamage()
    {
        Debug.Log("[Hero] DealDamage() llamado (Animation Event OK)");
        if (attackPoint == null)
        {
            Debug.LogError("[Hero] attackPoint NO esta asignado en el Inspector");
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
            Debug.Log("[Hero] Ningun enemigo en rango. Revisa: posicion del attackPoint, attackRange y capa del enemigo.");
            return;
        }
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("[Hero] Detectado: " + enemy.name + " (capa: " + LayerMask.LayerToName(enemy.gameObject.layer) + ")");

            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                Debug.Log("[Hero] Pegando a EnemyHealth: " + enemy.name);
                enemyHealth.TakeDamage(attackDamage, transform.position);
                continue;
            }

            BossHealth bossHealth = enemy.GetComponent<BossHealth>();
            if (bossHealth != null)
            {
                Debug.Log("[Hero] Pegando a BossHealth: " + enemy.name);
                bossHealth.TakeDamage(attackDamage, transform.position);
                continue;
            }

            Debug.LogWarning("[Hero] " + enemy.name + " esta en la capa enemigo pero no tiene EnemyHealth ni BossHealth.");
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}