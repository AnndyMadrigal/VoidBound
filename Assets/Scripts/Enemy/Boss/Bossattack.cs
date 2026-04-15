using UnityEngine;

public class BossAttack : MonoBehaviour
{
    [Header("Daño por ataque")]
    public int meleeDamage = 25;
    public int chargeDamage = 35;
    public int jumpDamage = 40;

    [Header("Rangos")]
    public float meleeRange = 1.8f;
    public float chargeRange = 2.2f;
    public float jumpRange = 2.5f;
    public LayerMask heroLayer;

    [Header("Puntos de ataque")]
    public Transform meleePoint;
    public Transform chargePoint;
    public Transform jumpPoint;

    private bool isAttackActive = false;
    private int currentAttackIndex = 1;

    // Llamado desde BossBehaviour antes de cada ataque
    public void SetCurrentAttack(int index)
    {
        currentAttackIndex = index;
    }

    // Animation Events ────────────────────────────────────────────────────────

    public void EnableAttack()
    {
        isAttackActive = true;
    }

    public void DealDamage()
    {
        if (!isAttackActive) return;

        Transform point;
        float range;
        int damage;

        switch (currentAttackIndex)
        {
            case 3: // embestida
                point = chargePoint != null ? chargePoint : meleePoint;
                range = chargeRange;
                damage = chargeDamage;
                break;
            case 4: // salto
                point = jumpPoint != null ? jumpPoint : meleePoint;
                range = jumpRange;
                damage = jumpDamage;
                break;
            default: // atack melee (1 y 2)
                point = meleePoint;
                range = meleeRange;
                damage = meleeDamage;
                break;
        }

        if (point == null) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(point.position, range, heroLayer);
        foreach (Collider2D hit in hits)
        {
            HeroHealth heroHealth = hit.GetComponent<HeroHealth>();
            if (heroHealth != null)
                heroHealth.TakeDamage(damage, transform.position);
        }
    }

    public void DisableAttack()
    {
        isAttackActive = false;
    }

    void OnDrawGizmosSelected()
    {
        if (meleePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(meleePoint.position, meleeRange);
        }
        if (chargePoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(chargePoint.position, chargeRange);
        }
        if (jumpPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(jumpPoint.position, jumpRange);
        }
    }
}