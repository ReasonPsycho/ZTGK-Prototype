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
    public float scrollSpeed = 5f;
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

            if (Mathf.Abs(Input.mouseScrollDelta.y) > 0.01f)
            {
                pos += cameraTransform.forward * (Input.mouseScrollDelta.y * (scrollSpeed * Time.deltaTime * boost));
            }

            

            if (Input.GetMouseButton((int)MouseButton.Middle))
            {
                if (hasHit)
                {
                    float mouseX = Input.GetAxis("Mouse X");
                    float mouseY = Input.GetAxis("Mouse Y");

                    //Camera position
                    var dir = transform.position - hit.point; // find direction relative to point

                    Vector3 eulerRotation = new Vector3( mouseY  * rotationSpeed * Time.deltaTime * boost,  mouseX  * rotationSpeed * Time.deltaTime* boost, mouseY  * rotationSpeed * Time.deltaTime* boost);
                    Quaternion q1 = Quaternion.Euler(eulerRotation);
                    dir = q1 * dir; // apply rotation

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

                    Vector3 relativePos = hit.point - transform.position;
                    
                    // the second argument, upwards, defaults to Vector3.up
                    Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
                    transform.rotation = rotation;
                    
                }
            }

            if (Input.GetMouseButtonDown((int)MouseButton.Middle)) //WHYYY Unity
            {
                camRay = camera.ScreenPointToRay(new Vector2(Screen.width/2,Screen.height/2));
                hasHit = Physics.Raycast(camRay, out hit, 100f);
            }

            if (Input.GetMouseButtonUp((int)MouseButton.Middle))
            {
                hasHit = false;
            }
        }

        transform.position = pos;
    }
}