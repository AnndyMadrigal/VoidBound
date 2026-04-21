using UnityEngine;
public class BossAttack : MonoBehaviour
{
    [Header("Daño")]
    public int meleeDamage = 25;
    public int chargeDamage = 35;
    public int jumpDamage = 40;

    [Header("Rangos")]
    public float meleeRange = 1.8f;
    public float chargeRange = 2.2f;
    public float jumpRange = 2.5f;

    [Header("Capa del Hero")]
    public LayerMask heroLayer;

    [Header("Puntos de ataque")]
    public Transform meleePoint;
    public Transform chargePoint;
    public Transform jumpPoint;

    [Header("Audio")]
    public AudioClip meleeSound;
    public AudioClip chargeSound;
    public AudioClip jumpSound;
    private AudioSource audioSource;

    private bool isAttackActive = false;
    private int currentAttackIndex = 1;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SetCurrentAttack(int index)
    {
        currentAttackIndex = index;
    }

    public void EnableAttack()
    {
        isAttackActive = true;
        Debug.Log("[Boss] EnableAttack  (index=" + currentAttackIndex + ")");
    }

    public void DealDamage()
    {
        if (!isAttackActive)
        {
            Debug.Log("[Boss] DealDamage ignorado - attack no activo");
            return;
        }

        Transform point;
        float range;
        int damage;

        switch (currentAttackIndex)
        {
            case 3:
                point = chargePoint;
                range = chargeRange;
                damage = chargeDamage;
                if (chargeSound != null) audioSource.PlayOneShot(chargeSound);
                break;
            case 4:
                point = jumpPoint;
                range = jumpRange;
                damage = jumpDamage;
                if (jumpSound != null) audioSource.PlayOneShot(jumpSound);
                break;
            default:
                point = meleePoint;
                range = meleeRange;
                damage = meleeDamage;
                if (meleeSound != null) audioSource.PlayOneShot(meleeSound);
                break;
        }

        if (point == null)
        {
            Debug.LogWarning("[Boss] Punto de ataque no asignado para index=" + currentAttackIndex);
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(point.position, range, heroLayer);
        if (hits.Length == 0)
        {
            Debug.Log("[Boss] DealDamage: no hay hero en rango");
            return;
        }

        foreach (Collider2D hit in hits)
        {
            HeroHealth h = hit.GetComponent<HeroHealth>();
            if (h != null)
            {
                Debug.Log("[Boss] PEGÓ A: " + hit.name + " (" + damage + " dmg)");
                h.TakeDamage(damage, transform.position);
            }
        }
    }

    public void DisableAttack()
    {
        Debug.Log("[Boss] DisableAttack");
        isAttackActive = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (meleePoint != null) Gizmos.DrawWireSphere(meleePoint.position, meleeRange);
        Gizmos.color = Color.yellow;
        if (chargePoint != null) Gizmos.DrawWireSphere(chargePoint.position, chargeRange);
        Gizmos.color = Color.magenta;
        if (jumpPoint != null) Gizmos.DrawWireSphere(jumpPoint.position, jumpRange);
    }
}