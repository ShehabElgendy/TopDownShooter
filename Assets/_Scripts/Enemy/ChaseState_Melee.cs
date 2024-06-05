using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState_Melee : EnemyState
{
    private EnemyMelee enemy;
    private float lastTimeUpdatedDestination;

    public ChaseState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as EnemyMelee;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.agent.isStopped = false;
        enemy.agent.speed = enemy.ChaseSpeed;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        enemy.transform.rotation = enemy.FaceTarget(enemy.agent.steeringTarget);

        if (enemy.PlayerInAttackRange())
           stateMachine.ChangeState(enemy.AttackState);

        if (CanUpdateDestination())
        {
            enemy.agent.destination = enemy.Player.position;
        }
    }

    private bool CanUpdateDestination()
    {
        if (Time.time > lastTimeUpdatedDestination + 0.25f)
        {
            lastTimeUpdatedDestination = Time.time;
            return true;
        }

        return false;
    }
}
