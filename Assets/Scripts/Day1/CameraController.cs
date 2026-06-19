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
        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                mode = CameraMode.FirstPerson;
            }
            if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                mode = CameraMode.ThirdPerson;
            }
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Chỉ xoay Camera khi người dùng NHẤN GIỮ chuột phải
        if (Mouse.current != null && Mouse.current.rightButton.isPressed)
        {
            Vector2 mouseDelata = Mouse.current.delta.ReadValue();

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
