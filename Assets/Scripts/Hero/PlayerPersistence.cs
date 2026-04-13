using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

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
        // 1. RECONECTAR LA CÁMARA
    //buscamos la cámara de la habitacion habitación
    var camaraVirtual = Object.FindFirstObjectByType<Cinemachine.CinemachineVirtualCamera>();
    if (camaraVirtual != null)
    {
        camaraVirtual.Follow = this.transform; 
        Debug.Log("Cámara vinculada al jugador en: " + scene.name);
    }

    // 2. RECONECTAR LA BARRA DE VIDA
    //buscamos el script HealthBar en la nueva habitación
    HealthBar barraDeVida = Object.FindFirstObjectByType<HealthBar>();
    HeroHealth miSalud = GetComponent<HeroHealth>();

    if (barraDeVida != null && miSalud != null)
    {
        barraDeVida.heroHealth = miSalud;
        Debug.Log("Barra de vida vinculada en: " + scene.name);
    }
    // 3. MOVER AL JUGADOR AL PUNTO DE SPAWN CORRECTO
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