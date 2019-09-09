using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerMovement : MonoBehaviour
{

    CharacterController controller;

    public float m_movementSpeed = 5;
    public float m_gravity = 1;
    public float m_jumpForce = 20;

    private float verticalVelocity;

	// Use this for initialization
	void Start ()
    {
        controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        float x = Input.GetAxis("Horizontal") * m_movementSpeed * Time.deltaTime;
        float z = Input.GetAxis("Vertical") * m_movementSpeed * Time.deltaTime;

        if (controller.isGrounded)
        {
            verticalVelocity = -m_gravity * Time.deltaTime;

            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = m_jumpForce * Time.deltaTime;
            }
        }
        else
        {
            verticalVelocity -= 1 * Time.deltaTime;
        }

        Vector3 moveDelta = new Vector3(x, verticalVelocity, z);

        controller.Move(moveDelta);
	}
}
