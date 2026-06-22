using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class PlayManager : MonoBehaviour
{
    [SerializeField] private Slider HPSlier;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button attackButton;
    [SerializeField] private Text scoreText; 
    [SerializeField] private Text levelText; 


    private bool isPaused = false;
    private PauseManager uiPause; 

    void Start()
    {
        InitButtonEvents();
        UpdateScoreUI();
        UpdateLevelUI();

        // Setup giá trị HP Slider ban đầu
        if (Player.Instance != null && Player.Instance.myStats != null)
        {
            HPSlier.maxValue = Player.Instance.myStats.maxHealth;
            HPSlier.value = Player.Instance.CurrentHealth;
        }
    }

    private void OnEnable()
    {
        Enemy.OnAnyEnemyDied += HandleEnemyDied;
        Player.OnPlayerHealthChanged += UpdateHPSliderUI; // Lắng nghe sự kiện đổi máu
        LevelManager.OnLevelChanged += HandleLevelChanged; // Lắng nghe sự kiện chuyển Level
        LevelManager.OnScoreChanged += UpdateScoreUI; // Lắng nghe sự thay đổi điểm
    }

    private void OnDisable()
    {
        Enemy.OnAnyEnemyDied -= HandleEnemyDied;
        Player.OnPlayerHealthChanged -= UpdateHPSliderUI;
        LevelManager.OnLevelChanged -= HandleLevelChanged;
        LevelManager.OnScoreChanged -= UpdateScoreUI;
    }

    private void HandleLevelChanged(int newLevel)
    {
        UpdateLevelUI();
    }

    private void UpdateHPSliderUI(float currentHP, float maxHP)
    {
        if (HPSlier != null)
        {
            HPSlier.maxValue = maxHP;
            HPSlier.value = currentHP;
        }
    }

    private void HandleEnemyDied(Enemy enemyKilled)
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.AddScore(1);
        }
    }

    private void UpdateScoreUI(int currentScore)
    {
        if (scoreText != null)
        {
            scoreText.text = "" + currentScore;
        }
    }

    private void UpdateScoreUI()
    {
        if (LevelManager.Instance != null)
        {
            UpdateScoreUI(LevelManager.Instance.CurrentScore);
        }
    }

    // Hàm cập nhật Level UI
    public void UpdateLevelUI()
    {
        if (levelText != null && LevelManager.Instance != null && LevelManager.Instance.LevelConfigs != null)
        {
            int currentLevel = LevelManager.Instance.CurrentLevel;
            int maxLevel = LevelManager.Instance.LevelConfigs.Count;
            levelText.text = currentLevel + "/" + maxLevel;
        }
    }

    void Update()
    {
        bool pause = false;
        if (InputManager.Instance != null) pause = InputManager.Instance.PauseTriggered;
        else if (Keyboard.current != null) pause = Keyboard.current.escapeKey.wasPressedThisFrame;

        if (pause)
        {
            if (!isPaused)
            {
                OnClickPauseButton(); 
            }
        }
    }

    private void InitButtonEvents()
    {
        pauseButton.onClick.AddListener(OnClickPauseButton);
    }

    private void OnClickPauseButton()
    {
        if (isPaused) return; // Tránh gọi nhiều lần

        isPaused = true; 
        Time.timeScale = 0f; 
        
        uiPause = CanvasManager.Instance.LoadPrefabs<PauseManager>("UIPause");
        if (uiPause != null)
        {
            uiPause.OnContinueClicked += ResumeGame;
            uiPause.OnExitClicked += ExitToMenu;
            
            CanvasManager.Instance.AddUI(uiPause);
        }
        
        Debug.Log("Game Paused!");
    }

    private void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1f; 

        if (uiPause != null)
        {
            uiPause.OnContinueClicked -= ResumeGame;
            uiPause.OnExitClicked -= ExitToMenu;

            CanvasManager.Instance.removeUI(uiPause);
            uiPause = null; 
        }
        Debug.Log("Game Resumed!");
    }

    private void ExitToMenu()
    {
        Time.timeScale = 1f;

        if (uiPause != null)
        {
            CanvasManager.Instance.removeUI(uiPause);
            uiPause = null;
        }
        CanvasManager.Instance.removeUI(this); 

        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }
}
