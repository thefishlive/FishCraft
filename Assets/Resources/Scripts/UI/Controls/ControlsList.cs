using UnityEngine;

public class ControlsList : MonoBehaviour
{
    public GameObject ListItem;

    public GameObject ListSubHeader;
    
	// Use this for initialization
	void Start ()
    {
        foreach (var actionSet in ControlsManager.GetActionSets())
	    {
	        if (ListSubHeader != null)
	        {
	            var header = Instantiate(ListSubHeader);
                header.transform.SetParent(transform);
	        }

            foreach (var action in actionSet.Actions)
            {
                var item = Instantiate(ListItem);
                var listItem = item.GetComponent<ControlsListItem>();
                listItem.Action = action;
                item.transform.SetParent(transform);
            }
	    }
	}
}
