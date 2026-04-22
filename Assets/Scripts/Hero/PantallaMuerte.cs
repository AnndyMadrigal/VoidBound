using UnityEngine.SceneManagement;
using UnityEngine;

public class PantallaMuerte : MonoBehaviour
{
    public GameObject Muerte;

    void Start()
    {
        if (Muerte == null)
        {
            Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (Canvas c in canvases)
            {
                Transform t = c.transform.Find("Muerte");
                if (t != null)
                {
                    Muerte = t.gameObject;
                    break;
                }
            }
        }

        if (Muerte != null)
            Muerte.SetActive(false);
        else
            Debug.LogError("No se encontró el objeto Muerte");
    }


    public void ShowGameOver()
    {
        if (Muerte != null) Muerte.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1f;
    }
}