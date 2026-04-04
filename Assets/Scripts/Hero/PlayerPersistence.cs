using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPersistence : MonoBehaviour
{
    public static PlayerPersistence Instance;

    public int vidaActual = 100;
    public string spawnPointID;

    private Rigidbody2D rb;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SpawnPoint[] puntos = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);

        Debug.Log("Escena cargada: " + scene.name);
        Debug.Log("Buscando spawnID: " + spawnPointID);

        foreach (SpawnPoint punto in puntos)
        {
            Debug.Log("Spawn encontrado en escena: " + punto.spawnID);

            if (punto.spawnID == spawnPointID)
            {
                if (rb == null)
                    rb = GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    rb.velocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                }

                transform.position = new Vector3(
                    punto.transform.position.x,
                    punto.transform.position.y,
                    0f
                );

                Debug.Log("Jugador movido a: " + transform.position);
                return;
            }
        }

        Debug.LogWarning("No se encontró un SpawnPoint con ID: " + spawnPointID);
    }
}