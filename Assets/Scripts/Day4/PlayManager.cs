using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class PlayManager : MonoBehaviour
{
    [SerializeField] private Slider HPSlier;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Text scoreText; 
    [SerializeField] private Text levelText; // <--- Thêm dòng này để tham chiếu đến UI Text hiện Level


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
            levelText.text = "Level " + currentLevel + "/" + maxLevel;
        }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
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
        isPaused = !isPaused; 

        if (isPaused)
        {
            Time.timeScale = 0f; 
            uiPause = CanvasManager.Instance.LoadPrefabs<PauseManager>("UIPause");
            CanvasManager.Instance.AddUI(uiPause);
            Debug.Log("Game Paused!");
        }
        else
        {
            Time.timeScale = 1f; 
            if (uiPause != null)
            {
                CanvasManager.Instance.removeUI(uiPause);
                uiPause = null; 
            }
            Debug.Log("Game Resumed!");
        }
    }
}
