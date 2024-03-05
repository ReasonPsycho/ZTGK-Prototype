using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraMovment : MonoBehaviour
{
    private Camera camera;

    private bool isClicked = false;
    private bool hasHit = false;
    private RaycastHit hit;
    private Ray camRay;
    public float panSpeed = 5f;
    public float panBorderThickness = 30f;
    public float rotationSpeed = 10f;

    public float minRotationX = 10F;
    public float maxRotationX = 45f;

    public float minY = 5f;
    public float maxY = 15f;

    private void Start()
    {
        camera = this.GetComponent<Camera>();
    }


    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        Transform
            cameraTransform = Camera.main.transform; // replace with your camera's transform if it's not the main camera

        Vector3 cameraForward2D = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
// we obtain the forward direction of the camera but only in the XZ plane (y component is 0), so it is not affected by pitch rotation
// then we normalize the vector, just in case it was close to 0 magnitude due to facing up or down

        Vector3 mousePos = Input.mousePosition;

        if (mousePos.x >= 0 && mousePos.x <= Screen.width && mousePos.y >= 0 && mousePos.y <= Screen.height)
        {
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


            if (Input.GetMouseButton((int)MouseButton.Middle))
            {
                if (hasHit)
                {
                    float mouseX = Input.GetAxis("Mouse X");
                    float mouseY = Input.GetAxis("Mouse Y");

                    //Camera position
                    var dir = transform.position - hit.point; // find direction relative to point
                    Quaternion
                        q1 = Quaternion.AngleAxis(mouseX,
                            Vector3.up); // rotate that direction according to rotation params
                    Quaternion
                        q2 = Quaternion.AngleAxis(mouseY,
                            Vector3.right); // rotate that direction according to rotation params

                    dir = q1 * q2 * dir; // apply rotation
                    pos = hit.point + dir; // update position
                    transform.position = hit.point + dir; // update position 

                    if (pos.y > maxY)
                    {
                        pos.y = maxY;
                    }

                    if (pos.y < minY)
                    {
                        pos.y = minY;
                    }

                    //Camera rotation

                    Vector3 relativePos = hit.point - camRay.origin - transform.position;

                    // the second argument, upwards, defaults to Vector3.up
                    Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
                    transform.rotation = rotation;


                    float desiredRotationAngle =
                        transform.eulerAngles.x + mouseY * Time.deltaTime * boost * rotationSpeed * 10;

                    // if your rotation goes on in the other direction, invert the mouseX line to be -mouseX
                    if (desiredRotationAngle >= 180) desiredRotationAngle -= 360; // convert to -180..+180 range
                    desiredRotationAngle = Mathf.Clamp(desiredRotationAngle, minRotationX, maxRotationX);

                    transform.rotation =
                        Quaternion.Euler(desiredRotationAngle, transform.eulerAngles.y, transform.eulerAngles.z);
                }
            }

            if (Input.GetMouseButtonDown((int)MouseButton.Middle)) //WHYYY Unity
            {
                camRay = camera.ScreenPointToRay(Input.mousePosition);
                hasHit = Physics.Raycast(camRay, out hit, 100f);

                if (hasHit)
                {
                    Debug.Log(hit.point);
                }
            }

            if (Input.GetMouseButtonUp((int)MouseButton.Middle))
            {
                hasHit = false;
            }
        }

        transform.position = pos;
    }
}