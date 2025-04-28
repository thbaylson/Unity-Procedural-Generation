using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DirectedAgent : MonoBehaviour
{
    NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void MoveToLocation(Vector3 targetPosition)
    {
        agent.SetDestination(targetPosition);
        agent.isStopped = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
