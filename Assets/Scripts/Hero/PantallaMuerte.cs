using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PantallaMuerte : MonoBehaviour
{
    public GameObject Muerte;

    void Start()
    {
        Muerte.SetActive(false);
    }

    public void ShowGameOver()
    {
        Muerte.SetActive(true);

        Time.timeScale = 0f; 
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1f;
    }
}