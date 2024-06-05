using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    [Header("Idle Data")]
    public float IdleTime;
    public float AggressionRange;

    [Header("Move Data")]
    public float MoveSpeed;
    public float ChaseSpeed;
    public float TurnSpeed;
    private bool manualMovement;
    private bool manualRotation;

    [SerializeField]
    private Transform[] patrolPoints;
    private int currentPatrolIndex;


    public Transform Player {  get; private set; }

    public Animator anim { get; private set; }
    public NavMeshAgent agent { get; private set; }
    public EnemyStateMachine stateMachine { get; private set; }


    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        Player = GameObject.Find("Player").GetComponent<Transform>();
    }

    protected virtual void Start()
    {
        InitializePatrolPoints();
    }

    protected virtual void Update()
    {
    }

    public void ActivateManualMovement(bool manualMovement) => this.manualMovement = manualMovement;

    public bool ManualMovementActive() => manualMovement;

    public void ActivateManualRotation(bool manualRotation) => this.manualRotation= manualRotation;

    public bool ManualRotationActive() => manualRotation;

    public void AnimationTriggered() => stateMachine.currentState.AnimationTriggered();

    public bool PlayerInAggressionRange() => Vector3.Distance(transform.position, Player.position) <= AggressionRange;

    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPoints[currentPatrolIndex].transform.position;

        currentPatrolIndex++;

        if(currentPatrolIndex >= patrolPoints.Length)
            currentPatrolIndex = 0;

        return destination;
    }

    private void InitializePatrolPoints()
    {
        foreach (Transform t in patrolPoints)
        {
            t.parent = null;
        }
    }

    public Quaternion FaceTarget(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);

        Vector3 currentEulerAngels  = transform.rotation.eulerAngles;

        float yRotation = Mathf.LerpAngle(currentEulerAngels.y, targetRotation.eulerAngles.y, TurnSpeed * Time.deltaTime);

        return Quaternion.Euler(currentEulerAngels.x, yRotation, currentEulerAngels.z);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, AggressionRange);
    }
}
