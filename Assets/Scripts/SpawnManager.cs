using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private const float ArenaSpawnRadius = 3.35f;
    private const float SafeCenterRadius = 1.75f;
    private const float SpawnHeight = 0.86f;

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject powerupPrefab;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Transform player;
    [SerializeField, Range(3f, 12f), Tooltip("Random X/Z range used for arena spawns.")]
    private float spawnRange = ArenaSpawnRadius;
    [SerializeField, Range(2f, 10f), Tooltip("Base enemy speed before difficulty scaling.")]
    private float baseEnemySpeed = 4f;

    private int waveNumber;
    private int difficulty = 1;
    private bool isSpawning;

    public void Configure(GameObject enemy, GameObject powerup, GameManager manager, Transform playerTransform)
    {
        enemyPrefab = enemy;
        powerupPrefab = powerup;
        gameManager = manager;
        player = playerTransform;
    }

    private void OnEnable()
    {
        GameManager.OnGameOver += StopSpawning;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= StopSpawning;
    }

    private void Update()
    {
        if (!isSpawning || gameManager == null || !gameManager.IsGameActive)
        {
            return;
        }

        int enemyCount = FindObjectsByType<Enemy>(FindObjectsSortMode.None).Length;
        if (enemyCount == 0)
        {
            SpawnNextWave();
        }
    }

    public void StartSpawning(int selectedDifficulty)
    {
        difficulty = selectedDifficulty;
        isSpawning = true;
        waveNumber = 0;
        SpawnNextWave();
    }

    private void SpawnNextWave()
    {
        waveNumber++;
        gameManager.UpdateWaveText(waveNumber);
        int enemyCount = waveNumber + difficulty - 1;
        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation);
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            enemyScript.Configure(gameManager, player, baseEnemySpeed + difficulty * 1.25f + waveNumber * 0.2f, difficulty * 10);
        }
        Instantiate(powerupPrefab, GenerateSpawnPosition(), powerupPrefab.transform.rotation);
    }

    private Vector3 GenerateSpawnPosition()
    {
        float radius = Mathf.Min(spawnRange, ArenaSpawnRadius);
        Vector2 flatPosition = Random.insideUnitCircle * radius;
        if (flatPosition.magnitude < SafeCenterRadius)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            flatPosition = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * SafeCenterRadius;
        }
        return new Vector3(flatPosition.x, SpawnHeight, flatPosition.y);
    }

    private void StopSpawning()
    {
        isSpawning = false;
    }
}
