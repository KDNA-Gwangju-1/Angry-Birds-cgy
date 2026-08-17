using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int startingShots = 10;
    public int shotsRemaining;
    public int score;
    public int pigsRemaining;
    public bool gameEnded;

    public Text shotsText;
    public Text scoreText;
    public GameObject resultPanel;
    public Text resultText;
    public Button restartButton;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        shotsRemaining = startingShots;
        pigsRemaining = GameObject.FindGameObjectsWithTag("Pig").Length;
        gameEnded = false;
        if (resultPanel != null) resultPanel.SetActive(false);
        if (restartButton != null) restartButton.onClick.AddListener(RestartGame);
        UpdateUI();
    }

    public void RegisterShotFired()
    {
        if (gameEnded) return;
        shotsRemaining--;
        UpdateUI();
        if (shotsRemaining <= 0)
        {
            Invoke(nameof(CheckLose), 3f);
        }
    }

    public void RegisterPigDestroyed(int points)
    {
        if (gameEnded) return;
        score += points;
        pigsRemaining--;
        UpdateUI();
        if (pigsRemaining <= 0)
        {
            ShowWin();
        }
    }

    void CheckLose()
    {
        if (!gameEnded && pigsRemaining > 0)
        {
            ShowLose();
        }
    }

    void ShowWin()
    {
        gameEnded = true;
        if (resultPanel != null) resultPanel.SetActive(true);
        if (resultText != null) resultText.text = "You Win!";
    }

    void ShowLose()
    {
        gameEnded = true;
        if (resultPanel != null) resultPanel.SetActive(true);
        if (resultText != null) resultText.text = "You Lose";
    }

    void UpdateUI()
    {
        if (shotsText != null) shotsText.text = "Shots: " + shotsRemaining;
        if (scoreText != null) scoreText.text = "Score: " + score;
    }

    public void RestartGame()
    {
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }
}
