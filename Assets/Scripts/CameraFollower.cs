using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Объект, за которым следует камера (ваш персонаж)
    public float smoothSpeed = 0.125f; // Плавность перемещения камеры
    public Vector3 offset; // Смещение камеры относительно персонажа

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}