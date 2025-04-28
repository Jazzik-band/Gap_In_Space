using UnityEngine;
using TMPro;

public class ItemTooltip : MonoBehaviour
{
    public TMP_Text tooltipText;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            tooltipText.text = tooltipText.text;
            tooltipText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            tooltipText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        tooltipText.transform.position = screenPosition + new Vector3(0, 30, 0);
    }
}