using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState_Melee : EnemyState
{
    private EnemyMelee enemy;
    private Vector3 destination;

    public MoveState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as EnemyMelee;
    }

    public override void Enter()
    {
        base.Enter();

        destination = enemy.GetPatrolDestination();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        enemy.agent.SetDestination(destination);

        if(enemy.agent.remainingDistance <= 1 )
            stateMachine.ChangeState(enemy.IdleState);
    }
}
