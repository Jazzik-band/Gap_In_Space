using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlashlightController : MonoBehaviour
{
    private Light2D flashlight;
    private float flashlightIntensity;
    private float flashlightRadius;
    public static float BatteryLife = 100.0f;
    private readonly float drainRate = 2.0f;
    public static bool IsFlashLightOn = true;
    public static bool IsFlashLightSuper;
    
    
    private void Start()
    {
        flashlight = GetComponent<Light2D>();
        flashlightIntensity = flashlight.intensity;
        flashlightRadius = flashlight.pointLightOuterRadius;
        BatteryLife = 100;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            IsFlashLightOn = !IsFlashLightOn;
        }

        // Включение супер режима у фонарика
        if (Input.GetKey(KeyCode.Mouse1))
        {
            IsFlashLightSuper = true;
            flashlightIntensity = 200;
        }
        else
        {
            IsFlashLightSuper = false;
            flashlightIntensity = 10;
        }
        
        if (IsFlashLightOn)
        {
            if (BatteryLife > 0)
            {
                if (IsFlashLightSuper)
                {
                    BatteryLife -= drainRate * 4 * Time.deltaTime;
                }
                else
                {
                    BatteryLife -= drainRate / 2 * Time.deltaTime;
                }
            }
            else
            {
                BatteryLife = 0;
                IsFlashLightOn = false;
            }
        }

        if (PlayerController.IsCrouching() && IsFlashLightOn)
        {
            flashlight.intensity = flashlightIntensity - 7;
            flashlight.pointLightOuterRadius = flashlightRadius - 4;
        }
        else if (IsFlashLightOn)
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