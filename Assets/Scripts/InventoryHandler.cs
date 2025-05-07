using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryHandler : MonoBehaviour
{
    public static Transform Target;
    public TextMeshProUGUI costText;
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private GameObject interactionHint;
    [SerializeField, Range(1, 5)] private int inventorySlotAmount;
    [SerializeField] private float pickupRadius = 1.5f;
    private GameObject[] inventorySlots;
    
    private int selectedSlot;
    private int previousSelectedSlot;
    private float lastPickupTime;
    private float lastSwapTime;

    public Transform playerTransform; // Точка появления предмета (например, рядом с игроком)

    // Метод для выброса предмета
    public void DropItemFromSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventorySlotAmount) return;

        Transform slotContent = inventorySlots[slotIndex].transform.GetChild(0);
        var dragHandler = slotContent.GetComponent<DragAndDropHandler>();
        Debug.Log($"[DropItemFromSlot] Префаб в слоте: {dragHandler.itemPrefab?.name}");
        if (dragHandler.icon.sprite == null || dragHandler.itemPrefab == null)
        {
            Debug.Log("Слот пустой или префаб не назначен");
            return;
        }

        if (playerTransform == null)
        {
            Debug.LogError("playerTransform не назначен!");
            return;
        }

        // Получаем вектор направления игрока (с учётом поворота и отражения)
        Vector2 forward = playerTransform.right; // right — вектор направления вправо
        float scale = playerTransform.lossyScale.x; // Учитываем отражение по оси X
        forward = new Vector2(forward.x * Mathf.Sign(scale), forward.y); // Корректируем направление

        // Рассчитываем позицию перед игроком
        Vector3 dropPosition = playerTransform.position 
                               + (Vector3)forward * 1.5f // Смещение в направлении взгляда
                               + Vector3.up * 0.5f; // Поднятие над землёй

        dropPosition = new Vector3(dropPosition.x, dropPosition.y, 0f); // Фиксируем Z

        // Создаем предмет
        GameObject droppedItem = Instantiate(dragHandler.itemPrefab, dropPosition, Quaternion.identity);

        if (dragHandler.icon != null)
        {
            dragHandler.icon.sprite = null;
            dragHandler.icon.color = Color.clear;
        }

        if (dragHandler.costText != null)
        {
            dragHandler.costText.text = "";
        }

        if (dragHandler.itemPrefab != null)
        {
            dragHandler.itemPrefab = null;
        }

        // Деактивируем слот
        slotContent.gameObject.SetActive(false);
        
        Debug.Log($"[DropItemFromSlot] Слот {slotIndex} очищен");
        // Отладка
        Debug.Log($"[DropItemFromSlot] Направление игрока: {forward}, позиция: {dropPosition}");
    }
    
    private void Start()
    {
        inventorySlots = new GameObject[inventorySlotAmount];
        for (var i = 0; i < inventorySlotAmount; i++)
        {
            var slot = Instantiate(inventorySlotPrefab, transform);
            inventorySlots[inventorySlotAmount - 1 - i] = slot;
            slot.transform.localPosition = new Vector3(-120 * i, 0, 0);
            var slotContent = new GameObject("SlotContent", typeof(RectTransform), typeof(CanvasRenderer), typeof(CanvasGroup))
                {
                    layer = LayerMask.NameToLayer("UI")
                };
            var rectTransform = slotContent.GetComponent<RectTransform>();
            rectTransform.SetParent(slot.transform);
            rectTransform.localPosition = Vector3.zero;
            
            var collider = slotContent.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(64, 64); 
            
            slotContent.AddComponent<DragAndDropHandler>();
            
            slotContent.SetActive(false);
        }
        inventorySlots[selectedSlot].GetComponent<Image>().color = Color.yellow;
    }

    private void Update()
    {
        HandlePickingUp();
        if (PlayerController.IsNextSlot())
            HandleSlotSelection();
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            DropItemFromSlot(selectedSlot);
        }
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
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
        if (slotContent.transform.childCount > 0 && slotContent.GameObject().activeSelf)
        {
            Debug.LogWarning("Слот уже содержит предмет");
            return;
        }
        slotContent.SetActive(true);
        var itemUI = new GameObject("Item", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        itemUI.transform.SetParent(slotContent.transform, false);
        var spriteRenderer = originalItem.GetComponent<SpriteRenderer>();
        var imageComponent = itemUI.GetComponent<Image>();
        
        if (spriteRenderer)
        {
            imageComponent.sprite = spriteRenderer.sprite;
            imageComponent.rectTransform.transform.localScale = new Vector3(0.6f, 0.6f, 0);
        }
        
        Item item = originalItem.GetComponent<Item>();
        Text costText = slotContent.GetComponentInChildren<Text>();
        if (costText == null)
        {
            GameObject textObj = new GameObject("Cost", typeof(Text));
            costText = textObj.GetComponent<Text>();
            costText.transform.SetParent(slotContent.transform);
            costText.rectTransform.anchoredPosition = new Vector2(5, -40);
        }
        costText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        costText.rectTransform.sizeDelta = new Vector2(40, 15);
        costText.text = $"{item.data.cost.ToString()}$";
        costText.fontSize = 10;
        costText.color = Color.green;
        
        var dragHandler = slotContent.GetComponent<DragAndDropHandler>();
        if (dragHandler == null)
        {
            dragHandler = slotContent.AddComponent<DragAndDropHandler>();
        }
        dragHandler.icon = imageComponent;
        dragHandler.costText = costText;
        if (item == null || item.data.prefab == null)
        {
            Debug.LogError($"[PutItemInSlot] Item или prefab == null! {originalItem.name}");
            return;
        }

        // 🔥 Сохраните ссылку на оригинальный префаб, а не на экземпляр
        dragHandler.itemPrefab = item.data.prefab; // Используем prefab из Item
        Debug.Log($"[PutItemInSlot] Icon.Sprite: {dragHandler.icon.sprite?.name}");
        Debug.Log($"[PutItemInSlot] Префаб предмета: {item.data.prefab.name}");
        slotContent.SetActive(true);
        Destroy(originalItem);
        Debug.Log($"[PutItemInSlot] Префаб предмета: {item.data.prefab.name}");
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