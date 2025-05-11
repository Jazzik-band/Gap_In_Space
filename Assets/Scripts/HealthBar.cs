using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar: MonoBehaviour
{
    public Sprite healthBar0, healthBar1,
                    healthBar2, healthBar3,
                    healthBar4, healthBar5,
                    healthBar6, healthBar7,
                    healthBar8, healthBar9,
                    healthBar10;
    private Image spriteRenderer;
    private Dictionary<int, Sprite> healthDict = new();
    
    
    void Start()
    {
        spriteRenderer = GetComponent<Image>();
        healthDict[0] = healthBar0;
        healthDict[1] = healthBar1;
        healthDict[2] = healthBar2;
        healthDict[3] = healthBar3;
        healthDict[4] = healthBar4;
        healthDict[5] = healthBar5;
        healthDict[6] = healthBar6;
        healthDict[7] = healthBar7;
        healthDict[8] = healthBar8;
        healthDict[9] = healthBar9;
        healthDict[10] = healthBar10;
    }

    void Update()
    {
        spriteRenderer.sprite = healthDict[(int)PlayerController.maxHealth];
    } 
}
