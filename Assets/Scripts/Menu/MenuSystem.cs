using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{

    public GameObject menuPrincipal;
    public GameObject menuOpciones;

    public void Jugar()
    {

        SceneManager.LoadScene("escenaPrincipal");

    }

    public void Salir()
    {

        Application.Quit();

    }

    public void setPantalla(bool estado)
    {
        Screen.fullScreen = estado;
    }

    public void entrarMenuOpciones()
    {
        menuPrincipal.SetActive(false);
        menuOpciones.SetActive(true);
    }

    public void volverMenuPrincipal()
    {
        menuOpciones.SetActive(false);
        menuPrincipal.SetActive(true);
    }

}

