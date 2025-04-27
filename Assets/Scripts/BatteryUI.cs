using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    public FlashlightController flashlight;
    public Text batteryText;
    void Update()
    {
        batteryText.text = Mathf.Round(flashlight.batteryLife) + "%";
    }
}
