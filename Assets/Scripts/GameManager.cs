using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static event Action OnGameOver;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject titleScreen;
    [SerializeField] private CanvasGroup titleCanvasGroup;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private CanvasGroup gameOverCanvasGroup;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject restartButton;
    [SerializeField, Range(0.2f, 1.5f), Tooltip("Seconds used for coroutine-driven UI fades.")]
    private float fadeDuration = 0.5f;
    [SerializeField, Range(1f, 1.8f), Tooltip("Scale multiplier for the score punch animation.")]
    private float scorePunchScale = 1.25f;

    [HideInInspector] public int currentDifficulty;

    private int score;
    private bool isGameActive;

    public bool IsGameActive => isGameActive;

    public void Configure(TextMeshProUGUI score, TextMeshProUGUI wave, Transform playerTransform, GameObject title,
        CanvasGroup titleGroup, SpawnManager spawner, GameObject gameOverRoot, CanvasGroup gameOverGroup, GameObject restart)
    {
        scoreText = score;
        waveText = wave;
        player = playerTransform;
        titleScreen = title;
        titleCanvasGroup = titleGroup;
        spawnManager = spawner;
        gameOverPanel = gameOverRoot;
        gameOverCanvasGroup = gameOverGroup;
        restartButton = restart;
    }

    private void Start()
    {
        score = 0;
        UpdateScoreText();
        UpdateWaveText(0);
        isGameActive = false;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (restartButton != null) restartButton.SetActive(false);
        if (titleScreen != null) titleScreen.SetActive(true);
    }

    private void Update()
    {
        if (isGameActive && player != null && player.position.y < -10f)
        {
            GameOver();
        }
    }

    public void StartGame(int difficulty)
    {
        currentDifficulty = difficulty;
        score = 0;
        UpdateScoreText();
        isGameActive = true;
        StartCoroutine(FadeOutTitle());
        spawnManager.StartSpawning(difficulty);
    }

    public void UpdateScore(int points)
    {
        if (!isGameActive)
        {
            return;
        }

        score += points;
        UpdateScoreText();
        StartCoroutine(PunchScoreText());
    }

    public void UpdateWaveText(int wave)
    {
        if (waveText != null)
        {
            waveText.text = "Wave: " + wave;
            StartCoroutine(SlideWaveText());
        }
    }

    public void GameOver()
    {
        if (!isGameActive)
        {
            return;
        }

        isGameActive = false;
        OnGameOver?.Invoke();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    private IEnumerator FadeOutTitle()
    {
        if (titleCanvasGroup == null || titleScreen == null)
        {
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            titleCanvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }
        titleScreen.SetActive(false);
    }

    private IEnumerator PunchScoreText()
    {
        if (scoreText == null)
        {
            yield break;
        }

        Transform target = scoreText.transform;
        Vector3 start = Vector3.one;
        Vector3 peak = Vector3.one * scorePunchScale;
        float elapsed = 0f;
        while (elapsed < 0.18f)
        {
            elapsed += Time.deltaTime;
            target.localScale = Vector3.Lerp(start, peak, elapsed / 0.18f);
            yield return null;
        }
        elapsed = 0f;
        while (elapsed < 0.18f)
        {
            elapsed += Time.deltaTime;
            target.localScale = Vector3.Lerp(peak, start, elapsed / 0.18f);
            yield return null;
        }
        target.localScale = start;
    }

    private IEnumerator SlideWaveText()
    {
        if (waveText == null)
        {
            yield break;
        }

        RectTransform rect = waveText.rectTransform;
        Vector2 end = new Vector2(0, -36);
        Vector2 start = new Vector2(0, 8);
        float elapsed = 0f;
        while (elapsed < 0.35f)
        {
            elapsed += Time.deltaTime;
            rect.anchoredPosition = Vector2.Lerp(start, end, elapsed / 0.35f);
            yield return null;
        }
        rect.anchoredPosition = end;
    }
}
