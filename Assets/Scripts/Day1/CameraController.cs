using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public enum CameraMode { FirstPerson, ThirdPerson }
    public CameraMode mode = CameraMode.ThirdPerson;

    [Header("General Settings")]
    public Transform target;
    public float mouseSensitivity = 10f;
    public Vector2 pitchMinMax = new Vector2(-40f, 85f);

    [Header("Third Person Settings")]
    public float distanceFromTarget = 5f;
    private float yaw;
    private float pitch;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None; // Mở khóa để chuột tự do
        Cursor.visible = true; // Hiển thị chuột trên màn hình
    }


    void Update()
    {
        bool cam1 = false;
        bool cam3 = false;
        
        if (InputManager.Instance != null)
        {
            cam1 = InputManager.Instance.Camera1Triggered;
            cam3 = InputManager.Instance.Camera3Triggered;
        }
        else if (Keyboard.current != null)
        {
            cam1 = Keyboard.current.digit1Key.wasPressedThisFrame;
            cam3 = Keyboard.current.digit3Key.wasPressedThisFrame;
        }

        if (cam1) mode = CameraMode.FirstPerson;
        if (cam3) mode = CameraMode.ThirdPerson;
    }

    void LateUpdate()
    {
        if (target == null) return;

        bool isRotating = false;
        if (InputManager.Instance != null) isRotating = InputManager.Instance.IsRotating;
        else if (Mouse.current != null) isRotating = Mouse.current.rightButton.isPressed;

        // Chỉ xoay Camera khi người dùng NHẤN GIỮ chuột phải (hoặc nhấn giữ nút xoay trên màn hình)
        if (isRotating)
        {
            Vector2 mouseDelata = Vector2.zero;
            if (InputManager.Instance != null) mouseDelata = InputManager.Instance.LookDelta;
            else if (Mouse.current != null) mouseDelata = Mouse.current.delta.ReadValue();

            yaw += mouseDelata.x * mouseSensitivity * Time.deltaTime;
            pitch -= mouseDelata.y * mouseSensitivity * Time.deltaTime;
        }

        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
        target.rotation = Quaternion.Euler(0, yaw, 0);

        if (mode == CameraMode.FirstPerson)
        {
            HandleFirstPerson();
        }
        else
        {
            HandleThirdPerson();
        }
    }

    void HandleFirstPerson()
    {
        transform.position = target.position + Vector3.up * 1f;
        transform.eulerAngles = new Vector3(pitch, yaw, 0);
        // Debug.Log($"camera {transform.position}");

    }

    void HandleThirdPerson()
    {
        transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        transform.position = target.position - transform.forward * distanceFromTarget + Vector3.up * 1.5f;
        // Debug.Log($"camera {transform.position}");
    }
}
