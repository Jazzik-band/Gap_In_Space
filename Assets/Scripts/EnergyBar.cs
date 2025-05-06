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
        if (FlashlightController.batteryLife >= 80)
            spriteRenderer.sprite = batteryBar5;
        if (FlashlightController.batteryLife >= 60 && FlashlightController.batteryLife < 79)
            spriteRenderer.sprite = batteryBar4;
        if (FlashlightController.batteryLife >= 40 && FlashlightController.batteryLife < 59)
            spriteRenderer.sprite = batteryBar3;
        if (FlashlightController.batteryLife >= 20 && FlashlightController.batteryLife < 39)
            spriteRenderer.sprite = batteryBar2;
        if (FlashlightController.batteryLife >= 1 && FlashlightController.batteryLife < 19)
            spriteRenderer.sprite = batteryBar1;
        if (FlashlightController.batteryLife <= 0)
            spriteRenderer.sprite = batteryBar0;
    }
}
