using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public class WaspController : MonoBehaviour
{
    [Tooltip("Draws the shoot-radius and ray to the player in the scene")]
    public bool m_DebugView;

    [Header("General")]
    [Tooltip("The Layers that the enemy is able to see")]
    public LayerMask m_LayerMask;
    [Tooltip("The radius in which the enemy will shoot at the player")]
    public float m_shootRadius = 15f;
    [Tooltip("The ammount of Seconds before the enemy will shoot again")]
    public float m_shootCooldown = 5f;
    [Tooltip("The speed in which the bullet-prefab will fly")]
    public float m_bulletSpeed = 6f;

    [Header("Audio")]
    [Tooltip("The sound that plays while the Enemy is attacking")]
    public AudioClip m_attackSound;

    //The current cooldown of the shots of the enemy
    private float m_shootCooldownTimer;
    //The gameObject of the player
    private GameObject m_player;
    //The prefab for the Bullets
    private GameObject m_bulletPrefab;
    //The position in which the Bullet prefab will spawn
    private Transform m_bulletPoint;
    //The AudioSource which plays the Enemies sounds
    private AudioSource m_audioSource;

    private void OnValidate()
    {
        if (m_shootRadius <=0)
        {
            Debug.Log("m_shootRadius was " + m_shootRadius + "!");
            m_shootRadius = 15;
        }

        if (m_shootCooldown <0)
        {
            Debug.Log("m_shootCooldown was " + m_shootCooldown + "!");
            m_shootCooldown = 5;
        }

        if (m_bulletSpeed <= 0)
        {
            Debug.Log("m_bulletSpeed was " + m_bulletSpeed + "!");
            m_bulletSpeed = 6;
        }
    }

    void OnDrawGizmos()
    {
        if (!m_DebugView)
        {
            return;
        }
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, m_shootRadius);
    }

    void Start ()
    {
        m_player = GameObject.Find("Player");
        m_bulletPoint = transform.GetChild(0);
        m_bulletPrefab = (GameObject)Resources.Load("Prefabs/BulletPrefab");
        m_audioSource = GetComponent<AudioSource>();
    }
	
	void Update ()
    {

        if (m_shootCooldownTimer > 0)
        {
            m_shootCooldownTimer -= Time.deltaTime;
        }

        if (m_shootCooldownTimer < 0)
        {
            m_shootCooldownTimer = 0;
        }

        if (Time.frameCount % 5 == 0)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, m_player.transform.position - transform.position); 
            if (Physics.Raycast(ray, out hit, m_shootRadius, m_LayerMask))
            {
                transform.LookAt(m_player.transform);
                if (m_shootCooldownTimer == 0)
                {
                    Shoot();
                    m_shootCooldownTimer = m_shootCooldown;
                }
            }
        }

        if (m_DebugView)
            Debug.DrawRay(transform.position, m_player.transform.position - transform.position, Color.green, 0.25f);
    }

    /// <summary>
    /// Shoots the bullet-prefab in the current direction of the player
    /// </summary>
    void Shoot()
    {
        GameObject bullet = (GameObject)Instantiate(
            m_bulletPrefab,
            m_bulletPoint.position,
            m_bulletPoint.rotation);

        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * m_bulletSpeed;
        if (m_attackSound != null)
        {
            m_audioSource.PlayOneShot(m_attackSound);
        }
        Destroy(bullet, 2.0f);
    }
}
