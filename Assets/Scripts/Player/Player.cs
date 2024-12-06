using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public interface IInteractable
{
    void Interact();
}

[RequireComponent(typeof(CharacterController))]

public class Player : MonoBehaviour
{
    const float ANIM_MPS = 6;
    
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
    
    [Header("Tiny")]
    [ReadOnly] public bool tiny = false;
    [ReadOnly] public float tinyMult = 0.05f;

    [Header("Debug Settings")]
    public GameObject debugObjectToCreate;

    // Internal Logics
    public CoreInput action;

    // Input Logics
    private InputAction moveAction;
    private InputAction interactAction;
    private InputAction debugAction;

    // Trackers
    private float verticalVelocity = 0f;
    private float currentWalkSpeed;
    private bool canMove = true;
    [ReadOnly] public bool inConversation = false;

    // Input Trackers
    private Vector2 moveInput;
    private float moveMagnitude;
    private bool pressedInteract;
    private bool pressedDebugButton;
    bool walking => moveInput.magnitude > 0.2f && canMove;
    
    // Character Controller Default Values
    float ccStepOffset;
    float ccSkinWidth;
    
    // Animator
    Animator animator;

    #region "Setup"
    private void Awake()
    {
        // Input
        action = new CoreInput();
        moveAction = action.Player.Move;
        interactAction = action.Player.Interact;
        debugAction = action.Player.Debug;

        // Components
        characterController = GetComponent<CharacterController>();
        ccStepOffset = characterController.stepOffset;
        ccSkinWidth = characterController.skinWidth;

        // Movement
        currentWalkSpeed = 0f;
        
        // Animator
        animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        moveAction.Enable();
        interactAction.Enable();
        debugAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        interactAction.Disable();
        debugAction.Disable();
        
        action.Player.Disable();
        action.UI.Disable();
    }

    #endregion

    private void Update()
    {
        ReadInput();
        if (canMove)
        {
            Gravity();
            Movement();
        }

        if (pressedDebugButton)
        {
            Instantiate(debugObjectToCreate);
        }

        SendInteract();
        
        characterController.stepOffset = tiny ? ccStepOffset*tinyMult : ccStepOffset;
        characterController.skinWidth = tiny ? ccSkinWidth*tinyMult : ccSkinWidth;
        
        animator.SetBool("Walking", walking);
        animator.SetFloat("Walking Speed", currentWalkSpeed/ANIM_MPS);
    }

    private void ReadInput()
    {
        var temp = moveAction.ReadValue<Vector2>();
        moveInput = temp.normalized;
        moveMagnitude = temp.magnitude;
        pressedInteract = interactAction.WasPressedThisFrame() && interactAction.ReadValue<float>() == 1;
        pressedDebugButton = debugAction.WasPressedThisFrame() && debugAction.ReadValue<float>() == 1;
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
        float _minWalkSpeed = minWalkSpeed;
        float _maxWalkSpeed = maxWalkSpeed;
        float _walkAcceleration = walkAcceleration;
        float _walkDeceleration = walkDeceleration;
        
        if (tiny){
            _minWalkSpeed *= tinyMult;
            _maxWalkSpeed *= tinyMult;
            _walkAcceleration *= tinyMult;
            _walkDeceleration *= tinyMult;
        }
        
        

        // Adjust movement
        if (walking)
        {
            currentWalkSpeed += _walkAcceleration * Time.deltaTime;
        }
        else
        {
            currentWalkSpeed -= _walkDeceleration * Time.deltaTime;
        }

        currentWalkSpeed = Mathf.Clamp(currentWalkSpeed, _minWalkSpeed, _maxWalkSpeed)*moveMagnitude;

        // Move
        Vector3 move = new Vector3(moveInput.x, verticalVelocity, moveInput.y);
        float temp = 0;
        float targetRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg, ref temp, turnTime);
        if (moveInput.magnitude>0){
            transform.rotation = Quaternion.Euler(0f, targetRotation, 0f);
        }
        move *= currentWalkSpeed;
        characterController.Move(move * Time.deltaTime);
    }

    public void ToggleMovement(bool canMove)
    {
        this.canMove = canMove;
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

    private IInteractable getClosestInteractable()
    {
        float _interactDistance = tiny ? interactDistance*tinyMult : interactDistance;
        
        // Get all colliders near player
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, _interactDistance, interactionLayers);

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
        if (inConversation) highlight = false;
        interactIndicator.SetActive(highlight);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }

}
