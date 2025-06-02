using UnityEngine;
using UnityEngine.UI;

public class CheatCodeManager : MonoBehaviour
{
    [SerializeField] private GameObject cheatPanel;
    [SerializeField] private InputField cheatInputField;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F8))
        {
            cheatPanel.SetActive(!cheatPanel.activeSelf);
        }
    }
    
    public void OnCheatCodeSubmitted()
    {
        string cheatCode = cheatInputField.text.Trim().ToLower();
        
        switch (cheatCode)
        {
            case "lotsofbattery":
                IncreaseBatteryLife();
                break;
            case "lotsofhealth":
                IncreaseHealth();
                break;
        }

        cheatInputField.text = "";
        cheatPanel.SetActive(false);
    }

    private void IncreaseBatteryLife()
    {
        FlashlightController.BatteryLife = 1000f;
    }
    
    private void IncreaseHealth()
    {
        PlayerController.maxHealth = 1000f;
    }
}