using UnityEngine;

public class DamageDealerEnemy : MonoBehaviour
{
    [SerializeField] private float damageAmount = 20f;
    [SerializeField] private string targetTag = "Player"; // Cambia esto a "Player" en los ataques enemigos

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si chocamos con el objetivo correcto
        if (collision.CompareTag(targetTag))
        {
            // Busca el sistema de vida en el objetivo y le aplica el daño
            HealthSystemForDummies targetHealth = collision.GetComponent<HealthSystemForDummies>();
            if (targetHealth != null)
            {
                targetHealth.AddToCurrentHealth(-damageAmount);
            }
        }
    }
}
