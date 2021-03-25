using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    private CharacterController characterController;
    private float speed = 5.0f;
    private float mouseSensitivity = 150.0f;
    private float xRotation = 0.0f;
    private Transform m_transform;
    public Camera cam;


    public override void OnStartLocalPlayer()
    {
        characterController = gameObject.GetComponent<CharacterController>();
        m_transform = gameObject.GetComponent<Transform>();
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        playerMove(x, z);
        MouseController(mouseX, mouseY);

        if(!Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void playerMove(float x, float z)
    {
        Vector3 move = transform.right * x + transform.forward * z;
        characterController.Move(move * speed * Time.deltaTime);
    }

    void MouseController(float mouseX, float mouseY)
    {
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90.0f, 90.0f);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0.0f, 0.0f);
        m_transform.Rotate(Vector3.up * mouseX);
    }
}
