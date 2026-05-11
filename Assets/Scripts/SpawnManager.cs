using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject powerupPrefab;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Transform player;
    [SerializeField, Range(3f, 12f), Tooltip("Random X/Z range used for arena spawns.")]
    private float spawnRange = 8f;
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
        Vector3 position = new Vector3(Random.Range(-spawnRange, spawnRange), 0.7f, Random.Range(-spawnRange, spawnRange));
        return position.magnitude < 2.5f ? position.normalized * 3f : position;
    }

    private void StopSpawning()
    {
        isSpawning = false;
    }
}
