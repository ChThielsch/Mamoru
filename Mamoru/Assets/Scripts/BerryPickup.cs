using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public class BerryPickup : MonoBehaviour
{
    [Tooltip("The sound which plays when pickedup")]
    public AudioClip m_pickupSound;
    [Tooltip("The particle System which will play when pickedup. Should NOT loop!")]
    public ParticleSystem m_pickupParticleSystem;
    [Tooltip("The particle System that will always play. Should loop!")]
    public ParticleSystem m_idleParticleSystem;
    [Space]
    [Tooltip("The speed in which the object will Rotate")]
    [Range(0,100)]
    public float m_rotationSpeed;
    [Tooltip("The distance the object will hover in")]
    [Range(0, 10)]
    public float m_amplitude;
    [Tooltip("The frequency the object takes to hover")]
    [Range(0, 10)]
    public float m_frequency;

    //the AudioSource of the GameObject
    private GameObject m_audioSource;
    //the position Offfset of the Gameobject
    private Vector3 m_positionOffset = new Vector3();
    //the temporary position of the Gameobject
    private Vector3 m_tempPosition = new Vector3();

    void Start()
    {
        m_audioSource = (GameObject)Resources.Load("Prefabs/AudioSourcePrefab");
        m_positionOffset = transform.position;
        transform.Rotate(0, Random.Range(-180,180), 0);

        if (m_idleParticleSystem != null)
        {
            ParticleSystem newparticleSystem = Instantiate(m_idleParticleSystem, transform.position, this.transform.rotation);
            newparticleSystem.transform.parent = transform;
            newparticleSystem.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void Update()
    {
        transform.Rotate(0, m_rotationSpeed*Time.deltaTime, 0);
        m_tempPosition = m_positionOffset;
        m_tempPosition.y += Mathf.Sin(Time.fixedTime * Mathf.PI * (m_frequency/10)) * (m_amplitude/10);

        transform.position = m_tempPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (m_pickupSound != null)
            {
                GameObject audioSource = (GameObject)Instantiate(m_audioSource, transform.position, this.transform.rotation);
                audioSource.GetComponent<AudioSource>().PlayOneShot(m_pickupSound);
                Destroy(audioSource, m_pickupSound.length + 0.1f);
            }
            if (m_idleParticleSystem != null)

                gameObject.SetActive(false);

            if (m_pickupParticleSystem != null)
            {
                ParticleSystem newparticleSystem = Instantiate(m_pickupParticleSystem, transform.position, this.transform.rotation);
                Destroy(newparticleSystem.gameObject, m_pickupParticleSystem.main.duration + m_pickupParticleSystem.main.startLifetimeMultiplier);
            }
            Destroy(this.gameObject);
        }
    }
}
