using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class RigidbodyPlayerController : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("The speed in which the Player moves")]
    public float m_movementSpeed = 8;
    [Tooltip("The ammount of force applied on the Players Y coordinate")]
    public float m_jumpForce = 8;
    [Tooltip("The ammount of force applied in the direction the Player is looking")]
    public float m_dashForce = 10;
    [Tooltip("The time in seconds before the Player can dash again")]
    public float m_dashCooldown = 5;
    [Tooltip("The time in seconds in which the player is invincable after a dash")]
    public float m_invincebilityCooldown = 2;
    [Tooltip("The ammount of possible jumps between 1 and 5")]
    [Range(1f, 5f)]
    public byte m_multiJumps = 2;

    [Header("Health")]
    [Tooltip("The maximum ammount of health the player has")]
    [Range(1f, 300f)]
    public int m_maxHealth = 100;
    [Tooltip("The ammount of Damage from hitting Enemies")]
    [Range(1f, 100f)]
    public int m_enemyDamage = 30;
    [Tooltip("The ammount of Damage from hitting Bullets")]
    [Range(1f, 100f)]
    public int m_bulletDamage = 30;
    [Tooltip("The ammount of Health from colliding with an Healthpickup")]
    [Range(1f, 100f)]
    public int m_healthPickup = 40;
    [Range(1f, 30f)]
    [Tooltip("The time in seconds it takes to change the health slider")]
    public float m_TimeToUpdateHealbar;

    [Header("Audio")]
    [Tooltip("The sound which plays when the Player jumps")]
    public AudioClip m_jumpSound;
    [Tooltip("The sound which plays when the Player dashes")]
    public AudioClip m_dashSound;
    [Tooltip("The sound which plays when the Player takes damage")]
    public AudioClip m_damageSound;


    //The ammount of current jumps left;
    private float m_currentJump;
    //The current time of the dash cooldown
    private float m_dashCoolDownTimer;
    //The current time of the invincibility cooldown
    private float m_invicibilityCoolDownTimer;
    //The start value for the healthslider
    private float m_startValue;
    //The goal value for the healthslider
    private float m_goalValue;
    //The time the healthslider takes to lerp
    private float m_lerpTime;
    //the current health of the Player;
    private int m_currentHP;
    //the current health of the Player as Property
    private int CurrentHP
    {
        get
        {
            return m_currentHP;
        }
        set
        {
            if (m_currentHP == value)
            {
                return;
            }
            m_currentHP = Mathf.Clamp(value, 0, m_maxHealth);
            HPChanged(m_currentHP / (m_maxHealth * 1.0f));
        }
    }
    //The current score of the player
    private int m_currentScore = 0;
    //The maximum score of the player
    private int m_maxScore;
    //Checks if the player is currently invincable
    private bool m_isInvicable;
    //Checks if the player got the "Keycard"
    private bool m_gotKeyCard = false;
    //Checks if the player is grounded
    private bool m_isGrounded = false;

    //The vector in which the player is going to move
    private Vector3 m_moveVector;
    //The last Object Taged as "Ground" the Player has touched;
    private Vector3 m_lastGround;
    //The Rigidbody of the Player-GameObject
    private Rigidbody m_rigidbody;
    //The main Camera in the Scene;
    private Camera m_mainCamera;
    //The position where the player will respawn
    private GameObject m_spawnPoint;
    //The Image of the Keycard within the UI
    private GameObject m_keycardImage;
    //The main canvas of the UI
    private Canvas m_mainCanvas;
    //The AudioSource which plays the players Sounds
    private AudioSource m_audioSource;
    //The Slider that shows the current health
    private Slider m_healthSlider;
    //The Text that shows the current score
    private TMP_Text m_scoreText;

    private void OnValidate()
    {
        if (m_maxHealth <= 0)
        {
            Debug.Log("m_health was " + m_maxHealth + "!");
            m_maxHealth = 100;
        }

        if (m_movementSpeed <= 0)
        {
            Debug.Log("m_movementSpeed was " + m_movementSpeed + "!");
            m_movementSpeed = 8;
        }

        if (m_jumpForce <= 0)
        {
            Debug.Log("m_jumpForce was " + m_jumpForce + "!");
            m_movementSpeed = 8;
        }

        if (m_dashForce <= 0)
        {
            Debug.Log("m_dashForce was " + m_dashForce + "!");
            m_dashForce = 12;
        }

        if (m_dashCooldown <= 0)
        {
            Debug.Log("m_dashCooldown was " + m_dashCooldown + "!");
            m_dashCooldown = 5;
        }

        if (m_invincebilityCooldown <= 0)
        {
            Debug.Log("m_invincebilityCooldown was " + m_invincebilityCooldown + "!");
            m_invincebilityCooldown = 2;
        }
    }

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_spawnPoint = GameObject.Find("SpawnPoint");
        m_mainCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        m_healthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>();
        m_scoreText = GameObject.Find("ScoreText").GetComponent<TMP_Text>();
        m_mainCamera = Camera.main;
        m_audioSource = GetComponent<AudioSource>();
        m_currentJump = m_multiJumps;
        m_keycardImage = GameObject.Find("KeyCardImage");
        m_keycardImage.SetActive(false);
        m_maxScore = GameObject.FindGameObjectsWithTag("Berry").Length;
        CurrentHP = m_maxHealth;
    }

    void Update()
    {
        GroundCheck();
        if (m_isGrounded)
        {
            MovementInput();
        }
        JumpInput();
        DashInput();
        WallJumpInput();


        m_healthSlider.value = Mathf.Lerp(m_startValue, m_goalValue, m_lerpTime);
        m_lerpTime += Time.deltaTime / m_TimeToUpdateHealbar;
    }

    private void OnCollisionEnter(Collision collision)
    {
        DamageManager(collision);
        RespawnPlayer(collision);
        UIManager();
    }

    private void OnTriggerEnter(Collider other)
    {
        ScoreManager(other);
        HealthManager(other);
        DashWallManager(other);
        UIManager();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Bullet" || other.gameObject.tag == "Player")
        {
            return;
        }
        m_isGrounded = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Ground" || other.tag == "Platform")
        {

            if (m_rigidbody.velocity.y <= 0)
            {
                if (other.gameObject.tag == "Bullet" || other.gameObject.tag == "Player")
                {
                    return;
                }
                m_isGrounded = true;
            }
        }
    }


    #region MyMethods
    /// <summary>
    /// Applies the X and Z forces to the player
    /// </summary>
    private void MovementInput()
    {
        m_moveVector = transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal");

        Vector3 camF = m_mainCamera.transform.forward;
        camF.y = 0;
        camF = camF.normalized;

        if (m_moveVector.magnitude > 0.001f)
        {
            transform.forward = camF;
        }

        m_moveVector.Normalize();
        m_moveVector *= m_movementSpeed;
        m_moveVector.y = 0;
        m_moveVector.y = m_rigidbody.velocity.y;

        m_rigidbody.velocity = m_moveVector;
    }

    /// <summary>
    /// Applies the Y force to the player
    /// </summary>
    private void JumpInput()
    {

        if (Input.GetButtonDown("Jump") && m_currentJump > 0)
        {
            m_isGrounded = false;
            Vector3 force = Vector3.zero;
            force.y = 0.01f;
            m_rigidbody.velocity = force;
            Vector3 dir = transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal");
            force = dir;
            force.y = 0;
            force = Vector3.up + force;

            m_rigidbody.AddForce(force.normalized * m_jumpForce, ForceMode.VelocityChange);
            m_currentJump--;

            if (m_jumpSound != null)
            {
                m_audioSource.PlayOneShot(m_jumpSound);
            }
        }

    }

    /// <summary>
    /// Applies the Dash force in the direction the player is looking
    /// </summary>
    private void DashInput()
    {
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

        if (m_invicibilityCoolDownTimer < 0)
        {
            m_invicibilityCoolDownTimer = 0;
            m_isInvicable = false;
        }

        if (Input.GetButtonDown("Fire1") && m_dashCoolDownTimer == 0)
        {
            m_rigidbody.AddForce(transform.forward * m_dashForce, ForceMode.Impulse);
            m_dashCoolDownTimer = m_dashCooldown;
            m_invicibilityCoolDownTimer = m_invincebilityCooldown;

            if (m_dashSound != null)
            {
                m_audioSource.PlayOneShot(m_dashSound);
            }
        }
    }


    /// <summary>
    /// Checks if the player is currently touching the ground
    /// </summary>
    private void GroundCheck()
    {
        if (m_isGrounded)
        {
            m_currentJump = m_multiJumps;
        }
        if (transform.position.y <= -70)
        {
            transform.position = m_spawnPoint.transform.position;
            CurrentHP = m_maxHealth;
        }
    }

    /// <summary>
    /// Checks if the player is currently touching a wall and applies a force in the oposide direction if so
    /// </summary>
    private void WallJumpInput()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), transform.forward, out hit, 1.5f))
        {
            if (!m_isGrounded) //&& hit.normal.y < 0.1f
            {
                if (Input.GetButtonDown("Jump"))
                {
                    transform.forward = transform.position - hit.point;
                    Quaternion rotation = transform.rotation;
                    rotation.x = 0;
                    rotation.z = 0;
                    m_rigidbody.AddForce(((transform.position - hit.point).normalized * m_jumpForce) + transform.up, ForceMode.VelocityChange);
                    transform.rotation = rotation;

                    if (m_jumpSound != null)
                    {
                        m_audioSource.PlayOneShot(m_jumpSound);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Manages the ammount of health the player is againing and loosing
    /// </summary>
    /// <param name="collision"></param>
    private void DamageManager(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (m_isInvicable == true)
            {
                collision.gameObject.SetActive(false);
            }
            else
            {
                CurrentHP -= m_enemyDamage;
                if (m_damageSound != null)
                {
                    m_audioSource.PlayOneShot(m_damageSound);
                }
            }
        }

        if (collision.gameObject.tag == "Bullet")
        {
            CurrentHP -= m_bulletDamage;
            if (m_damageSound != null)
            {
                m_audioSource.PlayOneShot(m_damageSound);
            }
        }

        if (CurrentHP <= 0)
        {
            transform.position = m_spawnPoint.transform.position;
            CurrentHP = m_maxHealth;
        }
    }

    /// <summary>
    /// Respawns the player to the last touched "Ground" Object
    /// </summary>
    /// <param name="collision"></param>
    private void RespawnPlayer(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            m_lastGround = transform.position;
        }
        if (collision.gameObject.tag == "Killzone")
        {
            if (CurrentHP > 0)
            {
                m_audioSource.PlayOneShot(m_damageSound);
                transform.position = m_lastGround;
                CurrentHP -= m_enemyDamage;
            }
        }
    }

    /// <summary>
    /// Checks if the player is dashing at a dashable wall and will disable it if so
    /// </summary>
    /// <param name="collision"></param>
    private void DashWallManager(Collider other)
    {
        if (other.gameObject.tag == "Keycard")
        {
            m_gotKeyCard = true;
            m_keycardImage.SetActive(true);
        }

        if (other.gameObject.tag == "DashWallEasy" && m_isInvicable)
        {
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.tag == "DashWallHard" && m_gotKeyCard && m_isInvicable)
        {
            other.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Checks if the player is entering a pickup trigger
    /// </summary>
    /// <param name="other"></param>
    private void ScoreManager(Collider other)
    {
        if (other.gameObject.tag == "Berry")
        {
            m_currentScore++;
        }
    }

    /// <summary>
    /// Checks if the player is entering a health trigger
    /// </summary>
    /// <param name="other"></param>
    private void HealthManager(Collider other)
    {
        if (other.gameObject.tag == "Health")
        {
            CurrentHP += m_healthPickup;
            if (CurrentHP > m_maxHealth)
            {
                CurrentHP = m_maxHealth;
            }
        }
    }

    /// <summary>
    /// Manages the variables showen in the UI
    /// </summary>
    private void UIManager()
    {
        m_scoreText.text = m_currentScore + "/" + m_maxScore;
    }

    /// <summary>
    /// Changes the Healthpoints relative to the current health
    /// </summary>
    /// <param name="_newPercentage"></param>
    public void HPChanged(float _newPercentage)
    {
        m_startValue = m_healthSlider.value;
        m_goalValue = _newPercentage;
        m_lerpTime = 0;
    }

    #endregion
}
