using UnityEngine;

public class Enemy: MonoBehaviour
{
    [SerializeField] private float radius = 2f;
    [SerializeField] public float enemyWalkSpeed;
    [SerializeField] public float enemyRunSpeed;
    [SerializeField] private Vector2 centerPoint;
    private float angle = 0f;
    private Transform player;
    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        if (centerPoint == Vector2.zero)
            centerPoint = transform.position;
    }

    private void Update()
    {
        if (Vector2.Distance(player.transform.position, transform.position) <= 7)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, enemyRunSpeed * Time.deltaTime);
        }
        else
        {
            angle -= enemyWalkSpeed * Time.deltaTime;
            float x = centerPoint.x + Mathf.Cos(angle) * radius;
            float y = centerPoint.y + Mathf.Sin(angle) * radius;
            // Применяем новую позицию
            transform.position = new Vector2(x, y);
        }
    }
}
