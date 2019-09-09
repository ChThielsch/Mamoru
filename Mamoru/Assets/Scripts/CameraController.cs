using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CameraController : MonoBehaviour
{
    // The minimum angle in the cameras y position
    private const float Y_ANGLE_MIN = 0.0f;
    // The maximum angle in the cameras y position
    private const float Y_ANGLE_MAX = 50.0f;

    [Tooltip("All layers the raycast of the camera will collide with")]
    public LayerMask m_raycastCollisionLayer;
    [Tooltip("All layers the spherecast of the the camera will collide with")]
    public LayerMask m_shperecastCollisionLayer;

    [Header("Mouse Settings")]
    [Tooltip("The maximum scroll distance of the mouse")]
    public float m_zoomMax = 10.0f;
    [Tooltip("The minimum scoll distance of the mouse")]
    public float m_zoomMin = 2.5f;

    [Space]
    [Tooltip("The sensitivity of the mouse's X coordinate")]
    public float m_sensitivityX = 4.0f;
    [Tooltip("The sensitivity of the mouse's Y coordinate")]
    public float m_sensitivityY = 1.0f;
    [Tooltip("The time in Seconds it takes to zoom the Camera out")]
    public float m_zoomTime = 5f;

    // The gameObject of the player
    private GameObject m_Player;
    // The gameObject of the victoryZoomPosition
    private GameObject m_victoryZoomPosition;
    // All Mashes of the player
    private MeshRenderer[] m_PlayerMashes;
    // The main camera
    private Camera m_Camera;
    // The transform of the main camera
    private Transform m_CameraTransform;
    //The current position of the camera
    private Vector3 m_currentPosition;
    //A reference to the levelFade script
    private LevelFade levelFade;
    //A reference to the victoryManager script
    private VictoryManager victoryManager;


    // The current distance of the camera to the player
    private float m_distance = 10.0f;
    // The current X position of the camera
    private float m_currentX = 0.0f;
    // The current Y position of the camera
    private float m_currentY = 0.0f;
    // The Amount if distance to the collided gameobject
    private float m_distanceAmount;
    // The current zoomfactor of the camera
    private float m_zoomfactor;
    // The size of the sherecast
    private float m_sphereCastRadius = 0.5f;

    private float m_StartTime = 0f;
    // Checks if the camera collides
    private bool m_collided = false;
    // Checks if the X axis of th mouse is inverted
    private bool m_invertX;
    // Checks if the Y axis of th mouse is inverted
    private bool m_invertY;
    //Checks if the player winns the game
    private bool m_victory = false;
    // Checks if the camera is paused  
    [HideInInspector]
    public bool m_isPaused = false;

    private void OnValidate()
    {
        if (m_zoomMax <= 0)
        {
            Debug.Log("m_zoomMax was " + m_zoomMax + "!");
            m_zoomMax = 10f;
        }

        if (m_zoomMin < 0)
        {
            Debug.Log("m_zoomMin was " + m_zoomMin + "!");
            m_zoomMin = 2.5f;
        }

        if (m_sensitivityX <= 0)
        {
            Debug.Log("m_sensitivityX was " + m_sensitivityX + "!");
            m_sensitivityX = 4f;
        }

        if (m_sensitivityY <= 0)
        {
            Debug.Log("m_sensitivityY was " + m_sensitivityY + "!");
            m_sensitivityY = 1f;
        }
    }

    void Start()
    {
        m_Player = GameObject.Find("Player");
        m_victoryZoomPosition = GameObject.Find("VictoryCameraPosition");
        levelFade = GameObject.Find("Fade_Image").GetComponent<LevelFade>();
        victoryManager = GameObject.Find("VictoryTrigger").GetComponent<VictoryManager>();
        m_zoomfactor = m_zoomMax;
        m_CameraTransform = transform;
        m_Camera = Camera.main;
        m_PlayerMashes = m_Player.GetComponentsInChildren<MeshRenderer>();
        ApplySettings();
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = new Ray(m_Player.transform.position + new Vector3(0,1,0), m_CameraTransform.position - m_Player.transform.position);
        if (Physics.SphereCast(ray, m_sphereCastRadius, out hit, m_zoomfactor, m_shperecastCollisionLayer))
        {
            m_distance = Mathf.Max(0f, hit.distance);
            m_collided = true;
        }
        else
        if ((Physics.Raycast(ray, out hit, m_zoomfactor, m_raycastCollisionLayer)))
        {
            m_distance = Mathf.Max(0f, hit.distance);
            m_collided = true;
        }
        else
        {
            m_collided = false;
        }

        if (!m_collided)
        {
            m_distanceAmount += 0.1f * Time.deltaTime;
            m_distance = Mathf.Lerp(m_distance, m_zoomfactor, m_distanceAmount);
        }

        if (m_distance <= 1.5f)
        {
            foreach (MeshRenderer PlayerRederer in m_PlayerMashes)
            {
                PlayerRederer.enabled = false;
            }
        }
        else
        {
            foreach (MeshRenderer PlayerRederer in m_PlayerMashes)
            {
                PlayerRederer.enabled = true;
            }
        }

        if (!m_isPaused)
        {
            if (m_invertX)
            {
                m_currentX -= (Input.GetAxis("Mouse X") * m_sensitivityX);
            }
            else
                m_currentX += (Input.GetAxis("Mouse X") * m_sensitivityX);

            if (m_invertY)
            {
                m_currentY -= (Input.GetAxis("Mouse Y") * m_sensitivityY);
            }
            else
                m_currentY += (Input.GetAxis("Mouse Y") * m_sensitivityY);

            float m_scroll = -Input.GetAxis("Mouse ScrollWheel") * 5;

            m_currentY = Mathf.Clamp(m_currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
            m_zoomfactor = Mathf.Clamp(m_zoomfactor += m_scroll, m_zoomMin, m_zoomMax);
        }

        if (m_victory)
        {
            m_StartTime += Time.deltaTime * (Time.timeScale / m_zoomTime);
            m_CameraTransform.position = Vector3.Slerp(m_currentPosition, m_victoryZoomPosition.transform.position, m_StartTime);
        }

    }

    private void LateUpdate()
    {
        if (!m_victory)
        {
            Vector3 m_direction = new Vector3(0, 0.25f, -m_distance);
            Quaternion m_rotation = Quaternion.Euler(m_currentY, m_currentX, 0);
            m_CameraTransform.position = m_Player.transform.position + m_rotation * m_direction;
        }
        m_CameraTransform.LookAt(m_Player.transform.position);
    }

    /// <summary>
    /// Applies the Mouse-Settings of the XML-File to the variables of the script.
    /// </summary>
    public void ApplySettings()
    {
        Settings m_savedSettings = XMLOop.Deserialize<Settings>("settings.xml");

        m_sensitivityX = m_savedSettings.MouseSensibility * 4;
        m_sensitivityY = m_savedSettings.MouseSensibility;
        m_invertX = m_savedSettings.MouseInvertX;
        m_invertY = m_savedSettings.MouseInvertY;
    }

    public void VictoryZoomOut()
    {
        m_currentPosition = m_CameraTransform.position;
        m_victory = true;
        levelFade.FadeLevel(2);
    }
}
