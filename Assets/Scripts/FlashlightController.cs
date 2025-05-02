using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlashlightController : MonoBehaviour
{
    private Light2D flashlight;
    private float flashlightIntensity;
    private float flashlightRadius;
    public float batteryLife = 100.0f;
    private float drainRate = 2.0f;
    private bool isFlashLightOn = true;
    public static bool isFlashLightSuper = false;
    
    
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

        // Включение супер режима у фонарика
        if (Input.GetKey(KeyCode.Mouse1))
        {
            isFlashLightSuper = true;
            flashlightIntensity = 200;
        }
        else
        {
            isFlashLightSuper = false;
            flashlightIntensity = 10;
        }
        
        if (isFlashLightOn)
        {
            if (batteryLife > 0)
            {
                if (isFlashLightSuper)
                {
                    batteryLife -= drainRate * 4 * Time.deltaTime;
                }
                else
                {
                    batteryLife -= drainRate / 2 * Time.deltaTime;
                }
            }
            else
            {
                batteryLife = 0;
                isFlashLightOn = false;
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
            flashlight.intensity = 0;
            flashlight.pointLightOuterRadius = 0;
        }
    }

}