using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class TopDownMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    
    [Header("Boost Settings")]
    [SerializeField] private float boostMultiplier = 2f;
    [SerializeField] private float maxBoost = 100f; //Boost tank
    [SerializeField] private float boostDrainRate = 40f; // Boost usage
    [SerializeField] private float boostRechargeRate = 15f; //Boos reload
    [SerializeField] private Slider boostSlider; // boosSlider

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 720f;
    
    [Tooltip("Stell das so ein, wie dein Auto im Ordner (als PNG) aussieht:\nSchaut nach Rechts = 0\nSchaut nach Oben = -90\nSchaut nach Unten = 90\nSchaut nach Links = 180")]
    [SerializeField] private float rotationOffset = -90f; 

    [Header("Input Settings")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string actionMapName = "Player";

    private Rigidbody2D rb;
    private Vector2 moveInput;
    
    // Input Actions
    private InputAction moveAction;
    private InputAction boostAction; // Unsere neue Boost-Aktion

    // Boost Variablen
    private float currentBoost;
    private bool isBoosting;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        var map = inputActions.FindActionMap(actionMapName, throwIfNotFound: true);
        
        moveAction = map.FindAction("Move", throwIfNotFound: true);
        boostAction = map.FindAction("Boost", throwIfNotFound: true); 
        
        currentBoost = maxBoost; 
    }

    private void OnEnable()
    {
        moveAction.Enable();
        boostAction.Enable(); // Boost aktivieren
        
        moveAction.performed += OnMovePerformed;
        moveAction.canceled += OnMoveCanceled;
    }

    private void OnDisable()
    {
        moveAction.performed -= OnMovePerformed;
        moveAction.canceled -= OnMoveCanceled;
        
        moveAction.Disable();
        boostAction.Disable();
    }

    private void Update()
    {

        HandleBoostLogic();
    }

    private void FixedUpdate()
    {
        float currentSpeed = isBoosting ? moveSpeed * boostMultiplier : moveSpeed;

        // Bewegung
        rb.MovePosition(rb.position + moveInput.normalized * currentSpeed * Time.fixedDeltaTime);

        // SMOOTHE ROTATION
        if (moveInput.sqrMagnitude > 0.01f) 
        {
  
            float targetAngle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg + rotationOffset;
            float smoothedAngle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            
            rb.MoveRotation(smoothedAngle);
        }
    }

    private void HandleBoostLogic()
    {

        if (boostAction.IsPressed() && moveInput.sqrMagnitude > 0.01f && currentBoost > 0)
        {
            isBoosting = true;
            currentBoost -= boostDrainRate * Time.deltaTime; //Boost subtract
        }
        else
        {
            isBoosting = false;
            currentBoost += boostRechargeRate * Time.deltaTime; //Boost reload
        }

        currentBoost = Mathf.Clamp(currentBoost, 0f, maxBoost);

        if (boostSlider != null)
        {
            boostSlider.value = currentBoost / maxBoost; 
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx) => moveInput = ctx.ReadValue<Vector2>();
    private void OnMoveCanceled(InputAction.CallbackContext ctx) => moveInput = Vector2.zero;
}