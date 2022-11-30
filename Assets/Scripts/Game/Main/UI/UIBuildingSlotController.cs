using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class UIBuildingSlotController : MonoBehaviour
{
    public TextMeshProUGUI display;
    public string displayText;

    public virtual GameObject Child
    {
        get
        {
            if (transform.childCount > 1)
            {
                return transform.GetChild(1).gameObject;
            }
            return null;
        }
        set {
            value.transform.SetParent(transform, false);
        }
    }

    void Start()
    {
        display.text = displayText;
    }
}
