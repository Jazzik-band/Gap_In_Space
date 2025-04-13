using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public static Transform Target;
    private const float SmoothSpeed = 0.05f;
    private readonly Vector3 baseOffset = new Vector3(0, 0, -10);
    private const float MaxCursorDistance = 3f;
    private const float CursorInfluence = 0.5f;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
    }
    
    private void LateUpdate()
    {
        var cursorWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        cursorWorldPos.z = 0;
        var playerToCursor = cursorWorldPos - Target.position;
        var cursorDistance = playerToCursor.magnitude;
        var cursorDirection = playerToCursor.normalized;
        var dynamicOffsetAmount = Mathf.Min(cursorDistance * CursorInfluence, MaxCursorDistance);
        var cursorOffset = cursorDirection * dynamicOffsetAmount;
        var desiredPosition = Target.position + baseOffset + cursorOffset;
        var smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, SmoothSpeed);
        transform.position = smoothedPosition;
    }
}