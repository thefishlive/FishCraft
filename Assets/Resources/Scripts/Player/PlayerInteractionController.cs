using System.Runtime.Remoting.Channels;
using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    /// <summary>
    /// The currently hovered block
    /// </summary>
    public BlockPos HoveredBlock { get; private set; }
    /// <summary>
    /// The face that the player is hovering over
    /// </summary>
    public Direction HoveredDirection { get; private set; }
    /// <summary>
    /// The currently selected block type to place
    /// </summary>
    public BlockType SelectedBlock { get; set; }
    /// <summary>
    /// The world the player is in
    /// </summary>
    public World World { get; private set; }

    [Header("Player Interaction")]
    [Tooltip("The reach in units of this player")]
    [Range(0, 10)]
    public float Reach = 4;

    [Tooltip("The offset applied to highlight raytracing")]
    [Range(0, 1)]
    public float HighlightOffset = 0.2f;

    [Header("Player Children")]
    [Tooltip("The main camera for this player")]
    public Camera Camera;

    [Tooltip("The transform of the highlight object")]
    public Transform HighlightTransform;

    /// <summary>
    /// The controls for this controller
    /// </summary>
    private PlayerInteractionControls m_controls;
    
	// Use this for initialization
	private void Start ()
	{
	    var worldObj = GameObject.FindWithTag("World");

	    if (worldObj == null)
	    {
	        Debug.LogError("Could not find world game object");
	        enabled = false;
	        return;
	    }

        World = worldObj.GetComponent<World>();

        Debug.Assert(Camera != null, "Camera is null");
        Debug.Assert(HighlightTransform != null, "Highlight transform is null");

        m_controls = ControlsManager.GetActionSet<PlayerInteractionControls>();
        
        SelectedBlock = BlockType.Grass;
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
	    UpdateHoveredBlock();
	}

    void FixedUpdate()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (m_controls.Dig.WasPressed)
        {
            HandleDig();
        }

        if (m_controls.Place.WasPressed)
        {
            HandlePlace();
        }
    }

    private void HandlePlace()
    {
        if (HoveredBlock == null)
            return;

        BlockPos newBlock;

        switch (HoveredDirection)
        {
            case Direction.Up:
                newBlock = HoveredBlock + new BlockPos(0, 1, 0);
                break;
            case Direction.Down:
                newBlock = HoveredBlock + new BlockPos(0, -1, 0);
                break;
            case Direction.Left:
                newBlock = HoveredBlock + new BlockPos(-1, 0, 0);
                break;
            case Direction.Right:
                newBlock = HoveredBlock + new BlockPos(1, 0, 0);
                break;
            case Direction.Forward:
                newBlock = HoveredBlock + new BlockPos(0, 0, 1);
                break;
            case Direction.Backwards:
                newBlock = HoveredBlock + new BlockPos(0, 0, -1);
                break;

            default:
                return;
        }

        if (World.IsValid(newBlock))
        {
            World.SetBlock(newBlock, SelectedBlock);
        }
    }

    private void HandleDig()
    {
        if (HoveredBlock == null)
            return;

        World.SetBlock(HoveredBlock, BlockType.Air);
        UpdateHoveredBlock();
    }

    private void UpdateHoveredBlock()
    {
        RaycastHit result;

        if (Physics.Raycast(new Ray(Camera.transform.position, Camera.transform.forward), out result, Reach))
        {
            HoveredBlock = new BlockPos(result.point + (Camera.transform.forward * HighlightOffset));

            if (result.normal == Vector3.up)
                HoveredDirection = Direction.Up;
            else if (result.normal == Vector3.down)
                HoveredDirection = Direction.Down;
            else if (result.normal == Vector3.left)
                HoveredDirection = Direction.Left;
            else if (result.normal == Vector3.right)
                HoveredDirection = Direction.Right;
            else if (result.normal == Vector3.forward)
                HoveredDirection = Direction.Forward;
            else if (result.normal == Vector3.back)
                HoveredDirection = Direction.Backwards;
            
            HighlightTransform.gameObject.SetActive(true);
            HighlightTransform.position = HoveredBlock.ToScenePos();
            HighlightTransform.rotation = Quaternion.identity;
        }
        else
        {
            HoveredBlock = null;
            HoveredDirection = Direction.None;

            HighlightTransform.gameObject.SetActive(false);
        }
    }
}
