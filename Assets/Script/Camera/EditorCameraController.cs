using UnityEngine;

public class EditorCameraController : MonoBehaviour
{
    public float moveSpeed = 20f;
    public float zoomSpeed = 200f;

    float horizontalInput;
    float verticalInput;
    float zoomInput;

    void Update()
        {
            HandleMove();
            HandleZoom();
    }

    void HandleMove()
    {
        horizontalInput=Input.GetAxis("Horizontal");
        verticalInput=Input.GetAxis("Vertical");
        
        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    void HandleZoom()
    {
        zoomInput=Input.GetAxis("Mouse ScrollWheel");
        transform.Translate(Vector3.up * zoomInput * zoomSpeed * Time.deltaTime, Space.World);
    }
}
