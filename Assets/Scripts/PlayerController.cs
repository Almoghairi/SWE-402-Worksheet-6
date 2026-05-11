using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
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
        playerRb.AddForce(focalPoint.forward * verticalInput * speed);
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
            enemyRb.AddForce(awayFromPlayer.normalized * powerupStrength, ForceMode.Impulse);
        }
        if (audioSource != null && hitClip != null) audioSource.PlayOneShot(hitClip, 0.7f);
    }
}
