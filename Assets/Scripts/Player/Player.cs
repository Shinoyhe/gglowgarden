using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]

public class Player : MonoBehaviour
{
    [Header("References")]
    private CharacterController characterController;

    [Header("Movement Settings")]
    [SerializeField] private float minWalkSpeed = 5f;
    [SerializeField] private float maxWalkSpeed = 15f;
    [SerializeField] private float walkAcceleration = 5f;
    [SerializeField] private float walkDeceleration = 10f;
    [SerializeField] private float gravityConstant = 15f;

    // Internal Logics
    public CoreInput action;
    private InputAction moveAction;
    private Vector2 moveInput;
    private float verticalVelocity = 0f;
    public float currentWalkSpeed;

    #region "Setup"
    private void Awake()
    {
        // Input
        action = new CoreInput();
        moveAction = action.Player.Move;

        // Components
        characterController = GetComponent<CharacterController>();

        // Movement
        currentWalkSpeed = 0f;
    }

    private void OnEnable()
    {
        moveAction = action.Player.Move;
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }

    #endregion

    private void Update()
    {
        ReadInput();
        Gravity();
        Movement();
    }

    private void ReadInput()
    {
        moveInput = moveAction.ReadValue<Vector2>().normalized;
    }

    private void Gravity()
    {
        if (characterController.isGrounded)
        {
            verticalVelocity = 0f;
        }

        verticalVelocity -= gravityConstant * Time.deltaTime;
    }

    private void Movement()
    {
        bool walking = moveInput.magnitude > 0.2f;

        Debug.Log(moveInput.magnitude);

        // Adjust movement
        if (walking)
        {
            currentWalkSpeed += walkAcceleration * Time.deltaTime;
        }
        else
        {
            currentWalkSpeed -= walkAcceleration * Time.deltaTime;
        }

        currentWalkSpeed = Mathf.Clamp(currentWalkSpeed, minWalkSpeed, maxWalkSpeed);

        // Move
        Vector3 move = new Vector3(moveInput.x, verticalVelocity, moveInput.y);
        move *= currentWalkSpeed;
        characterController.Move(move * Time.deltaTime);
    }
}
