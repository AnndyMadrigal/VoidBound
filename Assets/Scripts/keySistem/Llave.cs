using UnityEngine;

public class Llave : MonoBehaviour
{
    [SerializeField] private int idLlave = 1;
    [SerializeField] private AudioClip sonidoRecogida;
    private bool yaRecogida = false;

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager no encontrado en la escena");
            return;
        }

        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            Debug.LogError("Llave no tiene BoxCollider2D");
            return;
        }

        if (!collider.isTrigger)
        {
            Debug.LogError("BoxCollider2D no está marcado como Trigger");
            return;
        }

        Debug.Log("Llave " + idLlave + " lista para ser recogida");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Llave " + idLlave + " - Colisión detectada con: " + other.gameObject.name + " (Tag: " + other.tag + ")");

        if (!other.CompareTag("Player"))
        {
            Debug.Log("Llave " + idLlave + " - No es el Player, ignorando");
            return;
        }

        if (yaRecogida)
        {
            Debug.Log("Llave " + idLlave + " - Ya fue recogida");
            return;
        }

        yaRecogida = true;

        if (idLlave == 1)
        {
            GameManager.Instance.SetLlave1(true);
            Debug.Log("LLAVE 1 RECOGIDA!");
        }
        else if (idLlave == 2)
        {
            GameManager.Instance.SetLlave2(true);
            Debug.Log("LLAVE 2 RECOGIDA!");
        }
        else
        {
            Debug.LogWarning("ID de llave desconocido: " + idLlave);
        }

        GameManager.Instance.MostrarEstado();

        if (sonidoRecogida != null)
        {
            AudioSource.PlayClipAtPoint(sonidoRecogida, transform.position);
        }

        Destroy(gameObject);
    }
}