using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlashlightController : MonoBehaviour
{
    private Light2D flashlight;
    private float flashlightIntensity;
    private float flashlightRadius;
    public float batteryLife = 100.0f;
    private float drainRate = 10.0f;
    private bool isFlashLightOn = true;
    
    private void Start()
    {
        flashlight = GetComponent<Light2D>();
        flashlightIntensity = flashlight.intensity;
        flashlightRadius = flashlight.pointLightOuterRadius;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isFlashLightOn = !isFlashLightOn;
        }

        if (isFlashLightOn)
        {
            if (batteryLife > 0)
            {
                batteryLife -= drainRate / 2 * Time.deltaTime;
            }
            else
            {
                batteryLife = 0;
                isFlashLightOn = false;
            }
        }
        else
        {
            if (batteryLife < 100)
            {
                batteryLife += drainRate * Time.deltaTime;
            }
            else
            {
                batteryLife = 100;
            }
        }
        
        if (PlayerController.IsCrouching() && isFlashLightOn)
        {
            flashlight.intensity = flashlightIntensity - 7;
            flashlight.pointLightOuterRadius = flashlightRadius - 4;
        }
        else if (isFlashLightOn)
        {
            flashlight.intensity = flashlightIntensity;
            flashlight.pointLightOuterRadius = flashlightRadius;
        }
        else
        {
            flashlightIntensity = 0;
            flashlight.pointLightOuterRadius = 0;
        }
        
    }
}
