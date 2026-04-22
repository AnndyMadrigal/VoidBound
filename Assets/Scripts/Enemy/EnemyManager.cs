using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    private HashSet<string> enemigosMatados = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void MarcarEnemigoMuerto(string enemigoID)
    {
        enemigosMatados.Add(enemigoID);
        Debug.Log("Enemigo marcado como muerto: " + enemigoID);
    }

    public bool EstaEnemigoMuerto(string enemigoID)
    {
        return enemigosMatados.Contains(enemigoID);
    }

    public void LimpiarEnemigos()
    {
        enemigosMatados.Clear();
    }
}