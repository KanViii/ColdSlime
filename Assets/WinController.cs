using UnityEngine;
using System.Collections;

public class WinController : MonoBehaviour
{
    private bool hasWon = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            hasWon = true;
            Debug.Log("You won! Couting 3s down to exit ...");
            StartCoroutine(WinRoutine());
        }
    }

    private IEnumerator WinRoutine()
    {
        yield return new WaitForSeconds(3);
        Debug.Log("Exiting ...");

    }
}
