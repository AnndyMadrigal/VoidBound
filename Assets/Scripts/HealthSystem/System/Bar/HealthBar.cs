using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Referencias")]
    public HeroHealth heroHealth;
    public Image fillImage;

    [Header("Configuración")]
    public float animationSpeed = 10f; // Qué tan rápido se vacía la barra

    void Start()
    {
        
        if (heroHealth != null && fillImage != null)
        {
            fillImage.fillAmount = (float)heroHealth.currentHealth / heroHealth.maxHealth;
        }
    }

    void Update()
    {
        
        if (heroHealth == null || fillImage == null) return;

        // 1. Calculamos el porcentaje de vida actual (Ej: 50 / 100 = 0.5)
        float targetFill = (float)heroHealth.currentHealth / heroHealth.maxHealth;

        
        fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, targetFill, Time.deltaTime * animationSpeed);
    }
}