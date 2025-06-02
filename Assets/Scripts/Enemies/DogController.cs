using System;
using System.Collections;
using Tooltips;
using UnityEngine;
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
    public static bool IsBite;
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
        if (PlayerController.IsCrouching())
        {
            distance = 5;
        }
        else
        {
            distance = 7;
        }
        if (Vector2.Distance(player.transform.position, transform.position) <= distance && canMove)
        {
            if (!PlayerController.IsSeeingEnemy)
            {
                PlayerController.IsSeeingEnemy = true;
                TooltipsSystem.Instance.ShowTooltip("ПКМ - усилить фонарик", 5);
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

        if (((Vector3)enemyRb.position - player.position).magnitude > 2f)
        {
            IsBite = false;
        }

        enemyAnimator.SetBool("IsWalking", isWalking);
        enemyAnimator.SetBool("IsRunning", isSprinting);
        enemyAnimator.SetBool("IsAtacking", IsBite);
    }

    private void RunTurn()
    {
        if (player)
        {
            transform.position =
                Vector2.MoveTowards(transform.position, player.position, enemyRunSpeed * Time.deltaTime);
            Vector2 direction = player.position - transform.position;
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
        transform.position = new Vector3 (
            transform.position.x - (player.transform.position.x - transform.position.x) * 0.8f,
            transform.position.y - (player.transform.position.y - transform.position.y) * 0.8f, 0);
        enemyRb.bodyType = RigidbodyType2D.Static;
        IsBite = true;
        yield return new WaitForSeconds(2f);

        enemyRb.bodyType = RigidbodyType2D.Dynamic;
        canMove = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            IsBite = true;
            StartCoroutine(BiteAndWait());
            audioSource.UnPause();
            audioSource.PlayOneShot(dogSounds[0]);
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
            canMove = false;
            isWalking = false;
            isSprinting = false;
        }

        if (!FlashlightController.IsFlashLightSuper)
        {
            canMove = true;
        }
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerLightTester"))
        {
            canMove = true;
        }
    }
}