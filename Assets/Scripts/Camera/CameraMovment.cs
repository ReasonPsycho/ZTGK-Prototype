using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraMovment : MonoBehaviour
{
    public float panSpeed = 5f;
    public float panBorderThickness = 10f;
    public float rotationSpeed = 10f;

    public float minRotationX = 10F;
    public float maxRotationX = 45f;

    public float minX = 5f;
    public float maxX = 15f;
    
    
    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        Transform
            cameraTransform = Camera.main.transform; // replace with your camera's transform if it's not the main camera

        Vector3 cameraForward2D = new Vector3(cameraTransform.forward.x, 0, 0).normalized;
// we obtain the forward direction of the camera but only in the XZ plane (y component is 0), so it is not affected by pitch rotation
// then we normalize the vector, just in case it was close to 0 magnitude due to facing up or down

        float boost = 1f;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            boost = 4f;
        }


        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos += cameraForward2D * (panSpeed * Time.deltaTime * boost);
        }

        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= panBorderThickness)
        {
            pos -= cameraForward2D * (panSpeed * Time.deltaTime * boost);
        }

        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos += cameraTransform.right * (panSpeed * Time.deltaTime * boost);
        }

        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= panBorderThickness)
        {
            pos -= cameraTransform.right * (panSpeed * Time.deltaTime * boost);
        }


        if (Input.GetMouseButton((int)MouseButton.Middle)) //WHYYY Unity
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            transform.Rotate(Vector3.up, mouseX * Time.deltaTime * boost * rotationSpeed * 10, Space.World);

            float desiredRotationAngle = transform.eulerAngles.x + mouseY * Time.deltaTime * boost * rotationSpeed * 10;

            // if your rotation goes on in the other direction, invert the mouseX line to be -mouseX
            if (desiredRotationAngle >= 180) desiredRotationAngle -= 360; // convert to -180..+180 range
            desiredRotationAngle = Mathf.Clamp(desiredRotationAngle, minRotationX, maxRotationX);

            transform.rotation =
                Quaternion.Euler(desiredRotationAngle, transform.eulerAngles.y, transform.eulerAngles.z);
        }


        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime * boost, Space.World);
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime * boost, Space.World);
        }

        transform.position = pos;
    }
}