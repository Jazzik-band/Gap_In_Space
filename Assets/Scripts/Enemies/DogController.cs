using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Timeline;
using Random = UnityEngine.Random;

public class DogController : MonoBehaviour
{
    [SerializeField] private float radius = 4f;
    [SerializeField] public float enemyWalkSpeed;
    [SerializeField] public float enemyRunSpeed;
    [SerializeField] public float enemyReturnSpeed;
    [SerializeField] public float chaseDistance = 5f;
    [SerializeField] private float minWaitTime = 1f;
    [SerializeField] private float maxWaitTime = 3f;
    [SerializeField] private float distance = 7;
    public float rotationSpeed = 5f;
    public Rigidbody2D enemyRb;
    public float delayAfterBite = 1;
    private Vector2 savedVelocity;
    private Vector2 targetPosition;
    private Vector2 centerPoint;
    private float timer;
    private Vector2 currentVelocity;
    public AudioClip[] dogSounds;
    private AudioSource audioSource;
    
    public Animator enemyAnimator;
    
    private float waitTime;
    private bool isStopped = false;

    private bool wasChasing;
    public static bool isBite;
    private bool canMove = true;
    private bool isWalking;
    private bool isSprinting;
    private Transform player;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyAnimator = GetComponent<Animator>();
        centerPoint = transform.position;
        enemyRb = GetComponent<Rigidbody2D>();
        currentVelocity = enemyRb.linearVelocity;
        SetNewRandomTarget();
    }

    private void Update()
    {
        if (PlayerController.IsCrouching() || !FlashlightController.IsFlashLightOn)
        {
            distance = 5;
        }
        else
        {
            distance = 7;
        }
        if (Vector2.Distance(player.transform.position, transform.position) <= distance && canMove)
        {
            if (!PlayerController.IsSeeingEnemy && SceneManager.GetActiveScene().name == "Education")
            {
                PlayerController.IsSeeingEnemy = true;
                TooltipsSystem.Instance.ShowTooltip(
                    "ПКМ - усилить фонарик\nСледите за запасом энергии!", 7);
            }
            RunTurn();
            wasChasing = true;
            isWalking = false;
            isSprinting = true;
        }
        else if (Vector2.Distance(player.transform.position, transform.position) > 7 && wasChasing && canMove)
        {
            wasChasing = false;
            isSprinting = false;
            isWalking = true;
            ReturnToSpawn();
        }
        else
        {
            if (canMove)
            {
                WalkTurn();
                isSprinting = false;
            }
        }

        // if (((Vector3)enemyRb.position - player.position).magnitude > 2f)
        // {
        //     isBite = false;
        // }

        enemyAnimator.SetBool("IsWalking", isWalking);
        enemyAnimator.SetBool("IsRunning", isSprinting);
        enemyAnimator.SetBool("IsAtacking", isBite);
    }

    private void RunTurn()
    {
        if (player && canMove)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            enemyRb.linearVelocity = direction * enemyRunSpeed;
        
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private void WalkTurn()
    {
        transform.position =
            Vector2.MoveTowards(
                transform.position, targetPosition, enemyWalkSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            isWalking = false;
            timer += Time.deltaTime;
            if (timer >= waitTime)
            {
                SetNewRandomTarget();
                timer = 0f;
            }
        }

        Vector2 direction = targetPosition - (Vector2)transform.position;
        if (direction.magnitude > 0.01f)
        {
            isWalking = true;
            var targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var targetRotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void SetNewRandomTarget()
    {
        var randomAngle = Random.Range(0f, Mathf.PI * 2f);
        var randomRadius = Random.Range(0f, radius);
        targetPosition = centerPoint + new Vector2(
            Mathf.Cos(randomAngle) * randomRadius,
            Mathf.Sin(randomAngle) * randomRadius
        );
        waitTime = Random.Range(minWaitTime, maxWaitTime);
    }

    private void ReturnToSpawn()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            centerPoint,
            enemyReturnSpeed * Time.deltaTime
        );
    }
    
    private void DogStepSoundPlay()
    {
        if (Vector2.Distance(transform.position, player.transform.position) <= 8)
        {
            audioSource.volume = 0.1f;
            audioSource.UnPause();
            audioSource.PlayOneShot(dogSounds[1]);
        }
        else
        {
            audioSource.volume = 0.5f;
            audioSource.Pause();
        }
    }
    
    private IEnumerator BiteAndWait()
    {
        canMove = false;
        isBite = true;
    
        savedVelocity = enemyRb.linearVelocity;
        enemyRb.linearVelocity = Vector2.zero;
        enemyRb.bodyType = RigidbodyType2D.Kinematic;
    
        yield return new WaitForSeconds(2f);
    
        enemyRb.bodyType = RigidbodyType2D.Dynamic;
        canMove = true;
        isBite = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isBite = true;
            StartCoroutine(BiteAndWait());
            audioSource.UnPause();
            audioSource.PlayOneShot(dogSounds[0]);
            PlayerController.maxHealth -= 1f;
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                Vector2 bounceDirection = (other.transform.position - transform.position).normalized;
                playerController.ApplyBounce(bounceDirection);
            }
        }
        else
        {
            audioSource.Pause();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerLightTester") && FlashlightController.IsFlashLightSuper)
        {
            enemyRb.bodyType = RigidbodyType2D.Static;
            canMove = false;
            isWalking = false;
            isSprinting = false;
        }

        if (!FlashlightController.IsFlashLightSuper)
        {
            enemyRb.bodyType = RigidbodyType2D.Dynamic;
            canMove = true;
        }
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerLightTester"))
        {
            enemyRb.bodyType = RigidbodyType2D.Dynamic;
            canMove = true;
        }
    }
}