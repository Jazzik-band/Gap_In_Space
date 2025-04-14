using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [Header("Slider Settings")]
    [SerializeField] private Slider slider;
    [SerializeField] private float hideDelay = 1f;
    
    private Image sliderFillImage;
    private float lastStaminaChangeTime;
    private bool wasFullLastFrame;

    private void Start()
    {
        sliderFillImage = slider.fillRect.GetComponent<Image>();
        sliderFillImage.enabled = false;
        lastStaminaChangeTime = Time.time;
        wasFullLastFrame = Mathf.Approximately(PlayerMovement.GetStaminaNormalized(), 1f);
    }
    
    private void Update()
    {
        HandleStamina();
    }

    private void HandleStamina()
    {
        var staminaValue = PlayerMovement.GetStaminaNormalized();
        slider.value = staminaValue;
        var isFullNow = Mathf.Approximately(staminaValue, 1f);
        if (isFullNow != wasFullLastFrame)
        {
            lastStaminaChangeTime = Time.time;
            wasFullLastFrame = isFullNow;
            if (!isFullNow && sliderFillImage)
                sliderFillImage.enabled = true;
        }
        if (isFullNow && sliderFillImage && 
            Time.time >= lastStaminaChangeTime + hideDelay)
        {
            sliderFillImage.enabled = false;
        }
    }
}