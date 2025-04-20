using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float crouchSpeed = 2f;
    
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 10f;
    
    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField, Range(0.01f, 1f)] private float sprintPercentAvailability;
    [SerializeField] private float staminaDrainRate = 20f;
    [SerializeField] private float staminaRegenRate = 15f;
    [SerializeField] private float staminaRegenDelay = 1f;
    
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference crouchAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private InputActionReference nextSlotAction;
    
    private Rigidbody2D rb;
    private Camera mainCamera;
    private Vector2 currentVelocity;

    private const float RotationSpeed = 15f;
    private const float Acceleration = 10f;
    private static float _currentStamina;
    private static float _maxStamina;
    private float lastSprintTime;
    private bool canSprint = true;
    
    private static bool _isCrouching;
    private static bool _isPickingUp;
    private static bool _isNextSlotPicking;

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
        _isPickingUp = interactAction.action.IsPressed();
        _isNextSlotPicking = nextSlotAction.action.IsPressed();
        var lookPosition = lookAction.action.ReadValue<Vector2>();
        Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(lookPosition);
        var direction = (mouseWorldPosition - (Vector2)transform.position).normalized;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        var targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
        var moveInput = moveAction.action.ReadValue<Vector2>();
        var isSprinting = sprintAction.action.IsPressed() && canSprint && moveInput.magnitude > 0.1f;
        _isCrouching = crouchAction.action.IsPressed();
        var currentSpeed =  moveSpeed;
        if (isSprinting && !_isCrouching && _currentStamina > 0)
            currentSpeed = sprintSpeed;
        if (_isCrouching && !isSprinting)
            currentSpeed = crouchSpeed;
        var targetVelocity = moveInput * currentSpeed;
        rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref currentVelocity, Acceleration * Time.fixedDeltaTime);
        if (maxHealth <= 0)
        {
            gameObject.transform.GetChild(0).GetComponent<Light2D>().enabled = false;
            gameObject.transform.GetChild(1).GetComponent<Light2D>().enabled = false;
        }
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

    public static bool IsCrouching()
    {
        return _isCrouching;
    }

    public static bool TryPickUp()
    {
        return _isPickingUp;
    }

    public static bool IsNextSlot()
    {
        return _isNextSlotPicking;
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            maxHealth -= 1;
        }
    }
}