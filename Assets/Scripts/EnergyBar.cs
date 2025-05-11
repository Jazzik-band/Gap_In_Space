using System;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBar: MonoBehaviour
{
    private Image spriteRenderer;
    public Sprite batteryBar5, batteryBar4, batteryBar3, batteryBar2, batteryBar1, batteryBar0;

    private void Start()
    {
        spriteRenderer = GetComponent<Image>();
    }

    private void Update()
    {
        if (FlashlightController.BatteryLife >= 80)
            spriteRenderer.sprite = batteryBar5;
        else if (FlashlightController.BatteryLife >= 60 && FlashlightController.BatteryLife < 79)
            spriteRenderer.sprite = batteryBar4;
        else if (FlashlightController.BatteryLife >= 40 && FlashlightController.BatteryLife < 59)
            spriteRenderer.sprite = batteryBar3;
        else if (FlashlightController.BatteryLife >= 20 && FlashlightController.BatteryLife < 39)
            spriteRenderer.sprite = batteryBar2;
        else if (FlashlightController.BatteryLife >= 1 && FlashlightController.BatteryLife < 19)
            spriteRenderer.sprite = batteryBar1;
        else if (FlashlightController.BatteryLife <= 0)
            spriteRenderer.sprite = batteryBar0;
    }
}
