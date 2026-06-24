using UnityEngine;
using UnityEngine.AI;

public class SampleNavigation : MonoBehaviour
{
    public Transform start;
    public Transform end;
    public NavMeshAgent agent;

    void Start()
    {
        // var newPath = new NavMeshPath();
        // var path = NavMesh.CalculatePath(start.position, end.position, NavMesh.AllAreas, newPath);
        

        // foreach (var point in newPath.corners)
        // {    
        //     Debug.Log(point);
        // }

    }

    void Update()
    {
        agent.SetDestination(end.position);
        
    }
}
