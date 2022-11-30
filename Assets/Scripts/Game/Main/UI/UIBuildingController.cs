using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class UIBuildingController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject buildingPrefab;
    PlacingController placingController;
    public Image image;
    public CanvasGroup cg;
    public static UIBuildingController buildingBeingDragged;

    bool beingDragged;
    Transform previousParent;

    public void Initialize(GameObject buildingPrefab, PlacingController placingController)
    {
        this.buildingPrefab = buildingPrefab;
        image.sprite = buildingPrefab.GetComponent<SpriteRenderer>().sprite;
        this.placingController = placingController;
    }

    public void OnClick()
    {
        if (!beingDragged)
            placingController.StartPlacing(buildingPrefab);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        cg.blocksRaycasts = false;
        previousParent = transform.parent;
        buildingBeingDragged = this;
        beingDragged = true;
        var newObject = Instantiate(gameObject);
        newObject.transform.SetParent(transform.parent, false);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        beingDragged = false;
        cg.blocksRaycasts = true;
        buildingBeingDragged = null;
        if (transform.parent == previousParent)
        {
            Destroy(gameObject);
        }
    }
}
