using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private BasicInput basicInput;

    public Vector2 MoveInput { get; private set; }
    public bool AttackTriggered { get; private set; }
    public bool IsRotating { get; private set; }
    
    public Vector2 LookDelta { get; private set; }
    public bool PauseTriggered { get; private set; }
    public bool Camera1Triggered { get; private set; }
    public bool Camera3Triggered { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        basicInput = new BasicInput();
    }

    private void OnEnable()
    {
        basicInput?.Enable();
    }

    private void OnDisable()
    {
        basicInput?.Disable();
    }

    private void Update()
    {
        // Nhận Input có sẵn trong BasicInput.cs
        MoveInput = basicInput.Moverment.Move.ReadValue<Vector2>();
        AttackTriggered = basicInput.Moverment.Attack.WasPressedThisFrame();
        IsRotating = basicInput.Moverment.Rotation.IsPressed();

        
        // Pause
        var pauseAction = basicInput.asset.FindAction("Pause");
        if (pauseAction != null && pauseAction.WasPressedThisFrame()) PauseTriggered = true;
        else PauseTriggered = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;

        // Look Delta
        var lookAction = basicInput.asset.FindAction("LookDelta");
        if (lookAction != null) LookDelta = lookAction.ReadValue<Vector2>();
        else if (Mouse.current != null) LookDelta = Mouse.current.delta.ReadValue();
        else LookDelta = Vector2.zero;

        // Camera Switch 1
        var cam1Action = basicInput.asset.FindAction("Camera1");
        if (cam1Action != null && cam1Action.WasPressedThisFrame()) Camera1Triggered = true;
        else Camera1Triggered = Keyboard.current != null && Keyboard.current.digit1Key.wasPressedThisFrame;

        // Camera Switch 3
        var cam3Action = basicInput.asset.FindAction("Camera3");
        if (cam3Action != null && cam3Action.WasPressedThisFrame()) Camera3Triggered = true;
        else Camera3Triggered = Keyboard.current != null && Keyboard.current.digit3Key.wasPressedThisFrame;
    }
}
