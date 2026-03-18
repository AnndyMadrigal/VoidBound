using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    public void Jugar()
    {

        SceneManager.LoadScene("escenaPrincipal");

    }

    public void Salir()
    {

        Application.Quit();

    }

}

