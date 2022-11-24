using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 10;
    bool showingOverview;
    float oldSize;
    public Camera cam;
    public Grid grid;
    public WorldGenerationController worldGenController;
    public GameObject groundPrefab;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            showingOverview = !showingOverview;
            if (showingOverview)
            {
                oldSize = cam.orthographicSize;
            }
            else
            {
                cam.orthographicSize = oldSize;
            }
        }
        if (showingOverview)
        {
            cam.orthographicSize = 200f;
        }
        else
        {
            cam.orthographicSize = Mathf.Clamp(
                cam.orthographicSize - Input.mouseScrollDelta.y,
                1f,
                7f
            );
        }
        transform.position += new Vector3(
            x: Input.GetAxis("Horizontal") * Time.deltaTime * speed * cam.orthographicSize,
            y: Input.GetAxis("Vertical") * Time.deltaTime * speed * cam.orthographicSize
        );

        worldGenController.LookAtCell(grid.WorldToCell(transform.position));
    }
}
