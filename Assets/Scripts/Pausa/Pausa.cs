using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject Pausa;
    private bool estaPausado = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (estaPausado)
                Reanudar();
            else
                Pausar();
        }
    }

    public void Pausar()
    {
        Pausa.SetActive(true);
        Time.timeScale = 0f; 
        estaPausado = true;
    }

    public void Reanudar()
    {
        Pausa.SetActive(false);
        Time.timeScale = 1f; 
        estaPausado = false;
    }

    public void Salir()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1f;
    }

}
