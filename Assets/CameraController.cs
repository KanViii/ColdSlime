using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public enum CameraMode { FirstPerson, ThirdPerson }
    public CameraMode mode = CameraMode.FirstPerson;

    [Header("General Settings")]
    public Transform target;
    public float mouseSensitivity = 10f;
    public Vector2 pitchMinMax = new Vector2(-40f, 85f);
    public Vector2 yawMinMax = new Vector2(-60f, 60f);

    [Header("Third Person Settings")]
    public float distanceFromTarget = 5f;
    private float yaw;
    private float pitch;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

        if (Mouse.current != null)
        {
            Vector2 mouseDelata = Mouse.current.delta.ReadValue();

            yaw += mouseDelata.x * mouseSensitivity * Time.deltaTime;
            pitch += mouseDelata.y * mouseSensitivity * Time.deltaTime;
        }

        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
        yaw = Mathf.Clamp(yaw, yawMinMax.x, yawMinMax.y);

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
