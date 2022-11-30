using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class UIBuildingSlotController : MonoBehaviour, IDropHandler
{
    public TextMeshProUGUI display;
    public string displayText;

    public GameObject Child
    {
        get
        {
            if (transform.childCount > 1)
            {
                return transform.GetChild(1).gameObject;
            }
            return null;
        }
        set { value.transform.SetParent(transform, false); }
    }

    void Start()
    {
        display.text = displayText;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (Child == null)
        {
            Child = UIBuildingController.buildingBeingDragged.gameObject;
            Child.GetComponent<RectTransform>().offsetMin = new Vector2(10, 10);
            Child.GetComponent<RectTransform>().offsetMax = new Vector2(-10, -10);
        }
    }
}

