using UnityEngine;
using UnityEngine.InputSystem;

public class Effect : MonoBehaviour
{
    public GameObject fireflyPrefab;
    public AudioClip jumpAudio;
    public AudioSource audioSource;

    public void Update()
    {
        Firefly();
    }
    public void Firefly()
    {
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {

            GameObject firefly = Instantiate(fireflyPrefab, Vector3.zero, Quaternion.identity);
            Debug.Log("make firefly");
            audioSource.PlayOneShot(jumpAudio, 1f);
            Destroy(firefly, 5f);
        }
    }
}
