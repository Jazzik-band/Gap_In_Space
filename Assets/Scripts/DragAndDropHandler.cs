using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDropHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image icon;
    public Text costText; 

    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private GameObject dragGhost;
    public GameObject itemPrefab; // 🔥 Ссылка на оригинальный префаб предмета
    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (icon.sprite == null) return; 


        dragGhost = new GameObject("DragGhost", typeof(Image));
        dragGhost.transform.SetParent(transform.root);
        dragGhost.GetComponent<Image>().sprite = icon.sprite;
        dragGhost.GetComponent<RectTransform>().sizeDelta = icon.rectTransform.sizeDelta;


        var ghostCanvasGroup = dragGhost.AddComponent<CanvasGroup>();
        ghostCanvasGroup.blocksRaycasts = false;

        originalParent = transform.parent;
        canvasGroup.blocksRaycasts = false; 
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (dragGhost != null)
            dragGhost.transform.position = Input.mousePosition;
    }

 
    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragGhost != null)
        {
            Destroy(dragGhost);
        }


        RaycastHit2D hit = Physics2D.Raycast(Input.mousePosition, Vector2.zero);
        if (hit.collider != null && hit.collider.TryGetComponent<DragAndDropHandler>(out var targetSlot))
        {

            if (targetSlot.icon.sprite != null)
            {
                SwapItems(targetSlot);
            }
            else
            {

                MoveItemToEmptySlot(targetSlot);
            }
        }


        canvasGroup.blocksRaycasts = true;
    }


    private void SwapItems(DragAndDropHandler targetSlot)
    {

        Sprite tempSprite = icon.sprite;
        icon.sprite = targetSlot.icon.sprite;
        targetSlot.icon.sprite = tempSprite;


        string tempCost = costText.text;
        costText.text = targetSlot.costText.text;
        targetSlot.costText.text = tempCost;
    }
    private void MoveItemToEmptySlot(DragAndDropHandler targetSlot)
    {

        targetSlot.icon.sprite = icon.sprite;
        targetSlot.costText.text = costText.text;


        icon.sprite = null;
        costText.text = "";
    }
    
}