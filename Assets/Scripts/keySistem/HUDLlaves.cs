using UnityEngine;
using UnityEngine.UI;

public class LlaveHUD : MonoBehaviour
{
    [SerializeField] private int idLlave = 1;
    [SerializeField] private Image iconoHUD;

    private void Start()
    {
        if (iconoHUD != null)
        {
            iconoHUD.enabled = false;
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && iconoHUD != null)
        {
            if (idLlave == 1)
            {
                iconoHUD.enabled = GameManager.Instance.llave1;
            }
            else if (idLlave == 2)
            {
                iconoHUD.enabled = GameManager.Instance.llave2;
            }
        }
    }
}