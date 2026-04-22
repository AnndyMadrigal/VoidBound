using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool llave1 = false;
    public bool llave2 = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager creado y persistente");
        }
        else if (Instance != this)
        {
            Debug.Log("Destruyendo GameManager duplicado");
            Destroy(gameObject);
        }
    }

    public void SetLlave1(bool valor)
    {
        llave1 = valor;
        Debug.Log("Llave 1 = " + valor);
    }

    public void SetLlave2(bool valor)
    {
        llave2 = valor;
        Debug.Log("Llave 2 = " + valor);
    }

    public void ResetearProgreso()
    {
        llave1 = false;
        llave2 = false;
        Debug.Log("Progreso reseteado");
    }

    public void MostrarEstado()
    {
        Debug.Log("Estado actual - Llave1: " + llave1 + ", Llave2: " + llave2);
    }
}