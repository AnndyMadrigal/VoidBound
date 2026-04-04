using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavegarEscena : MonoBehaviour
{
    public string nombreEscena;
    public string spawnDestinoID;

    private bool jugadorDentro = false;

    private void Update()
    {
        if (jugadorDentro && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Portal activado con E");

            PlayerPersistence.Instance.spawnPointID = spawnDestinoID;
            SceneManager.LoadScene(nombreEscena);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = true;
            Debug.Log("Jugador puede usar portal (presiona E)");
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