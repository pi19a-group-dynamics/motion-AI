using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public float speed;
    public float mouseSens;
    private Rigidbody _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        float mouseY = Input.GetAxis("Mouse X");
        float mouseX = Input.GetAxis("Mouse Y");
        Quaternion currRot = _rb.rotation;
        Vector3 rotation = new Vector3(-mouseX * mouseSens + currRot.eulerAngles.x, mouseY * mouseSens + currRot.eulerAngles.y, 0);
        if (rotation.x < 275 && rotation.x > 80f)
        {
            rotation = currRot.eulerAngles;
        }
        Vector3 movementHor = new Vector3(moveHorizontal, 0f, moveVertical);
        Quaternion deltaRotation = Quaternion.Euler(rotation);
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _rb.AddRelativeForce(movementHor * speed * Time.deltaTime * 10f);
        }
        else
        {
            _rb.AddRelativeForce(movementHor * speed * Time.deltaTime);
        }
        _rb.MoveRotation(deltaRotation);
    }
}