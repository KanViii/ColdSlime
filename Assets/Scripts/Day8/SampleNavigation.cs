using UnityEngine;
using UnityEngine.AI;

public class SampleNavigation : MonoBehaviour
{
    public Transform start;
    public Transform end;
    public NavMeshAgent agent;

    void Start()
    {
        
    }

    void Update()
    {
        agent.SetDestination(end.position);
        
    }
}
