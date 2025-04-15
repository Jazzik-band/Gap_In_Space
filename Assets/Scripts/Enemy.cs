using UnityEngine;

public class Enemy: MonoBehaviour
{
    [SerializeField] private float radius = 4f;
    [SerializeField] public float enemyWalkSpeed;
    [SerializeField] public float enemyRunSpeed;
    [SerializeField] private float minWaitTime = 1f; // Минимальное время до смены точки
    [SerializeField] private float maxWaitTime = 3f; // Максимальное время до смены точки
    private Vector2 targetPosition;
    private Vector2 centerPoint;
    private float timer;
    private float waitTime;
    private bool isChasing;
    private bool isReturning;
    private bool wasChasing; // Был ли враг в режиме погони?
    private Transform player;
    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        centerPoint = transform.position;
        SetNewRandomTarget();
    }

    private void Update()
    {
        if (Vector2.Distance(player.transform.position, transform.position) <= 7)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, enemyRunSpeed * Time.deltaTime);
            if (player != null)
            {
                // Направление к цели
                Vector2 direction = player.position - transform.position;
                // Вычисляем угол
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                // Применяем поворот
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            isChasing = true;
            wasChasing = true;
        }
        else if (wasChasing && !isReturning)
        {
            isChasing = false;
            wasChasing = false;
            StartReturnToSpawn();
        }
        else
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
        }
        /*if (player != null)
        {
            // Направление к цели
            Vector2 direction = player.position - transform.position;
            // Вычисляем угол
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // Применяем поворот
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }*/
        
    }
    // Выбирает новую случайную позицию внутри круга
    private void SetNewRandomTarget()
    {
        // Случайный угол и радиус внутри круга
        float randomAngle = Random.Range(0f, Mathf.PI * 2f);
        float randomRadius = Random.Range(0f, radius);

        // Переводим полярные координаты (угол + радиус) в декартовы (x, y)
        targetPosition = centerPoint + new Vector2(
            Mathf.Cos(randomAngle) * randomRadius,
            Mathf.Sin(randomAngle) * randomRadius
        );

        waitTime = Random.Range(minWaitTime, maxWaitTime); // Случайное время ожидания
    }
    private void StartReturnToSpawn()
    {
        isReturning = true;
        Invoke(nameof(TeleportToSpawn), 0.5f);
    }

    private void TeleportToSpawn()
    {
        transform.position = centerPoint;
        isReturning = false;
    }
}
