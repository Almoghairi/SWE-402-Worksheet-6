using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    [SerializeField, Tooltip("Score awarded when this enemy falls off the arena.")]
    private int pointValue = 10;
    [SerializeField, Range(1f, 14f), Tooltip("Chase force applied toward the player.")]
    private float speed = 4f;
    [SerializeField] private GameManager gameManager;

    private Rigidbody enemyRb;
    private Transform player;

    public void Configure(GameManager manager, Transform target, float chaseSpeed, int points)
    {
        gameManager = manager;
        player = target;
        speed = chaseSpeed;
        pointValue = points;
    }

    private void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (gameManager == null || !gameManager.IsGameActive || player == null)
        {
            return;
        }

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;
        direction.Normalize();
        enemyRb.AddForce(direction * speed);
        if (transform.position.y < -10f)
        {
            gameManager.UpdateScore(pointValue);
            Destroy(gameObject);
        }
    }
}
