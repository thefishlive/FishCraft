using UnityEngine;
using UnityEngine.UI;

public class ControlsListSubHeader : MonoBehaviour
{
    public string Text
    {
        get { return TextComponent.text; }
        set { TextComponent.text = value; }
    }

    public Text TextComponent;

	// Use this for initialization
	void Start ()
	{
        Debug.Assert(TextComponent != null, "Text component is null");
	}
}
