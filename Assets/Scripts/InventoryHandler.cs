using UnityEngine;
using UnityEngine.UI;

public class InventoryHandler : MonoBehaviour
{
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField, Range(1, 5)] private int inventorySlotAmount;
    [SerializeField] private LayerMask itemsLayerMask;
    [SerializeField] private float pickupRadius = 1.5f;
    private GameObject[] inventorySlots;
    private GameObject selectedSlot;
    public static Transform Target;

    private void Start()
    {
        inventorySlots = new GameObject[inventorySlotAmount];
        for (var i = 0; i < inventorySlotAmount; i++)
        {
            var slot = Instantiate(inventorySlotPrefab, transform);
            inventorySlots[inventorySlotAmount - 1 - i] = slot;
            slot.transform.localPosition = new Vector3(-120 * i, 0, 0);
            var slotContent = new GameObject("SlotContent", typeof(RectTransform), typeof(CanvasRenderer))
                {
                    layer = LayerMask.NameToLayer("UI")
                };
            var rectTransform = slotContent.GetComponent<RectTransform>();
            rectTransform.SetParent(slot.transform);
            rectTransform.localPosition = Vector3.zero;
            slotContent.SetActive(false);
        }
        selectedSlot = inventorySlots[0];
    }

    private void Update()
    {
        if (!Target) return;
        var hitColliders = Physics2D.OverlapCircleAll(Target.position, pickupRadius, itemsLayerMask);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Item"))
            {
                TryPickupItem(hitCollider.gameObject);
            }
        }
    }
    
    private void TryPickupItem(GameObject item)
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.transform.childCount > 0)
            {
                var slotContent = slot.transform.GetChild(0);
                if (!slotContent.gameObject.activeInHierarchy)
                {
                    PutItemInSlot(item, slotContent.gameObject);
                    return;
                }
            }
        }
    }

    private void PutItemInSlot(GameObject originalItem, GameObject slotContent)
    {
        var itemUI = new GameObject("ItemUI", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        itemUI.transform.SetParent(slotContent.transform, false);
        var spriteRenderer = originalItem.GetComponent<SpriteRenderer>();
        var imageComponent = itemUI.GetComponent<Image>();
        if (spriteRenderer)
        {
            imageComponent.sprite = spriteRenderer.sprite;
            imageComponent.rectTransform.transform.localScale = new Vector3(0.6f, 0.6f, 0);
        }
        slotContent.SetActive(true);
        Destroy(originalItem);
    }
}