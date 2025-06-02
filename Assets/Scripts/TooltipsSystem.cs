using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TooltipsSystem : MonoBehaviour
{
    public static TooltipsSystem Instance;

    [SerializeField] private RectTransform tooltipPanel;
    [SerializeField] private TextMeshProUGUI tooltipText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowTooltip(string message, float duration = 3f)
    {
        tooltipText.text = message;
        
        var textWidth = tooltipText.preferredWidth;
        var textHeight = tooltipText.preferredHeight;
        
        tooltipPanel.sizeDelta = new Vector2(textWidth + 20, textHeight + 10);
        
        tooltipPanel.gameObject.SetActive(true);

        if (duration > 0)
            Invoke(nameof(HideTooltip), duration);
    }

    public void HideTooltip()
    {
        tooltipPanel.gameObject.SetActive(false);
    }
}
