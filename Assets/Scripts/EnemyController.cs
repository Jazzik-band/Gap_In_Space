using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float radius = 4f;
    [SerializeField] public float enemyWalkSpeed;
    [SerializeField] public float enemyRunSpeed;
    [SerializeField] public float enemyReturnSpeed;
    [SerializeField] public float chaseDistance = 5f;
    [SerializeField] private float minWaitTime = 1f;
    [SerializeField] private float maxWaitTime = 3f;
    public float rotationSpeed = 5f;
    private Vector2 targetPosition;
    private Vector2 centerPoint;
    private float timer;

    private float waitTime;

    // private bool isChasing;
    private bool wasChasing;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        centerPoint = transform.position;
        SetNewRandomTarget();
    }

    private void Update()
    {
        if (Vector2.Distance(player.transform.position, transform.position) <= 7)
        {
            RunTurn();
            // isChasing = true;
            wasChasing = true;
        }
        else if (Vector2.Distance(player.transform.position, transform.position) > 7 && wasChasing)
        {
            // isChasing = false;
            wasChasing = false;
            ReturnToSpawn();
        }
        else
        {
            WalkTurn();
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
            Vector2.MoveTowards(transform.position, targetPosition, enemyWalkSpeed * Time.deltaTime);
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
}