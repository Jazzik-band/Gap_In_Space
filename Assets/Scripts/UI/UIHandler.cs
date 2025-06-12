using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [Header("Slider Settings")]
    [SerializeField] private Slider slider;
    [SerializeField] private float hideDelay = 1f;
    [SerializeField] private Gradient colorGradient;
    
    [FormerlySerializedAs("Arrow")]
    [Header("Orb Counter Settings")]
    [SerializeField] private GameObject arrow;
    [SerializeField] private TextMeshProUGUI orbCounter;
    [SerializeField] private GameObject player;
    
    private static GameObject _portal;
    public GameObject learningPortalPrefab, learningArrow;
    private Image sliderFillImage;
    private OrbHandler orbHandler;
    private float lastStaminaChangeTime;
    private bool wasFullLastFrame;
    private int currentOrbs;
    private int necessaryOrbs;

    private void Start()
    {
        orbHandler = player.GetComponent<OrbHandler>();
        necessaryOrbs = orbHandler.GetNecessaryOrbs();
        orbCounter.text = $"0 / {necessaryOrbs}";
        sliderFillImage = slider.fillRect.GetComponent<Image>();
        sliderFillImage.enabled = false;
        lastStaminaChangeTime = Time.time;
        wasFullLastFrame = Mathf.Approximately(PlayerController.GetStaminaNormalized(), 1f);
    }
    
    private void Update()
    {
        HandleStamina();
        HandleOrbs();
    }

    private void HandleStamina()
    {
        var staminaValue = PlayerController.GetStaminaNormalized();
        slider.value = staminaValue;
        sliderFillImage.color = colorGradient.Evaluate(staminaValue);
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
            sliderFillImage.enabled = false;
    }

    private void HandleOrbs()
    {
        if (currentOrbs < orbHandler.GetCurrentOrbs())
        {
            currentOrbs = orbHandler.GetCurrentOrbs();
            orbCounter.text = $"{currentOrbs} / {necessaryOrbs}";
        }

        if (currentOrbs >= necessaryOrbs)
        {
            if (SceneManager.GetActiveScene().name == "Education")
            {
                learningPortalPrefab.SetActive(true);
                learningArrow.SetActive(true);
            }
            else
            {
                _portal.SetActive(true);
                arrow.SetActive(true);
            }
        }
    }

    public static void SetPortal(GameObject portal)
    {
        _portal = portal;
    }
}