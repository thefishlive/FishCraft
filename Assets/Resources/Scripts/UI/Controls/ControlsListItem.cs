using UnityEngine;
using InControl;
using UnityEngine.UI;

public class ControlsListItem : MonoBehaviour
{
    public PlayerAction Action
    {
        get { return m_action; }
        set
        {
            m_action = value;

            if (value != null)
            {
                UpdateAction();
            }
        }
    }

    public Text Label;
    public InputField KeyField;
    public Button BindButton;
    public Button ResetButton;

    private PlayerAction m_action;

	// Use this for initialization
	void Start ()
	{
	    ResetButton.onClick.AddListener(OnResetClicked);
        BindButton.onClick.AddListener(OnBindClicked);
    }

    private void UpdateAction()
    {
        Label.text = m_action.Name;
        KeyField.text = m_action.Bindings.Count == 0 ? "None" : m_action.Bindings[0].Name;

        m_action.ListenOptions = new BindingListenOptions
        {
            MaxAllowedBindings = 1,
            IncludeKeys = true,
            IncludeMouseButtons = true,
            OnBindingFound = (action, binding) =>
            {
                // Binding sources are comparable, so we can do this.
                if (binding == new KeyBindingSource(Key.Escape))
                {
                    action.StopListeningForBinding();
                    BindButton.interactable = true;
                    KeyField.text = m_action.Bindings.Count == 0 ? "None" : m_action.Bindings[0].Name;
                    return false;
                }

                return true;
            },
            OnBindingAdded = (action, binding) =>
            {
                KeyField.text = binding.Name;
                BindButton.interactable = true;
            }
        };
    }

    private void OnResetClicked()
    {
        m_action.StopListeningForBinding();
        m_action.ResetBindings();
        BindButton.interactable = true;

        KeyField.text = m_action.Bindings.Count == 0 ? "None" : m_action.Bindings[0].Name;
    }

    private void OnBindClicked()
    {
        m_action.ListenForBinding();
        KeyField.text = "Press Key";

        BindButton.interactable = false;
    }
}
