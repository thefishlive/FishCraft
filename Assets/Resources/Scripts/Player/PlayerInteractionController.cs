using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    public BlockPos HoveredBlock { get; private set; }
    public Direction HoveredDirection { get; private set; }
    public BlockType SelectedBlock { get; set; }
    public World World { get; private set; }

    public float Reach;

    public Camera Camera;
    public Transform HighlightTransform;

    private PlayerInteractionControls m_controls;
    
	// Use this for initialization
	void Start ()
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

        m_controls = new PlayerInteractionControls();

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
        if (HoveredBlock == null) return;
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
        World.SetBlock(HoveredBlock, BlockType.Air);
        UpdateHoveredBlock();
    }

    private void UpdateHoveredBlock()
    {
        RaycastHit result;

        if (Physics.Raycast(new Ray(Camera.transform.position, Camera.transform.forward), out result, Reach))
        {
            HoveredBlock = new BlockPos(result.point + (Camera.transform.forward * 0.1f));

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
