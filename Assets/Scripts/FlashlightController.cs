using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlashlightController : MonoBehaviour
{
    private Light2D flashlight;
    private float flashlightIntensity;
    private float flashlightRadius;
    private void Start()
    {
        flashlight = GetComponent<Light2D>();
        flashlightIntensity = flashlight.intensity;
        flashlightRadius = flashlight.pointLightOuterRadius;
    }

    private void Update()
    {
        if (PlayerController.IsCrouching())
        {
            flashlight.intensity = flashlightIntensity - 7;
            flashlight.pointLightOuterRadius = flashlightRadius - 4;
        }
        else
        {
            flashlight.intensity = flashlightIntensity;
            flashlight.pointLightOuterRadius = flashlightRadius;
        }
    }
}
