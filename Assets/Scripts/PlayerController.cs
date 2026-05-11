using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    private const float ArenaBodyHeight = 0.86f;
    private const float ArenaRadius = 4.05f;

    [SerializeField, Range(3f, 18f), Tooltip("Forward/back movement force.")]
    private float speed = 9f;
    [SerializeField, Tooltip("Knockback strength applied while powered up.")]
    private float powerupStrength = 18f;
    [SerializeField, Range(2f, 12f), Tooltip("Seconds before powerup expires.")]
    private float powerupDuration = 7f;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Transform focalPoint;
    [SerializeField] private GameObject powerupIndicator;
    [SerializeField] private AudioClip powerupClip;
    [SerializeField] private AudioClip hitClip;

    private Rigidbody playerRb;
    private AudioSource audioSource;
    private bool hasPowerup;

    public void Configure(GameManager manager, Transform focal, GameObject indicator, AudioClip collect, AudioClip hit)
    {
        gameManager = manager;
        focalPoint = focal;
        powerupIndicator = indicator;
        powerupClip = collect;
        hitClip = hit;
    }

    private void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        playerRb.linearDamping = 1.2f;
        playerRb.angularDamping = 0.8f;
        playerRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        playerRb.position = new Vector3(playerRb.position.x, ArenaBodyHeight, playerRb.position.z);
        playerRb.linearVelocity = Vector3.zero;
        DisableVisualColliders(powerupIndicator);
        if (powerupIndicator != null)
        {
            powerupIndicator.SetActive(false);
        }
    }

    private void Update()
    {
        if (gameManager == null || !gameManager.IsGameActive)
        {
            return;
        }

        float verticalInput = Input.GetAxis("Vertical");
        Vector3 moveDirection = focalPoint.forward;
        moveDirection.y = 0f;
        moveDirection.Normalize();
        playerRb.AddForce(moveDirection * verticalInput * speed);
    }

    private void FixedUpdate()
    {
        if (IsOverArena())
        {
            Vector3 velocity = playerRb.linearVelocity;
            playerRb.position = new Vector3(transform.position.x, ArenaBodyHeight, transform.position.z);
            playerRb.linearVelocity = new Vector3(velocity.x, 0f, velocity.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameManager == null || !gameManager.IsGameActive || !other.CompareTag("Powerup"))
        {
            return;
        }

        Destroy(other.gameObject);
        hasPowerup = true;
        if (powerupIndicator != null) powerupIndicator.SetActive(true);
        if (audioSource != null && powerupClip != null) audioSource.PlayOneShot(powerupClip, 0.75f);
        StartCoroutine(PowerupCountdownRoutine());
    }

    private IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(powerupDuration);
        hasPowerup = false;
        if (powerupIndicator != null) powerupIndicator.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasPowerup || !collision.gameObject.CompareTag("Enemy"))
        {
            return;
        }

        Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();
        if (enemyRb != null)
        {
            Vector3 awayFromPlayer = collision.gameObject.transform.position - transform.position;
            awayFromPlayer.y = 0f;
            enemyRb.AddForce(awayFromPlayer.normalized * powerupStrength, ForceMode.Impulse);
        }
        if (audioSource != null && hitClip != null) audioSource.PlayOneShot(hitClip, 0.7f);
    }

    private bool IsOverArena()
    {
        Vector2 flatPosition = new Vector2(transform.position.x, transform.position.z);
        return flatPosition.magnitude <= ArenaRadius;
    }

    private static void DisableVisualColliders(GameObject visualObject)
    {
        if (visualObject == null)
        {
            return;
        }

        foreach (Collider collider in visualObject.GetComponentsInChildren<Collider>(true))
        {
            collider.enabled = false;
        }
    }
}
