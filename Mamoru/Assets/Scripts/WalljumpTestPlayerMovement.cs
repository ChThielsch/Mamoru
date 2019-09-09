using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WalljumpTestPlayerMovement : MonoBehaviour
{

    private Vector3 m_moveVector;
    private Vector3 m_lastMove;
    private CharacterController m_controller;
    private Camera m_mainCamera;
    private float m_currentJump;
    private float m_verticalVelocity;
    private float m_dashCoolDownTimer;
    private float m_invicibilityCoolDownTimer;
    private bool m_isInvicable;
    private bool m_gotKeyCard = false;

    public GameObject m_spawnPoint;
    public Canvas m_mainCanvas;
    public TMP_Text m_healthText;
    public float m_health = 100;
    public float m_movementSpeed = 8;
    public float m_jumpForce = 8;
    public float m_gravity = 25;
    public float m_dashForce = 10;
    public float m_dashCooldown = 5;
    public float m_invincebilityCooldown = 2;
    [Range(1f,5f)]
    public byte m_multiJumps = 2;


    void Start ()
    {
        m_controller = GetComponent<CharacterController>();
        m_mainCamera = Camera.main;
        m_currentJump = m_multiJumps;
	}
	
	void Update ()
    {
        ApplyInputs();
        UIManager();
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        WallJump(hit);
        HealthManager(hit);
        RespawnPlayer(hit);
        DashWallManager(hit);

    }

    private void ApplyInputs()
    {
        m_moveVector = transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal");

        //Camera positions
        Vector3 camF = m_mainCamera.transform.forward;
        Vector3 camR = m_mainCamera.transform.right;
        camF.y = 0;
        camR.y = 0;
        camF = camF.normalized;
        camR = camR.normalized;

        if (m_moveVector.magnitude > 0.001f )
        {
            transform.forward = camF;
        }

        //Ground Check
        if (m_controller.isGrounded)
        {
            m_verticalVelocity = -1;
            m_currentJump = m_multiJumps;
        }
        else
        {
            m_verticalVelocity -= m_gravity * Time.deltaTime;
            m_moveVector = m_lastMove;
            
        }

        //Jump
        if (Input.GetButtonDown("Jump") && m_currentJump > 0)
        {
            m_verticalVelocity = m_jumpForce;
            m_currentJump--;
        }

        //Dash
        if (m_dashCoolDownTimer > 0)
        {
            m_dashCoolDownTimer -= Time.deltaTime;
            m_invicibilityCoolDownTimer -= Time.deltaTime;
            m_isInvicable = true;
        }

        if (m_dashCoolDownTimer < 0)
        {
            m_dashCoolDownTimer = 0;
        }

        if (m_invicibilityCoolDownTimer <0)
        {
            m_invicibilityCoolDownTimer = 0;
            m_isInvicable = false;
        }

        if (Input.GetButtonDown("Fire1") && m_dashCoolDownTimer == 0)
        {   
            m_controller.Move(transform.forward* m_dashForce * Time.deltaTime);
            m_dashCoolDownTimer = m_dashCooldown;
            m_invicibilityCoolDownTimer = m_invincebilityCooldown;
        }
       

        //Moving the Player

        m_moveVector.y = 0;
        m_moveVector.Normalize();
        m_moveVector *= m_movementSpeed;
        m_moveVector.y = m_verticalVelocity;


        m_controller.Move(m_moveVector * Time.deltaTime);
        Debug.DrawLine(transform.position,transform.position+ m_moveVector,Color.red,0.25f);
        m_lastMove = m_moveVector;        
    }

    private void WallJump(ControllerColliderHit hit)
    {
        if (!m_controller.isGrounded && hit.normal.y < 0.1f)
        {
            if (Input.GetButtonDown("Jump"))
            {
                Debug.DrawRay(hit.point, hit.normal, Color.red, 1.25f);
                m_verticalVelocity = m_jumpForce;
                m_moveVector = hit.normal * m_movementSpeed;
                m_currentJump = 1;
            }
        }
    }

    private void HealthManager(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Enemy")
        {
            if (m_isInvicable == true)
            {
                hit.gameObject.SetActive(false);
            }
            else
            {
                m_health -= 30;
            }
        }

        if (hit.gameObject.tag == "Bullet")
        {
            m_health -= 30;
            hit.gameObject.SetActive(false);
            Destroy(hit.gameObject);
        }

        if (hit.gameObject.tag == "Health" && m_health <=100)
        {
            m_health += 30;
        }

        if (m_health <=0)
        {
            transform.position = m_spawnPoint.transform.position;
            m_health = 100;
        }
    }


    private void RespawnPlayer(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Killzone")
        {
            transform.position = m_spawnPoint.transform.position;
        }
    }

    private void DashWallManager(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Keycard")
        {
            m_gotKeyCard = true;
            hit.gameObject.SetActive(false);
        }

        if (hit.gameObject.tag == "DashWallEasy" && m_isInvicable)
        {
            hit.gameObject.SetActive(false);
        }

        if (hit.gameObject.tag == "DashWallHard" && m_gotKeyCard && m_isInvicable)
        {
            hit.gameObject.SetActive(false);
        }
    }

    private void UIManager()
    {
        m_healthText.text = "Health: " + m_health;
    }
}
