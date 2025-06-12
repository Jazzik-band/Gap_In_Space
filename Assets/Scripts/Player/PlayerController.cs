using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float crouchSpeed = 2f;
    
    [Header("Health Settings")]
    public static float maxHealth = 10f;
    
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
    
    public static Rigidbody2D rb;
    private Camera mainCamera;
    private Vector2 currentVelocity;
    
    private SpriteRenderer spriteRenderer;
    public Sprite playerSpriteWithoutFlashlight;
    private const float RotationSpeed = 15f;
    private const float Acceleration = 10f;
    private static float _currentStamina;
    private static float _maxStamina;
    private float lastSprintTime;
    private bool canSprint = true;
    public static bool IsBoosted;
    private float boostTimeLeft = 7f;
    private Light2D playerLight, roundLight;
    public Light2D mainLight;
    public GameObject door;
    public Light2D[] lamps;
    public float delayBeforeLoad = 2f;

    public AnimatorOverrideController playerFlashlightAnimator;
    public Animator playerAnimator;
    public AudioClip[] stepSounds;
    private AudioSource audioSource;

    public static bool IsSeeingEnemy, IsSeeingItems,
        IsPickingUpItemEarly, IsSeeingOrbs, IsCollectedOrbs;
    public bool isShown;
    private bool isTriggered;
    
    
    private static bool _isWalking;
    private static bool _isCrouching;
    private static bool _isPickingUp;
    private static bool _isNextSlotPicking;
    public static bool isDieOnLearning;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Hub")
        {
            maxHealth = 10f;
            IsBoosted = false;
            OrbHandler.CurrentOrbs = 0;
        }
        else
        {
            playerAnimator.runtimeAnimatorController = playerFlashlightAnimator;
        }

        if (SceneManager.GetActiveScene().name == "Education")
        {
            TooltipsSystem.Instance.ShowTooltip(
                "WASD - движение\nL. Shift - ускорение\nL. Ctrl - присесть\nF - вкл/выкл фонарик", 10);
        }
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerLight = GetComponentInChildren<Light2D>();
        playerAnimator = GetComponent<Animator>();
        if (roundLight == null)
        {
            Transform lightTransform = transform.Find("RoundLight");
            if (lightTransform != null)
                roundLight = lightTransform.GetComponent<Light2D>();
        }
        FixedUpdate();
    }
    
    private void StepSoundPlay()
    {
        audioSource.volume = 1f;
        audioSource.PlayOneShot(stepSounds[Random.Range(0, stepSounds.Length)]);
    }
        
    private void QuietStepSoundPlay()
    {
        audioSource.volume = 0.5f;
        audioSource.PlayOneShot(stepSounds[Random.Range(0, stepSounds.Length)]);
    }
    
    

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
        var currentScene = SceneManager.GetActiveScene().name;
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
        _isWalking = moveInput.magnitude > 0.1f && !isSprinting && !_isCrouching;
        
        playerAnimator.SetBool("IsWalking", _isWalking);
        playerAnimator.SetBool("IsRunning", isSprinting);
        playerAnimator.SetBool("IsCrouching", _isCrouching);
        
        var currentSpeed = moveSpeed;
        if (isSprinting && !_isCrouching && _currentStamina > 0)
            currentSpeed = sprintSpeed;
        if (_isCrouching && !isSprinting)
            currentSpeed = crouchSpeed;
        
        var targetVelocity = moveInput * currentSpeed;
        rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref currentVelocity, Acceleration * Time.fixedDeltaTime);
        if (maxHealth < 0.5f)
        {
            if (currentScene == "Education")
                isDieOnLearning = true;
            SceneManager.LoadScene("Death");
        }

        if (currentScene == "Hub")
        {
            HandleStartScene();
        }
        if (currentScene == "Education")
        {
            CameraFollower.Target = transform;
            InventoryHandler.Target = transform;
            if (transform.position.y > 22 && !IsSeeingItems)
            {
                IsSeeingItems = true;
                TooltipsSystem.Instance.ShowTooltip(
                    "Tab - сменить ячейку инвентаря\nE - взять предмет", 5);
            }
            
            if (transform.position.y > 60 && !IsSeeingOrbs)
            {
                IsSeeingOrbs = true;
                TooltipsSystem.Instance.ShowTooltip(
                    "Соберите необходимое\nколичество сфер,\nчтобы открыть портал", 7);
            }

            if (OrbHandler.CurrentOrbs == 4 && !IsCollectedOrbs)
            {
                IsCollectedOrbs = true;
                TooltipsSystem.Instance.ShowTooltip(
                    "Стрелка указывает на портал\nИдите скорее!", 7);
            }
        }
    }

    private void HandleStartScene()
    {
        spriteRenderer.sprite = playerSpriteWithoutFlashlight;
        CameraFollower.Target = transform;
        playerLight.gameObject.SetActive(false);
        roundLight.gameObject.SetActive(false);

        if (transform.position.y >= 65)
        {
            isTriggered = true;
            if (isTriggered)
            {
                for (int i = 0; i < lamps.Length; i++)
                {
                    lamps[i].gameObject.SetActive(false);
                }
                roundLight.gameObject.SetActive(true);
                StartCoroutine(LoadSceneAfterDelay());
            }
        }

        if (isTriggered && transform.position.y < 65)
        {
            roundLight.gameObject.SetActive(true);
        }
        if (!isShown && transform.position.y >= 47.5f)
        {
            door.SetActive(true);
            isShown = true;
            mainLight.intensity = 0f;
            for (int j = 0; j < lamps.Length; j++)
            {
                lamps[j].gameObject.SetActive(true);
            }
        }
    }
    private void HandleStamina()
    {
        var isSprinting = sprintAction.action.IsPressed() && moveAction.action.ReadValue<Vector2>().magnitude > 0.1f;
        if (IsBoosted)
        {
            boostTimeLeft -= Time.deltaTime;
            if (boostTimeLeft <= 0)
            {
                IsBoosted = false;
                boostTimeLeft = 7;
            }
            _currentStamina += staminaRegenRate * Time.deltaTime;
            _currentStamina = Mathf.Min(_currentStamina, maxStamina);
            canSprint = true;
        }
        else if (isSprinting && canSprint)
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
            DogController.isBite = true;
        }
    }
    
    public void ApplyBounce(Vector2 direction)
    {
        if (rb != null)
        {
            float bounceForce = 9f;
            rb.AddForce(direction * bounceForce, ForceMode2D.Impulse);
        }
    }
    
    IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        SceneManager.LoadScene("Game");
    }
}