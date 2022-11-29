using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 100;

    //bool showingOverview;
    //float oldSize;
    public Camera cam;
    public Grid grid;
    public Rigidbody2D rb;
    public WorldGenerationController worldGenController;

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    showingOverview = !showingOverview;
        //    if (showingOverview)
        //    {
        //        oldSize = cam.orthographicSize;
        //    }
        //    else
        //    {
        //        cam.orthographicSize = oldSize;
        //    }
        //}
        //if (showingOverview)
        //{
        //    cam.orthographicSize = 200f;
        //}
        //else
        //{
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - Input.mouseScrollDelta.y, 1f, 7f);
        //}
        worldGenController.LookAtCell(grid.WorldToCell(transform.position));
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector3(
            x: Input.GetAxis("Horizontal") * speed * cam.orthographicSize,
            y: Input.GetAxis("Vertical") * speed * cam.orthographicSize
        );
    }
}
