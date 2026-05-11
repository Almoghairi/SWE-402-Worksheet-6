using System.Collections;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject gameOverText;
    [SerializeField] private GameObject restartButton;
    [SerializeField, Range(0.1f, 1.5f), Tooltip("Fade-in duration for the game over screen.")]
    private float fadeDuration = 0.5f;

    public void Configure(CanvasGroup group, GameObject textObject, GameObject restart)
    {
        canvasGroup = group;
        gameOverText = textObject;
        restartButton = restart;
    }

    private void OnEnable()
    {
        GameManager.OnGameOver += Show;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= Show;
    }

    private void Show()
    {
        if (gameOverText != null) gameOverText.SetActive(true);
        if (restartButton != null) restartButton.SetActive(true);
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        if (canvasGroup == null)
        {
            yield break;
        }

        canvasGroup.gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }
    }
}
