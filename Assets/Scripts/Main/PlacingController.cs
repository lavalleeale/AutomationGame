using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacingController : MonoBehaviour
{
    public Grid grid;
    public GameObject minerPrefab,
        conveyorPrefab,
        furnacePrefab,
        mergerPrefab,
        constructorPrefab;

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
        if (GameManager.inGUI)
        {
            if (placing != null)
            {
                Destroy(placing);
            }
            return;
        }
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
                var items = Physics2D.OverlapBoxAll(
                    point: placing.transform.position,
                    size: placing.Size * 0.32f,
                    angle: 0,
                    layerMask: itemsMask
                );
                if (items != null)
                {
                    foreach (
                        var item in Physics2D.OverlapBoxAll(
                            point: placing.transform.position,
                            size: placing.Size,
                            angle: 0,
                            layerMask: itemsMask
                        )
                    )
                    {
                        Destroy(item.gameObject);
                    }
                }
                placing = null;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (placing != null)
            {
                Destroy(placing.gameObject);
            }

            placing = Instantiate(minerPrefab).GetComponent<BuildingBehaviour>();
            placing.transform.rotation = targetPlacementRotation;
            placing.GetComponent<SpriteRenderer>().color = new Color(r: 0, g: 1, b: 0, a: 0.25f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (placing != null)
            {
                Destroy(placing.gameObject);
            }

            placing = Instantiate(conveyorPrefab).GetComponent<BuildingBehaviour>();
            placing.transform.rotation = targetPlacementRotation;
            placing.GetComponent<SpriteRenderer>().color = new Color(r: 0, g: 1, b: 0, a: 0.25f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (placing != null)
            {
                Destroy(placing.gameObject);
            }

            placing = Instantiate(furnacePrefab).GetComponent<BuildingBehaviour>();
            placing.transform.rotation = targetPlacementRotation;
            placing.GetComponent<SpriteRenderer>().color = new Color(r: 0, g: 1, b: 0, a: 0.25f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (placing != null)
            {
                Destroy(placing.gameObject);
            }

            placing = Instantiate(mergerPrefab).GetComponent<BuildingBehaviour>();
            placing.transform.rotation = targetPlacementRotation;
            placing.GetComponent<SpriteRenderer>().color = new Color(r: 0, g: 1, b: 0, a: 0.25f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (placing != null)
            {
                Destroy(placing.gameObject);
            }

            placing = Instantiate(constructorPrefab).GetComponent<BuildingBehaviour>();
            placing.transform.rotation = targetPlacementRotation;
            placing.GetComponent<SpriteRenderer>().color = new Color(r: 0, g: 1, b: 0, a: 0.25f);
        }
    }
}
