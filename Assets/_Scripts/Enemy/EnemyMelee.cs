using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AttackData
{
    public string AttackName;
    public float AttackRange;
    public float MoveSpeed;
    public float AttackIndex;
    [Range(1f, 2f)]
    public float AnimationSpeed;
    public AttackType_Melee AttackType;
}

public enum AttackType_Melee { Close,Charge}

public class EnemyMelee : Enemy
{
    public IdleState_Melee IdleState { get; private set; }
    public MoveState_Melee MoveState { get; private set; }
    public RecoveryState_Melee RecoveryState { get; private set; }
    public ChaseState_Melee ChaseState { get; private set; }
    public AttackState_Melee AttackState { get; private set; }

    [Header("Attack Data")]
    public AttackData AttackData;
    public List<AttackData> AttackDataList;

    [SerializeField] private Transform hiddenWeapon;
    [SerializeField] private Transform pulledWeapon;

    protected override void Awake()
    {
        base.Awake();

        IdleState = new IdleState_Melee(this, stateMachine, "Idle");
        MoveState = new MoveState_Melee(this, stateMachine, "Move");
        RecoveryState = new RecoveryState_Melee(this, stateMachine, "Recovery");
        ChaseState = new ChaseState_Melee(this, stateMachine, "Chase");
        AttackState = new AttackState_Melee(this, stateMachine, "Attack");
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(IdleState);
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();
    }

    public void PullWeapon()
    {
        hiddenWeapon.gameObject.SetActive(false);
        pulledWeapon.gameObject.SetActive(true);
    }

    public bool PlayerInAttackRange() => Vector3.Distance(transform.position, Player.position) <= AttackData.AttackRange;

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, AttackData.AttackRange);
    }
}
