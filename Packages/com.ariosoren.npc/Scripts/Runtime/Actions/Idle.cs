using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class Idle : Action
{
    public float minIdleTime = 1f;
    public float maxIdleTime = 5f;
    private float idleTime;
    private float startTime;
    public AgentController AgentController;
    public override void OnStart()
    {
        idleTime = Random.Range(minIdleTime, maxIdleTime);
        startTime = Time.time;
        AgentController.SetStateIdle();
    }

    public override TaskStatus OnUpdate()
    {
        if (Time.time - startTime >= idleTime)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }
}
