using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        WorldSelect,
        Playing,
        Paused,
        GameOver
    }

    [Header("Player & State")]
    public Transform player;
    public GameState currentState = GameState.WorldSelect;

    [Header("Scores")]
    [Tooltip("Coins collected during this run.")]
    public int coins = 0;
    [Tooltip("Gems (diamonds) collected.")]
    public int gems = 0;
    [Tooltip("Distance covered in units.")]
    public int distanceScore = 0;
    [Tooltip("Total calculated score (distance + coins * 10 + gems * 50).")]
    public int totalScore => distanceScore + (coins * 10) + (gems * 50);
    [Tooltip("Persistent all-time high score.")]
    public int highScore = 0;

    [Header("Revive Settings")]
    public int reviveGemCost = 5;

    public bool isGameOver => currentState == GameState.GameOver;
    public bool isPlaying => currentState == GameState.Playing;
    public bool isPaused => currentState == GameState.Paused;

    private const string HighScoreKey = "Runner_HighScore";
    private const string TotalGemsKey = "Runner_TotalGems";
    private float startZ = 0f;
    private float rawDistanceAccumulator = 0f;
    private PlayerController playerController;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        gems = PlayerPrefs.GetInt(TotalGemsKey, 5); // Start with 5 courtesy gems so user can test Revive!
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }

        OpenWorldSelect();
    }

    private void Update()
    {
        // Toggle pause with Escape or P
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (currentState == GameState.Playing)
            {
                PauseGame();
            }
            else if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
        }

        if (currentState == GameState.GameOver)
        {
            if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                RestartCurrentRun();
            }
            return;
        }

        if (currentState == GameState.Playing && player != null)
        {
            float rateMultiplier = (playerController != null && playerController.has2xMultiplier) ? 2.0f : 1.0f;
            rawDistanceAccumulator += (player.position.z - (startZ + rawDistanceAccumulator)) * rateMultiplier;
            distanceScore = Mathf.Max(0, Mathf.FloorToInt(player.position.z - startZ));
            if (playerController != null && playerController.has2xMultiplier)
            {
                distanceScore = Mathf.FloorToInt(distanceScore * 1.5f);
            }
        }
    }

    public void OpenWorldSelect()
    {
        Time.timeScale = 1f;
        currentState = GameState.WorldSelect;
        coins = 0;
        distanceScore = 0;

        if (playerController != null)
        {
            playerController.StopRunning();
            playerController.transform.position = new Vector3(0f, 0.95f, 0f);
        }

        if (TrackSpawner.Instance != null)
        {
            TrackSpawner.Instance.SpawnInitialRunway();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowWorldSelect();
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopMusic();
        }
    }

    public void StartRun(WorldType worldType)
    {
        Time.timeScale = 1f;
        if (WorldManager.Instance != null)
        {
            WorldManager.Instance.SelectWorld(worldType);
        }

        coins = 0;
        distanceScore = 0;
        rawDistanceAccumulator = 0f;
        currentState = GameState.Playing;

        if (player != null)
        {
            player.position = new Vector3(0f, 0.95f, 0f);
            startZ = player.position.z;
        }

        if (TrackSpawner.Instance != null)
        {
            TrackSpawner.Instance.SpawnInitialRunway();
        }

        if (playerController != null)
        {
            playerController.StartRunning();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowHUD();
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayMusic();
        }
    }

    public void AddCoin(int amount = 1)
    {
        if (currentState != GameState.Playing) return;
        coins += amount;
    }

    public void AddGem(int amount = 1)
    {
        if (currentState != GameState.Playing) return;
        gems += amount;
        PlayerPrefs.SetInt(TotalGemsKey, gems);
        PlayerPrefs.Save();
    }

    public void PauseGame()
    {
        if (currentState != GameState.Playing) return;
        currentState = GameState.Paused;
        Time.timeScale = 0f;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PauseMusic();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowPauseMenu();
        }
    }

    public void ResumeGame()
    {
        if (currentState != GameState.Paused) return;
        currentState = GameState.Playing;
        Time.timeScale = 1f;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.ResumeMusic();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.HidePauseMenu();
        }
    }

    public void GameOver()
    {
        if (currentState == GameState.GameOver) return;
        currentState = GameState.GameOver;

        int finalScore = totalScore;
        if (finalScore > highScore)
        {
            highScore = finalScore;
            PlayerPrefs.SetInt(HighScoreKey, highScore);
        }
        PlayerPrefs.SetInt(TotalGemsKey, gems);
        PlayerPrefs.Save();

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopMusic();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOver(finalScore, highScore, coins, gems);
        }
    }

    public bool TryRevive()
    {
        if (gems >= reviveGemCost)
        {
            gems -= reviveGemCost;
            PlayerPrefs.SetInt(TotalGemsKey, gems);
            PlayerPrefs.Save();
        }
        else
        {
            gems = 0;
        }

        currentState = GameState.Playing;
        Time.timeScale = 1f;

        if (playerController != null)
        {
            playerController.Revive(3);
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowHUD();
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayMusic();
        }

        return true;
    }

    public void RestartCurrentRun()
    {
        Time.timeScale = 1f;
        WorldType currentWorld = WorldManager.Instance != null ? WorldManager.Instance.selectedWorld : WorldType.Grassland;
        StartRun(currentWorld);
    }
}
