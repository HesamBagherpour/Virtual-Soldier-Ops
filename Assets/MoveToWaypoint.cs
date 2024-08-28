using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class MoveToWaypoint : Action
{
    public SharedTransform[] waypoints;
    public NavMeshAgent agent;
    public AgentController AgentController;
    private int currentWaypoint = 0;

    public override void OnAwake()
    {
        agent = GetComponent<NavMeshAgent>();

    }

    public override void OnStart()
    {
        currentWaypoint = Random.Range(0, waypoints.Length);
        agent.destination = waypoints[currentWaypoint].Value.position;
        AgentController.SetStateWalking();
    }

    public override TaskStatus OnUpdate()
    {
        if (waypoints.Length == 0) return TaskStatus.Failure;

        if (!agent.pathPending && agent.remainingDistance < 1.0f)
        {
            //currentWaypoint = Random.Range(0, waypoints.Length);
            //(currentWaypoint + 1) % waypoints.Length;
            //agent.destination = waypoints[currentWaypoint].Value.position;
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }
}
