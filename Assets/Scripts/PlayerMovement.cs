using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float rotationSpeed = 15f;
    
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference lookAction;

    private Rigidbody2D rb;
    private Camera mainCamera;
    private Vector2 currentVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        sprintAction.action.Enable();
        lookAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        sprintAction.action.Disable();
        lookAction.action.Disable();
    }

    [Obsolete("Obsolete")]
    private void FixedUpdate()
    {
        var lookPosition = lookAction.action.ReadValue<Vector2>();
        Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(lookPosition);
        var direction = (mouseWorldPosition - (Vector2)transform.position).normalized;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        var targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            targetRotation, 
            rotationSpeed * Time.deltaTime
        );
        var moveInput = moveAction.action.ReadValue<Vector2>();
        var isSprinting = sprintAction.action.IsPressed();
        var currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
        var targetVelocity = moveInput * currentSpeed;
        rb.velocity = Vector2.SmoothDamp(
            rb.velocity, 
            targetVelocity, 
            ref currentVelocity, 
            acceleration * Time.fixedDeltaTime
        );
    }
}