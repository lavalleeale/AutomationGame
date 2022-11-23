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

    bool isPlacing;
    BuildingBehaviour placing;
    Quaternion targetPlacementRotation = Quaternion.identity;
    LayerMask itemsMask;

    void Start()
    {
        itemsMask = LayerMask.GetMask("items");
    }

    void Update()
    {
        if (isPlacing)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isPlacing = false;
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
                isPlacing = false;
                placing.GetComponent<SpriteRenderer>().color = new Color(r: 1, g: 1, b: 1, a: 1);
                placing.Activate();
                var items = Physics2D.OverlapBoxAll(
                    point: placing.transform.position,
                    size: placing.Size,
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
            isPlacing = true;
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
            isPlacing = true;
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
            isPlacing = true;
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
            isPlacing = true;
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
            isPlacing = true;
            placing = Instantiate(constructorPrefab).GetComponent<BuildingBehaviour>();
            placing.transform.rotation = targetPlacementRotation;
            placing.GetComponent<SpriteRenderer>().color = new Color(r: 0, g: 1, b: 0, a: 0.25f);
        }
    }
}
