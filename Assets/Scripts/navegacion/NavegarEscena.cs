using UnityEngine;
using UnityEngine.SceneManagement;

public class NavegarEscena : MonoBehaviour
{
    [SerializeField] private string nombreEscena = "";
    [SerializeField] private string spawnDestinoID = "spawn1";
    [SerializeField] private int idPortal = 1;
    [SerializeField] private AudioClip sonidoPortal;

    private bool jugadorDentro = false;

    private void Update()
    {
        if (jugadorDentro && Input.GetKeyDown(KeyCode.E))
        {
            IntentarUsarPortal();
        }
    }

    private void IntentarUsarPortal()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager no encontrado");
            return;
        }

        if (!PuedeUsarPortal())
        {
            MostrarMensajeBloqueo();
            return;
        }

        CambiarEscena();
    }

    private bool PuedeUsarPortal()
    {
        if (idPortal == 1)
        {
            return true;
        }
        else if (idPortal == 2)
        {
            return GameManager.Instance.llave1;
        }
        else if (idPortal == 3)
        {
            return GameManager.Instance.llave2;
        }

        return false;
    }

    private void MostrarMensajeBloqueo()
    {
        if (idPortal == 2)
            Debug.Log("Portal bloqueado. Necesitas la LLAVE 1");
        else if (idPortal == 3)
            Debug.Log("Portal bloqueado. Necesitas la LLAVE 2");
    }

    private void CambiarEscena()
    {
        if (PlayerPersistence.Instance != null)
        {
            PlayerPersistence.Instance.spawnPointID = spawnDestinoID;
        }

        if (sonidoPortal != null)
        {
            AudioSource.PlayClipAtPoint(sonidoPortal, transform.position);
        }

        SceneManager.LoadScene(nombreEscena);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = false;
        }
    }
}