using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class FavoriteController : UIBuildingSlotController, IDropHandler
{
    public int slotIndex;

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
        set
        {
            if (value == null)
            {
                PlacingController.FavoritesList[slotIndex] = null;
            }
            else
            {
                PlacingController.FavoritesList[slotIndex] = value.GetComponent<UIBuildingController>().buildingPrefab;
                value.transform.SetParent(transform, false);
            }
        }
    }



    public void OnDrop(PointerEventData eventData)
    {
        if (Child != null)
        {
            Destroy(Child);
        }
        Child = UIBuildingController.buildingBeingDragged.gameObject;
        UIBuildingController.buildingBeingDragged.GetComponent<RectTransform>().offsetMin = new Vector2(10, 10);
        UIBuildingController.buildingBeingDragged.GetComponent<RectTransform>().offsetMax = new Vector2(-10, -10);
    }
}

