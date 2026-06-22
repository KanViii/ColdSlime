using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button loadButton;

    void Start()
    {
        InitButtonEvents();
    }

    private void InitButtonEvents()
    {
        startButton.onClick.AddListener(OnClickStartButton);
        loadButton.onClick.AddListener(OnClickLoadButton);
    }

    private void OnClickStartButton()
    {
        Debug.Log("Start Button Clicked");
        SaveSystem.ClearSaveData();
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.ResetScore();
        }
        LoadGameScene(1);
    }

    private void OnClickLoadButton()
    {
        Debug.Log("Load Button Clicked - Loading game states...");
        GameSaveData data = SaveSystem.LoadFromJson(1, 0);
        int savedLevel = data.currentLevel;
        LoadGameScene(savedLevel);
    }

    private void LoadGameScene(int targetLevel)
    {
        string sceneToLoad = null;

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.SetLevel(targetLevel); 
            LevelData currentData = LevelManager.Instance.GetCurrentLevelData();
            if (currentData != null && !string.IsNullOrEmpty(currentData.sceneName))
            {
                sceneToLoad = currentData.sceneName;
            }
        }

        if (sceneToLoad != null)
        {
            SceneManager.LoadScene(sceneToLoad); 
        }
        else
        {
            SceneManager.LoadScene(1);
        }

        CanvasManager.Instance.removeUI(this);
    }
}
