//using BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimator;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class AgentController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private int _walkingStateHash = Animator.StringToHash("Walking");
    private int _idleStateHash = Animator.StringToHash("Idle");


    private void Start()
    {
        
    }

    public void SetStateWalking()
    {
        _animator.Play(_walkingStateHash);
    }  
    public void SetStateIdle()
    {
        _animator.Play(_idleStateHash);
    }
}
