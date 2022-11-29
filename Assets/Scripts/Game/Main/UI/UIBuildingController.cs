using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIBuildingController : MonoBehaviour
{
    GameObject buildingPrefab;
    PlacingController placingController;
    public Image image;
    public TextMeshProUGUI nameText;

    public void Initialize(GameObject buildingPrefab, PlacingController placingController)
    {
        this.buildingPrefab = buildingPrefab;
        image.sprite = buildingPrefab.GetComponent<SpriteRenderer>().sprite;
        nameText.text = buildingPrefab.name;
        this.placingController = placingController;
    }

    public void OnClick()
    {
        placingController.StartPlacing(buildingPrefab);
    }
}

