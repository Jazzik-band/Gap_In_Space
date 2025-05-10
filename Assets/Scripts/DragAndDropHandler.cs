using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDropHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Tooltip("Иконка предмета в слоте")]
    public Image icon; // Иконка предмета

    [Tooltip("Текст стоимости предмета")]
    public Text costText; // Текст стоимости

    [Tooltip("Префаб оригинального предмета (для выброса)")]
    public GameObject itemPrefab; // Префаб предмета (опционально)

    private CanvasGroup canvasGroup; // Для блокировки рэйкаста во время перетаскивания
    private Transform originalParent; // Оригинальный слот
    private GameObject dragGhost; // Призрачный объект для перетаскивания

    private void Start()
    {
        // Получаем CanvasGroup, если он не назначен в инспекторе
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (icon.sprite == null) return;

        // Уничтожаем старый dragGhost
        if (dragGhost != null)
            DestroyImmediate(dragGhost);

        // Создаём новый dragGhost
        dragGhost = new GameObject("DragGhost", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(CanvasGroup));
        dragGhost.transform.SetParent(transform.root);

        // Настройка RectTransform
        var ghostRect = dragGhost.GetComponent<RectTransform>();
        ghostRect.sizeDelta = new Vector2(64, 64);
        ghostRect.pivot = new Vector2(0.5f, 0.5f);
        ghostRect.anchorMin = ghostRect.anchorMax = new Vector2(0.5f, 0.5f);

        // Настройка Image
        var ghostImage = dragGhost.GetComponent<Image>();
        ghostImage.sprite = icon.sprite;
        ghostImage.color = Color.white;

        // Настройка CanvasGroup
        var ghostCanvasGroup = dragGhost.GetComponent<CanvasGroup>();
        ghostCanvasGroup.blocksRaycasts = false;

        // Блокируем рэйкаст текущего слота
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragGhost != null)
        {
            dragGhost.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragGhost != null)
            Destroy(dragGhost);

        // Используем GraphicRaycaster для поиска UI-объектов
        var pointer = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);

        foreach (var result in raycastResults)
        {
            // Проверяем, есть ли DragAndDropHandler на целевом объекте
            if (result.gameObject.TryGetComponent<DragAndDropHandler>(out var targetSlot))
            {
                Debug.Log($"[OnEndDrag] Целевой слот: {targetSlot.gameObject.name}");

                // Перетаскивание в заполненный слот
                if (targetSlot.icon.sprite != null)
                {
                    SwapItems(targetSlot);
                }
                // Перетаскивание в пустой слот
                else
                {
                    MoveItemToEmptySlot(targetSlot);
                }

                break;
            }
        }

        // Восстанавливаем рэйкаст
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// Обмен предметами между слотами
    /// </summary>
    private void SwapItems(DragAndDropHandler targetSlot)
    {
        targetSlot.icon.sprite = icon.sprite;
        targetSlot.icon.color = Color.white;

        targetSlot.costText.text = costText.text;
        targetSlot.costText.color = new Color(0, 1, 0, 1); // Зелёный, непрозрачный

        icon.sprite = null;
        icon.color = new Color(0, 0, 0, 0); // Прозрачный

        costText.text = "";
        costText.color = new Color(0, 1, 0, 0); // Прозрачный
    }

    /// <summary>
    /// Перемещение предмета в пустой слот
    /// </summary>
    private void MoveItemToEmptySlot(DragAndDropHandler targetSlot)
    {
        // Перемещаем предмет в пустой слот
        targetSlot.icon.sprite = icon.sprite;
        targetSlot.icon.color = Color.white; // Делаем иконку видимой

        targetSlot.costText.text = costText.text;
        targetSlot.costText.color = new Color(0, 1, 0, 1); // Делаем текст видимым

        targetSlot.itemPrefab = itemPrefab; // Передаём префаб (если используется)

        // Очищаем текущий слот
        icon.sprite = null;
        icon.color = new Color(0, 0, 0, 0); // Прозрачный цвет

        costText.text = "";
        costText.color = new Color(0, 1, 0, 0); // Прозрачный текст

        itemPrefab = null; // Очищаем ссылку
    }
}