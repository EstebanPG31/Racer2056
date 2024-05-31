using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.AI;

public class BController : MonoBehaviour
{
    private NavMeshAgent agent;
    //public GameObject destino;
    //private Transform punto;
    public Transform[] checkpoints;
    private int index = 0;
    private int laps = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        NextPoint();
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 1f)
        {
            NextPoint();
        }
    }

    void NextPoint()
    {
        if (laps < 4)
        {
            agent.SetDestination(checkpoints[index].position);
            print("yendo a punto "+index);
            index = (index+1) % checkpoints.Length;
            /*if (index < checkpoints.Length)
            {
                index++;
            }
            else if (index == checkpoints.Length)
            {
                index = 0;
                laps++;
            }*/
        }
    }

}
