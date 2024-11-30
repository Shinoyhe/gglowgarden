using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    void Interact();
}

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
    [SerializeField] private float turnTime = 1f;

    [Header("Interaction Settings")]
    [SerializeField] private float interactDistance = 15f;
    [SerializeField] private LayerMask interactionLayers;
    [SerializeField] private GameObject interactIndicator;

    // Internal Logics
    public CoreInput action;
    private InputAction moveAction;
    private InputAction interactAction;
    private Vector2 moveInput;
    private bool pressedInteract;
    private float verticalVelocity = 0f;
    private float currentWalkSpeed;

    #region "Setup"
    private void Awake()
    {
        // Input
        action = new CoreInput();
        moveAction = action.Player.Move;
        interactAction = action.Player.Interact;

        // Components
        characterController = GetComponent<CharacterController>();

        // Movement
        currentWalkSpeed = 0f;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        interactAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        interactAction.Disable();
    }

    #endregion

    private void Update()
    {
        ReadInput();
        Gravity();
        Movement();
        SendInteract();
    }

    private void ReadInput()
    {
        moveInput = moveAction.ReadValue<Vector2>().normalized;
        pressedInteract = interactAction.WasPressedThisFrame() && interactAction.ReadValue<float>() == 1;
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
        float temp = 0;
        float targetRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg, ref temp, turnTime);
        if (moveInput.magnitude>0){
            transform.rotation = Quaternion.Euler(0f, targetRotation, 0f);
        }
        // move = transform.forward*moveInput.magnitude;
        move *= currentWalkSpeed;
        characterController.Move(move * Time.deltaTime);
    }

    private void SendInteract()
    {
        // if (!pressedInteract) return;

        IInteractable nearbyInteract = getClosestInteractable();

        if (nearbyInteract != null)
        {
            if (pressedInteract) nearbyInteract.Interact();
            else HighlightInteractable(true);
        }
        else {
            HighlightInteractable(false);
        }

    }

    private IInteractable getClosestInteractable() {

        // Get all colliders near player
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactDistance, interactionLayers);

        // Setup our trackers
        float minDistance = float.MaxValue;
        IInteractable closestInteract = null;

        // Find the Collider (that has an interactable) that is closest to the player
        foreach (Collider collider in colliderArray)
        {
            // Only if collider has interactable we keep processing
            if (collider.TryGetComponent(out IInteractable interact) == false) continue;
            
            // Get distance from player to collider
            float x = Vector3.Distance(collider.transform.position, transform.position);

            // If we found a new closest interactable, update our trackers!
            if (x < minDistance)
            {
                minDistance = x;
                closestInteract = interact;
            }
            
        }

        // Return our closest collider or NULL if none found!
        return closestInteract;
    }
    
    void HighlightInteractable(bool highlight){
        interactIndicator.SetActive(highlight);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }

}
