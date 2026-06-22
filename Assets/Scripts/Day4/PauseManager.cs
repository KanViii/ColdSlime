using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private Button exitButton;
    [SerializeField] private Button continueButton;

    // Khai báo event để PlayManager có thể lắng nghe
    public event Action OnExitClicked;
    public event Action OnContinueClicked;

    void Start()
    {
        if (exitButton != null) exitButton.onClick.AddListener(HandleExit);
        if (continueButton != null) continueButton.onClick.AddListener(HandleContinue);
    }

    void Update()
    {
        // Phím E để Exit
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            HandleExit();
        }

        // Bấm lại phím Pause (Escape) để Continue
        if (InputManager.Instance != null && InputManager.Instance.PauseTriggered)
        {
            HandleContinue();
        }
    }

    private void HandleExit()
    {
        OnExitClicked?.Invoke(); // Kích hoạt sự kiện Exit
    }

    private void HandleContinue()
    {
        OnContinueClicked?.Invoke(); // Kích hoạt sự kiện Continue
    }
}
