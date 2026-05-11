using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    private const float ArenaBodyHeight = 0.86f;
    private const float ArenaRadius = 4.05f;

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
        enemyRb.linearDamping = 1.2f;
        enemyRb.angularDamping = 0.8f;
        enemyRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        if (IsOverArena() && transform.position.y < ArenaBodyHeight - 0.2f)
        {
            transform.position = new Vector3(transform.position.x, ArenaBodyHeight, transform.position.z);
            enemyRb.linearVelocity = Vector3.zero;
        }
    }

    private void Update()
    {
        if (gameManager == null || !gameManager.IsGameActive || player == null)
        {
            return;
        }

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.001f)
        {
            enemyRb.AddForce(direction.normalized * speed);
        }
        KeepStableOnArena();
        if (transform.position.y < -10f)
        {
            gameManager.UpdateScore(pointValue);
            Destroy(gameObject);
        }
    }

    private void KeepStableOnArena()
    {
        if (!IsOverArena())
        {
            return;
        }

        Vector3 velocity = enemyRb.linearVelocity;
        transform.position = new Vector3(transform.position.x, ArenaBodyHeight, transform.position.z);
        enemyRb.linearVelocity = new Vector3(velocity.x, 0f, velocity.z);
    }

    private bool IsOverArena()
    {
        Vector2 flatPosition = new Vector2(transform.position.x, transform.position.z);
        return flatPosition.magnitude <= ArenaRadius;
    }
}
