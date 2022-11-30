using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacingController : MonoBehaviour
{
    public Grid grid;
    public GameObject canvas,
        listPrefab,
        UIBuildingPrefab,
        UIBuildingSlotPrefab;
    public GameObject[] buildingPrefabs;

    GameObject buildingList;

    BuildingBehaviour placing;
    Quaternion targetPlacementRotation = Quaternion.identity;
    LayerMask itemsMask;
    LayerMask buildingsMask;

    void Start()
    {
        itemsMask = LayerMask.GetMask("items");
        buildingsMask = LayerMask.GetMask("buildings", "conveyors");
    }

    void Update()
    {
        if (placing != null)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Destroy(placing.gameObject);
                placing = null;
                return;
            }
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            placing.transform.position =
                grid.CellToWorld(grid.WorldToCell(mousePos)) + placing.Size * 0.32f;
            if (Input.GetKeyDown(KeyCode.R))
            {
                placing.transform.Rotate(xAngle: 0, yAngle: 0, zAngle: -90);
                targetPlacementRotation = placing.transform.rotation;
            }
            else if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (
                    Physics2D
                        .OverlapBoxAll(
                            point: placing.transform.position,
                            size: placing.Size * 0.32f,
                            angle: 0,
                            layerMask: buildingsMask
                        )
                        .Length != 1
                )
                {
                    return;
                }
                placing.GetComponent<SpriteRenderer>().color = new Color(r: 1, g: 1, b: 1, a: 1);
                placing.Activate();

                foreach (
                    var item in Physics2D.OverlapBoxAll(
                        point: placing.transform.position,
                        size: placing.Size * 0.32f,
                        angle: 0,
                        layerMask: itemsMask
                    )
                )
                {
                    Destroy(item.gameObject);
                }
                placing = null;
            }
        }
        if (Input.GetKeyDown(KeyCode.B) && GameManager.openGUIs.Count == 0)
        {
            GameManager.openGUIs.Add(GameManager.GUIType.placing);
            buildingList = Instantiate(listPrefab);
            for (int i = 0; i < buildingPrefabs.Length; i++)
            {
                var slot = Instantiate(UIBuildingSlotPrefab)
                    .GetComponent<UIBuildingSlotController>();
                slot.displayText = buildingPrefabs[i].name;
                var building = Instantiate(UIBuildingPrefab).GetComponent<UIBuildingController>();
                building.Initialize(buildingPrefabs[i], this);
                slot.transform.SetParent(buildingList.transform, false);
                building.transform.SetParent(slot.transform, false);
            }
            buildingList.transform.SetParent(canvas.transform, false);
        }
        else if (
            Input.GetKeyDown(KeyCode.Escape)
            && GameManager.openGUIs.Contains(GameManager.GUIType.placing)
        )
        {
            GameManager.openGUIs.Remove(GameManager.GUIType.placing);
            Destroy(buildingList);
        }
    }

    public void StartPlacing(GameObject buildingPrefab)
    {
        GameManager.openGUIs.Remove(GameManager.GUIType.placing);
        Destroy(buildingList);
        placing = Instantiate(buildingPrefab).GetComponent<BuildingBehaviour>();
        placing.transform.rotation = targetPlacementRotation;
        placing.GetComponent<SpriteRenderer>().color = new Color(r: 0, g: 1, b: 0, a: 0.25f);
    }
}
