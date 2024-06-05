using System.Collections.Generic;
using UnityEngine;

public class AttackState_Melee : EnemyState
{
    private EnemyMelee enemy;
    private Vector3 attackDir;
    private float attackMoveSpeed;
    private const string ATTACK_ANIMATION_SPEED = "AttackAnimationSpeed";
    private const string ATTACK_INDEX = "AttackIndex";
    private const string RECOVERY_INDEX = "RecoveryIndex";

    private const float MAX_ATTACK_DISTANCE = 50f;
    public AttackState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as EnemyMelee;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.PullWeapon();
        enemy.anim.SetFloat(ATTACK_ANIMATION_SPEED, enemy.AttackData.AnimationSpeed);
        enemy.anim.SetFloat(ATTACK_INDEX, enemy.AttackData.AttackIndex);
        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;
        attackMoveSpeed = enemy.AttackData.MoveSpeed;

        attackDir = enemy.transform.position + (enemy.transform.forward * MAX_ATTACK_DISTANCE);
    }

    public override void Exit()
    {
        base.Exit();

        SetupNextAttack();

    }

    private void SetupNextAttack()
    {
        int recoveryIndex = IsPlayerClose() ? 1 : 0;
        enemy.anim.SetFloat(RECOVERY_INDEX, recoveryIndex);

        enemy.AttackData = UpdatedAttackData();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.ManualRotationActive())
        {
            enemy.transform.rotation = enemy.FaceTarget(enemy.Player.position);
            attackDir = enemy.transform.position + (enemy.transform.forward * MAX_ATTACK_DISTANCE);

        }

        if (enemy.ManualMovementActive())
        {
            enemy.transform.position =
                Vector3.MoveTowards(enemy.transform.position, attackDir, attackMoveSpeed * Time.deltaTime);
        }

        if (triggerCalled)
        {
            if (enemy.PlayerInAttackRange())
                stateMachine.ChangeState(enemy.RecoveryState);
            else
                stateMachine.ChangeState(enemy.ChaseState);
        }
    }

    private bool IsPlayerClose() => enemy.PlayerInAttackRange();
    //private bool IsPlayerClose() => Vector3.Distance(enemy.transform.position, enemy.Player.position) <= 1;
    
    private AttackData UpdatedAttackData()
    {
        List<AttackData> validAttacks = new List<AttackData>(enemy.AttackDataList);
        if(IsPlayerClose())
            validAttacks.RemoveAll(parameter => parameter.AttackType == AttackType_Melee.Charge);

        int randomIndex = Random.Range(0, validAttacks.Count);
        return validAttacks[randomIndex];
    }
}
