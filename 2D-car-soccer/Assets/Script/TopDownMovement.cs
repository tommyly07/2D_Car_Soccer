using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class TopDownMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 720f; // Grad pro Sekunde
    
    [Tooltip("Stell das so ein, wie dein Auto im Ordner (als PNG) aussieht:\nSchaut nach Rechts = 0\nSchaut nach Oben = -90\nSchaut nach Unten = 90\nSchaut nach Links = 180")]
    [SerializeField] private float rotationOffset = -90f; 

    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string actionMapName = "Player";

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private InputAction moveAction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        var map = inputActions.FindActionMap(actionMapName, throwIfNotFound: true);
        moveAction = map.FindAction("Move", throwIfNotFound: true);
    }

    private void OnEnable()
    {
        moveAction.Enable();
        moveAction.performed += OnMovePerformed;
        moveAction.canceled += OnMoveCanceled;
    }

    private void OnDisable()
    {
        moveAction.performed -= OnMovePerformed;
        moveAction.canceled -= OnMoveCanceled;
        moveAction.Disable();
    }

    private void FixedUpdate()
    {
        // Bewegung
        rb.MovePosition(rb.position + moveInput.normalized * moveSpeed * Time.fixedDeltaTime);

        // SMOOTHE ROTATION
        if (moveInput.sqrMagnitude > 0.01f) 
        {
            // Berechne den Ziel-Winkel basierend auf dem Input
            float targetAngle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg + rotationOffset;
            
            // Bewege die aktuelle Rotation langsam Richtung Ziel-Winkel
            float smoothedAngle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            
            rb.MoveRotation(smoothedAngle);
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx) => moveInput = ctx.ReadValue<Vector2>();
    private void OnMoveCanceled(InputAction.CallbackContext ctx) => moveInput = Vector2.zero;
}