using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    public FlashlightController flashlight;
    public Text batteryText;

    void Update()
    {
        // после запуска сцены объект PlayerLight добавляется в компоненты
        flashlight = GameObject.Find("PlayerLight").GetComponent<FlashlightController>();
        batteryText.text = Mathf.Round(flashlight.batteryLife) + "%";
    }
}
