using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float zoomFactor;
    private new Camera camera;
    void Awake()
    {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float cameraZoom = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(cameraZoom) > 0)
        {
            // Find the vector pointing from the camera and change inversely
            Vector3 cameraVector = Vector3.Normalize(transform.position) * zoomFactor; 

            cameraVector *= -cameraZoom;
            transform.position += cameraVector;
        }

        if (Input.GetKey("mouse 2"))
        {
            float cameraRotateX = Input.GetAxis("Mouse X") * zoomFactor;
            float cameraRotateY = Input.GetAxis("Mouse Y") * zoomFactor;

            transform.RotateAround(Vector3.zero, Vector3.up - Vector3.Normalize(transform.position), cameraRotateX);
            transform.RotateAround(Vector3.zero, Vector3.left - Vector3.Normalize(transform.position), cameraRotateY);
        }

        // Check if the scroll wheel button is pressed
    }
}
