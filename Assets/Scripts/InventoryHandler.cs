using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryHandler : MonoBehaviour
{
    public static Transform Target;
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private GameObject interactionHint;
    [SerializeField, Range(1, 5)] private int inventorySlotAmount;
    [SerializeField] private float pickupRadius = 1.5f;
    private GameObject[] inventorySlots;
    
    private int selectedSlot;
    private int previousSelectedSlot;
    
    private float lastPickupTime;
    private float lastSwapTime;

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
        inventorySlots[selectedSlot].GetComponent<Image>().color = Color.yellow;
    }

    private void Update()
    {
        HandlePickingUp();
        if (PlayerController.IsNextSlot())
            HandleSlotSelection();
    }

    private void HandlePickingUp()
    {
        var offset = new Vector3(0.8f, 0.8f, 0);
        var screenPos = Camera.main.WorldToScreenPoint(Target.position + offset);
        interactionHint.transform.position = screenPos;
        if (Time.time < lastPickupTime + 0.2f)
            return;
        var hitColliders = Physics2D.OverlapCircleAll(Target.position, pickupRadius);
        var isItemNotFound = true;
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Item"))
            {
                interactionHint.SetActive(true);
                isItemNotFound = false;
                if (PlayerController.TryPickUp())
                {
                    TryPickupItem(hitCollider.gameObject);
                    interactionHint.SetActive(false);
                    lastPickupTime = Time.time;
                }
                break;
            }
        }
        if (isItemNotFound)
            interactionHint.SetActive(false);
    }

    private void TryPickupItem(GameObject item)
    {
        var slotContent = inventorySlots[selectedSlot].transform.GetChild(0);
        if (slotContent.gameObject.activeSelf == false)
            PutItemInSlot(item, slotContent.gameObject);
    }

    private void PutItemInSlot(GameObject originalItem, GameObject slotContent)
    {
        if (slotContent.transform.childCount > 0) return;
        var itemUI = new GameObject("Item", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
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

    private void HandleSlotSelection()
    {
        if (Time.time < lastSwapTime + 0.2f)
            return;
        selectedSlot = (selectedSlot + 1) % inventorySlotAmount;
        inventorySlots[selectedSlot].GetComponent<Image>().color = Color.yellow;
        if (previousSelectedSlot != selectedSlot)
            inventorySlots[previousSelectedSlot].GetComponent<Image>().color = Color.white;
        previousSelectedSlot = selectedSlot;
        lastSwapTime = Time.time;
    }
}