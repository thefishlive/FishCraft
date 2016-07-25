using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("The x and y movement speed for this player")]
    public Vector2 MovementSpeed = Vector2.one;

    [Tooltip("The x and y look sensitivity for this player")]
    public Vector2 LookSensitivity = Vector2.one;

    [Tooltip("The minimum verticle look angle")]
    [Range(-90.0f, 90.0f)]
    public float LookMinY = -80.0f;

    [Tooltip("The maximum verticle look angle")]
    [Range(-90.0f, 90.0f)]
    public float LookMaxY = 80.0f;

    [Tooltip("The jump height for the player")]
    [Range(0f, 20f)]
    public float JumpHeight = 10f;

    [Header("Components")]
    [Tooltip("The camera for this player")]
    public Camera Camera = default(Camera);

    /// <summary>
    /// The action set for movement controls
    /// </summary>
    private PlayerMovementControls m_controls;
    /// <summary>
    /// The players rigid body
    /// </summary>
    private Rigidbody m_rigidbody;


    // Use this for initialization
    private void Start()
    {
        Debug.Assert(Camera != null, "Camera is not all set");

        m_controls = ControlsManager.GetActionSet<PlayerMovementControls>();
        m_rigidbody = GetComponent<Rigidbody>();

        LockCursor();
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void FixedUpdate()
    {
        HandleLook();

        HandleMovement();
        HandleJump();
    }

    private void HandleMovement()
    {
        var movement = Vector3.zero;
        movement += m_controls.Move.X * MovementSpeed.x * m_rigidbody.mass * transform.right;
        movement += m_controls.Move.Y * MovementSpeed.y * m_rigidbody.mass * transform.forward;

        m_rigidbody.AddForce(movement, ForceMode.Impulse);
    }

    private void HandleJump()
    {
        if (!m_controls.Jump.WasPressed) return;
        if (!Physics.Raycast(transform.position, Vector3.down, 1.5f)) return;

        m_rigidbody.AddForce(Vector3.up * JumpHeight * m_rigidbody.mass, ForceMode.Impulse);
    }

    private void HandleLook()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        transform.Rotate(Vector3.up, m_controls.Look.X * LookSensitivity.x);

        var vertAngle = Camera.transform.eulerAngles.x;
        vertAngle += m_controls.Look.Y * LookSensitivity.y;

        if (vertAngle <= 180.0f && vertAngle > -LookMinY)
        {
            vertAngle = -LookMinY;
        }

        if (vertAngle >= 180.0f && vertAngle < (360 - LookMaxY))
        {
            vertAngle = 360 - LookMaxY;
        }

        Camera.transform.rotation =
            Quaternion.Euler(new Vector3(vertAngle, Camera.transform.eulerAngles.y, Camera.transform.eulerAngles.z));
    }
}
