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
        enemy.agent.speed = enemy.MoveSpeed;
        destination = enemy.GetPatrolDestination();
        enemy.agent.SetDestination(destination);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.PlayerInAggressionRange())
        {
            stateMachine.ChangeState(enemy.RecoveryState);
            return;
        }

        enemy.transform.rotation = enemy.FaceTarget(enemy.agent.steeringTarget);

        if(enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
            stateMachine.ChangeState(enemy.IdleState);
    }
}
