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
    
    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField, Range(0.01f, 1f)] private float sprintPercentAvailability;
    [SerializeField] private float staminaDrainRate = 20f;
    [SerializeField] private float staminaRegenRate = 15f;
    [SerializeField] private float staminaRegenDelay = 1f;
    
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference lookAction;

    public static Transform Light;
    private Rigidbody2D rb;
    private Camera mainCamera;
    private Vector2 currentVelocity;
    
    private static float _currentStamina;
    private static float _maxStamina;
    private float lastSprintTime;
    private bool canSprint = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        _currentStamina = maxStamina;
        _maxStamina = maxStamina;
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
    
    private void FixedUpdate()
    {
        HandleStamina();
        var lookPosition = lookAction.action.ReadValue<Vector2>();
        Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(lookPosition);
        var direction = (mouseWorldPosition - (Vector2)transform.position).normalized;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        var targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        Light.transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
        var moveInput = moveAction.action.ReadValue<Vector2>();
        var isSprinting = sprintAction.action.IsPressed() && canSprint && moveInput.magnitude > 0.1f;
        var currentSpeed = isSprinting && _currentStamina > 0 ? sprintSpeed : moveSpeed;
        var targetVelocity = moveInput * currentSpeed;
        rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref currentVelocity, acceleration * Time.fixedDeltaTime);
    }
    
    private void HandleStamina()
    {
        var isSprinting = sprintAction.action.IsPressed() && moveAction.action.ReadValue<Vector2>().magnitude > 0.1f;
        if (isSprinting && canSprint)
        {
            _currentStamina -= staminaDrainRate * Time.deltaTime;
            lastSprintTime = Time.time;
            if (_currentStamina <= 0)
            {
                _currentStamina = 0;
                canSprint = false;
            }
        }
        else if (Time.time > lastSprintTime + staminaRegenDelay)
        {
            _currentStamina += staminaRegenRate * Time.deltaTime;
            _currentStamina = Mathf.Min(_currentStamina, maxStamina);
            if (_currentStamina >= maxStamina * sprintPercentAvailability)
            {
                canSprint = true;
            }
        }
    }

    public static float GetStaminaNormalized()
    {
        return _currentStamina / _maxStamina;
    }
}