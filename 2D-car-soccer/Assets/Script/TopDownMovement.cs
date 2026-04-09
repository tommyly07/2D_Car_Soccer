using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class TopDownMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string actionMapName = "Player"; // "Player" oder "Player2"

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private InputAction moveAction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Hole die Action direkt aus der Action Map by name
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
        rb.MovePosition(rb.position + moveInput.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
    }
}