using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState
{
    protected Enemy enemyBase;
    protected EnemyStateMachine stateMachine;
    protected string animBoolName;

    public EnemyState(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName)
    {
        this.enemyBase = enemyBase;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        Debug.Log($"I Enter {animBoolName} state!");
    }

    public virtual void Update()
    {
        Debug.Log($"I am Running {animBoolName} state!");
    }

    public virtual void Exit()
    {
        Debug.Log($"I Exit {animBoolName} state!");
    }
}
