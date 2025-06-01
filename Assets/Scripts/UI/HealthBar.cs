using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Sprite[] healthSprites;
    private Image healthImage;
    private float lastHealth;

    void Start()
    {
        healthImage = GetComponent<Image>();
        UpdateHealthBar(PlayerController.maxHealth);
    }

    void Update()
    {
        if (PlayerController.maxHealth != lastHealth)
        {
            UpdateHealthBar(PlayerController.maxHealth);
            lastHealth = PlayerController.maxHealth;
        }
    }

    private void UpdateHealthBar(float health)
    {
        int index = Mathf.Clamp(Mathf.FloorToInt(health), 0, healthSprites.Length - 1);
        healthImage.sprite = healthSprites[index];
    }
}