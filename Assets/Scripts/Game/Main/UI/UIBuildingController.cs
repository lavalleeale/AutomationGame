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
        if (
            transform.parent.TryGetComponent<FavoriteController>(
                out FavoriteController favController
            )
        )
        {
            favController.Child = null;
        }
        else
        {
            var newObject = Instantiate(gameObject);
            newObject
                .GetComponent<UIBuildingController>()
                .Initialize(buildingPrefab, placingController);
            newObject.transform.SetParent(transform.parent, false);
        }
        cg.blocksRaycasts = false;
        previousParent = transform.parent;
        buildingBeingDragged = this;
        beingDragged = true;
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
