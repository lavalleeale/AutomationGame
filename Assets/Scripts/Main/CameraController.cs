using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 10;
    public Camera cam;

    // Update is called once per frame
    void Update()
    {
        cam.orthographicSize = Mathf.Clamp(
            cam.orthographicSize - Input.mouseScrollDelta.y,
            1f,
            25f
        );
        transform.position += new Vector3(
            x: Input.GetAxis("Horizontal") * Time.deltaTime * speed * cam.orthographicSize,
            y: Input.GetAxis("Vertical") * Time.deltaTime * speed * cam.orthographicSize
        );
    }
}
