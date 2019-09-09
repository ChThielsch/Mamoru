using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject m_spawnPoint;
    public float m_rotationRate = 360;
    public float m_moveSpeed = 10;
    public float m_jumpForce = 10;
    public float m_fallingTime = 2f;
    public float m_dashForce = 10;
    public float m_walljumpAngle = 90;
    public float m_health = 100;
    public byte m_multiJumps =2;
    
    private string m_moveInputAxis = "Vertical";
    private string m_turnInputAxis = "Horizontal";
    private float m_currentJump;
    private float m_dashCoolDown = 5;
    private float m_dashCoolDownTimer;
    private float m_invincebilityTimer = 2;
    private bool m_isJumped = false;
    private bool m_isGrounded = false;
    private bool m_invincebility = false;
    private Rigidbody m_rigidbody;


    Vector3 appliedJumpForce;



    // Use this for initialization
    void Start ()
    {
        m_currentJump = m_multiJumps;
        m_rigidbody = GetComponent<Rigidbody>();
        appliedJumpForce = new Vector3(0f, m_jumpForce, 0f);
        m_isGrounded = true;
    }
	
	// Update is called once per frame
	void Update ()
    {
        Debug.Log("currentJump = "+m_currentJump);
        GroundCheck();
        float moveAxis = Input.GetAxis(m_moveInputAxis);
        float turnAxis = Input.GetAxis(m_turnInputAxis);
        
        ApplyInput(moveAxis, turnAxis);       
    }

    private void FixedUpdate()
    {
        if (m_isJumped)
        {
            m_rigidbody.AddForce(appliedJumpForce, ForceMode.Impulse);
            m_isJumped = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Walljump(collision);
        HealthManager(collision);
    }

    private void ApplyInput(float moveInput, float turnInput)
    {
        Move(moveInput);
        Turn(turnInput);
        Jump();
        Dash();
    }

    private void Move(float input)
    {
        Vector3 deltaTranslation;

            if (input >= 0)
            {
                deltaTranslation = transform.position + transform.forward * m_moveSpeed * input * Time.deltaTime;
            }
            else
            {
                deltaTranslation = transform.position + transform.forward * (m_moveSpeed/2) * input * Time.deltaTime;
            }
            m_rigidbody.MovePosition(deltaTranslation);
        
    }

    private void Turn(float input)
    {
        Quaternion deltaRotation = Quaternion.Euler(m_rotationRate * new Vector3(0, input, 0) * Time.deltaTime);
        m_rigidbody.MoveRotation(m_rigidbody.rotation * deltaRotation);
    }
    
    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && m_currentJump > 0)
        {
            m_isJumped = true;
            m_currentJump--;
        }
    }

    private void Dash()
    {
        if (m_dashCoolDownTimer > 0)
        {
            m_dashCoolDownTimer -= Time.deltaTime;
        }

        if (m_dashCoolDownTimer <0)
        {
            m_dashCoolDownTimer = 0;
        }

        if (Input.GetButton("Fire1") && m_dashCoolDownTimer == 0)
        {

            //m_invincebilityTimer = Time.deltaTime;
            //m_invincebility = true;
            m_rigidbody.AddForce(transform.forward * m_dashForce, ForceMode.Impulse);
            m_dashCoolDownTimer = m_dashCoolDown;

        }
        Debug.Log("DashCoolDownTimer = "+m_dashCoolDownTimer);
    }


    private void Walljump(Collision collision)
    {
        Vector3 normal = collision.contacts[0].normal;
        Vector3 velocity = m_rigidbody.velocity;
        
        if (!m_isGrounded && Vector3.Angle(velocity, -normal) < m_walljumpAngle)
        {
            if (Input.GetButtonDown("Jump"))
            {
                //Jump at walls
            }
        }

    }

    private void HealthManager(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            m_health =-30f;
        }
    }



    private void GroundCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position+new Vector3(0,0.05f,0), -transform.up, out hit, 0.1f))
        {
            m_currentJump = m_multiJumps;
        
        }
            Debug.DrawLine(transform.position + new Vector3(0, 0.5f), - transform.up * 0.1f, Color.green);
    }
}
