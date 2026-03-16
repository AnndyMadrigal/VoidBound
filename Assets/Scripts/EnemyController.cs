using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        // Obtenemos el animador del enemigo al iniciar
        animator = GetComponent<Animator>();
    }

    // El sistema de vida "tocará este timbre" cuando la vida llegue a 0
    public void OnIsAliveChanged(bool isAlive)
    {
        if (animator != null)
        {
            animator.SetBool("b_is_alive", isAlive);
            
            // Si el valor es falso (muerto), dispara la animación
            if(!isAlive)
            {
                animator.SetTrigger("Death");
                
                // Opcional pero recomendado: Desactivar el collider para que 
                // el jugador no siga chocando con el cadáver
                Collider2D col = GetComponent<Collider2D>();
                if (col != null) col.enabled = false;
            }
        }
    }
}