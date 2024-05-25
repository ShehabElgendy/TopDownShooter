using UnityEngine;

public class EnemyMelee : Enemy
{
    public IdleState_Melee IdleState { get; private set; }
    public MoveState_Melee MoveState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        IdleState = new IdleState_Melee(this, stateMachine, "Idle");
        MoveState = new MoveState_Melee(this, stateMachine, "Move");
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
}
