using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleTopDownMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float smoothTime = 0.1f;

    private Vector2 moveInput;
    private Vector2 currentVelocity;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector2 targetVelocity = moveInput * moveSpeed;
        rb.linearVelocity = Vector2.SmoothDamp(
            rb.linearVelocity,
            targetVelocity,
            ref currentVelocity,
            smoothTime
        );
    }
}