using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Основные данные")]
    public string itemName; // Имя предмета (опционально)
    public int cost; // Стоимость предмета
    public Sprite icon; // Иконка для инвентаря
    public GameObject prefab; // Оригинальный префаб предмета
}