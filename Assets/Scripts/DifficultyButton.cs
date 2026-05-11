using UnityEngine;

public class DifficultyButton : MonoBehaviour
{
    [SerializeField, Range(1, 3), Tooltip("1 = Easy, 2 = Medium, 3 = Hard.")]
    private int difficulty = 1;
    [SerializeField] private GameManager gameManager;

    public void Configure(GameManager manager, int value)
    {
        gameManager = manager;
        difficulty = value;
    }

    public void SetDifficulty()
    {
        gameManager.StartGame(difficulty);
    }
}
