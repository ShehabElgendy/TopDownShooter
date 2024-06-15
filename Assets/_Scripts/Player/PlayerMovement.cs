using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Player player;

    private PlayerControls controls;

    private CharacterController characterController;

    private Animator animator;

    [Header("Movement Info")]

    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float runSpeed;

    [SerializeField]
    private float turnSpeed;

    private float speed;

    private float verticalVelocity;

    public  Vector2 moveInput {  get; private set; }

    private Vector3 movementDir;

    private float gravityScale = 9.81f;

    private bool isRunning;

    private static readonly int ISRUNNING_PARAMETER = Animator.StringToHash("isRunning");
    private static readonly int X_VELOCITY_PARAMETER = Animator.StringToHash("xVelocity");
    private static readonly int Z_VELOCITY_PARAMETER = Animator.StringToHash("zVelocity");


    private void Start()
    {
        player = GetComponent<Player>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        speed = walkSpeed;

        AssignInputEvents();
    }

    private void Update()
    {
        ApplyMovement();
        ApplyRotation();
        AnimatorControllers();
    }

    private void AnimatorControllers()
    {
        float xVelocity = Vector3.Dot(movementDir.normalized, transform.right);
        float zVelocity = Vector3.Dot(movementDir.normalized, transform.forward);

        animator.SetFloat(X_VELOCITY_PARAMETER, xVelocity, 0.1f, Time.deltaTime);
        animator.SetFloat(Z_VELOCITY_PARAMETER, zVelocity, 0.1f, Time.deltaTime);

        bool playRunAnimation = isRunning && movementDir.magnitude > 0f;
        animator.SetBool(ISRUNNING_PARAMETER, playRunAnimation);
    }

    private void ApplyRotation()
    {
        Vector3 lookingDir = player.aim.GetMouseHitInfo().point - transform.position;
        lookingDir.y = 0f;
        lookingDir.Normalize();

        Quaternion desiredRotation = Quaternion.LookRotation(lookingDir);

        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation,turnSpeed * Time.deltaTime);
    }

    private void ApplyMovement()
    {
        movementDir = new Vector3(moveInput.x, 0f, moveInput.y);
        ApplyGravit();
        if (movementDir.magnitude > 0f)
        {
            characterController.Move(movementDir * Time.deltaTime * speed);
        }
    }

    private void ApplyGravit()
    {
        if (!characterController.isGrounded)
        {
            verticalVelocity -= gravityScale * Time.deltaTime;
            movementDir.y = verticalVelocity;
        }
        else
            verticalVelocity = -0.5f;
    }


    private void AssignInputEvents()
    {
        controls = player.controls;

        controls.Character.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        controls.Character.Movement.canceled += context => moveInput = Vector2.zero;

        controls.Character.Run.performed += context =>
        {
            speed = runSpeed;
            isRunning = true;
        };

        controls.Character.Run.canceled += context =>
        {
            speed = walkSpeed;
            isRunning = false;
        };
    }
}
