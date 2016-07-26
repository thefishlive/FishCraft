using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ChunkLoader))]
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
    /// <summary>
    /// The chunk loader for this player
    /// </summary>
    private ChunkLoader m_chunkLoader;
    /// <summary>
    /// The world this player is in
    /// </summary>
    private World m_world;
    /// <summary>
    /// The chunks that were loaded last frame
    /// </summary>
    private List<ChunkLocation> m_loadedChunks = new List<ChunkLocation>();

    // Use this for initialization
    private void Start()
    {
        Debug.Assert(Camera != null, "Camera is not all set");

        m_controls = ControlsManager.GetActionSet<PlayerMovementControls>();
        m_chunkLoader = GetComponent<ChunkLoader>();
        m_rigidbody = GetComponent<Rigidbody>();

        var worldObj = GameObject.FindWithTag("World");

        if (worldObj == null)
        {
            Debug.LogError("Could not find world game object");
            enabled = false;
            return;
        }

        m_world = worldObj.GetComponent<World>();

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
        if (transform.position.y <= 0)
        {
            Destroy(gameObject);
            return;
        }

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

        UpdateLoadedChunks();
    }

    private void UpdateLoadedChunks()
    {
        var chunks = m_chunkLoader.GetLoadedChunks();

        foreach (var chunk in m_loadedChunks.Select(loc => m_world.GetChunk(loc)))
        {
            chunk.Loaded = false;
        }

        m_loadedChunks = chunks;

        foreach (var chunk in chunks.Select(loc => m_world.GetChunk(loc)))
        {
            chunk.Loaded = true;
        }
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
