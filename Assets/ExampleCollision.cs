using UnityEngine;

public class ExampleCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        var player = GetComponent<ExampleInput2>();

        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {   
            Debug.Log("Enter");
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {    
            Debug.Log("Stay");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        { 
            Debug.Log("Exit");
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        var player = GetComponent<ExampleInput2>();
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log("Enter");
        }
    }
}
