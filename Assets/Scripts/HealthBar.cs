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
    
    
    void Start()
    {
        spriteRenderer = GetComponent<Image>();
    }

    void Update()
    {
        switch (PlayerController.maxHealth)
        {
            case 10: spriteRenderer.sprite = healthBar10; break;
            case 9: spriteRenderer.sprite = healthBar9; break;
            case 8: spriteRenderer.sprite = healthBar8; break;
            case 7: spriteRenderer.sprite = healthBar7; break;
            case 6: spriteRenderer.sprite = healthBar6; break;
            case 5: spriteRenderer.sprite = healthBar5; break;
            case 4: spriteRenderer.sprite = healthBar4; break;
            case 3: spriteRenderer.sprite = healthBar3; break;
            case 2: spriteRenderer.sprite = healthBar2; break;
            case 1: spriteRenderer.sprite = healthBar1; break;
            case <= 0: spriteRenderer.sprite = healthBar0; break;
        }
    }
}
