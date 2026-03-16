using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FollowCameraRotation))]
public class HealthBar : MonoBehaviour
{
    [SerializeField] bool isBillboarded = true;
    [SerializeField] bool shouldShowHealthNumbers = true;

    float finalValue;
    float animationSpeed = 0.1f;
    float leftoverAmount = 0f;

    // Caches
    [SerializeField] HealthSystemForDummies healthSystem;
    [SerializeField] Image image;
    Text text;
    FollowCameraRotation followCameraRotation;

    private void Start()
    {
        
        if (image == null) image = GetComponentInChildren<Image>();
        text = GetComponentInChildren<Text>();
        followCameraRotation = GetComponent<FollowCameraRotation>();
    if(followCameraRotation != null) followCameraRotation.enabled = false;

    if (healthSystem != null)
    {
        healthSystem.OnCurrentHealthChanged.AddListener(ChangeHealthFill);
    }

}

    void Update()
    {
        animationSpeed = healthSystem.AnimationDuration;

        if (!healthSystem.HasAnimationWhenHealthChanges)
        {
            image.fillAmount = healthSystem.CurrentHealthPercentage / 100;
        }

        if (text != null)
        {
            if (shouldShowHealthNumbers)
            {
                text.text = $"{Mathf.Round(healthSystem.CurrentHealth)}/{Mathf.Round(healthSystem.MaximumHealth)}";
                text.enabled = true;
            }
            else
            {
                text.enabled = false;
            }
        }

        if (followCameraRotation != null)
            followCameraRotation.enabled = isBillboarded;
    }

    private void ChangeHealthFill(CurrentHealth currentHealth)
    {
        if (!healthSystem.HasAnimationWhenHealthChanges) return;

        StopAllCoroutines();
        StartCoroutine(ChangeFillAmount(currentHealth));
    }

    private IEnumerator ChangeFillAmount(CurrentHealth currentHealth)
    {
        finalValue = currentHealth.percentage / 100;

        float cacheLeftoverAmount = this.leftoverAmount;

        float timeElapsed = 0;

        while (timeElapsed < animationSpeed)
        {
            float leftoverAmount = Mathf.Lerp((currentHealth.previous / healthSystem.MaximumHealth) + cacheLeftoverAmount, finalValue, timeElapsed / animationSpeed);
            this.leftoverAmount = leftoverAmount - finalValue;
            image.fillAmount = leftoverAmount;
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        this.leftoverAmount = 0;
        image.fillAmount = finalValue;
    }
}