using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Timeline;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
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
    
    private float waitTime;
    private bool isStopped = false;

    // private bool isChasing;
    private bool wasChasing;
    private bool isBite = false;
    private bool canMove = true;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
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
            RunTurn();
            // isChasing = true;
            wasChasing = true;
        }
        else if (Vector2.Distance(player.transform.position, transform.position) > 7 && wasChasing && canMove)
        {
            // isChasing = false;
            wasChasing = false;
            ReturnToSpawn();
        }
        else
        {
            if (canMove)
            {
                WalkTurn();
            }
        }
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
    
    private IEnumerator BiteAndWait()
    {
        isBite = true;
        
        canMove = false;
        transform.position = new Vector3 (transform.position.x - (player.transform.position.x - transform.position.x), transform.position.y - (player.transform.position.y - transform.position.y), 0);
        enemyRb.bodyType = RigidbodyType2D.Static;
        yield return new WaitForSeconds(1f);
        canMove = true;
        
        isBite = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(BiteAndWait());
        }
    }

    // public void Freeze()
    // {
    //     canMove = false;
    //     enemyRb.linearVelocity = Vector3.zero;
    // }
    //
    // public void Unfreeze()
    // {
    //     canMove = true;
    //     enemyRb.linearVelocity = currentVelocity;
    // }
}