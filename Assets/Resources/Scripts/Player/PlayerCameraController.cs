using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerCameraController : MonoBehaviour
{
    [Header("Components")]
    [Tooltip("The camera for this player")]
    public Camera Camera = default(Camera);

    private Rigidbody m_rigidbody;
    
    // Use this for initialization
    private void Start()
    {
        Debug.Assert(Camera != null, "Camera is not all set");

        m_rigidbody = GetComponent<Rigidbody>();
    }

    public void EnableCamera()
    {
        Camera.gameObject.SetActive(true);
        m_rigidbody.isKinematic = false;
    }
}
