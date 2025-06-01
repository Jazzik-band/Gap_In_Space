using UnityEngine;
using UnityEngine.UI;

public class SimpleTooltip : MonoBehaviour
{
    [Header("Настройки времени")]
    [SerializeField] private float showDelay = 1f;
    [SerializeField] private float hideDelay = 3f;

    private void Start()
    {
        gameObject.SetActive(false);
        Invoke("ShowTooltip", showDelay);
    }

    private void ShowTooltip()
    {
        gameObject.SetActive(true);
        Invoke("HideTooltip", hideDelay);
    }

    private void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}