using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class HealthPickup : MonoBehaviour
{
    //the AudioSource of the GameObject
    private AudioSource m_audioSource;

    [Tooltip("The sound which plays when pickedup")]
    public AudioClip m_pickupSound;
    [Tooltip("The particle System which will play when pickedup. Should NOT loop!")]
    public ParticleSystem m_pickupParticleSystem;
    [Tooltip("The particle System that will always play. Should loop!")]
    public ParticleSystem m_idleParticleSystem;

    // Start is called before the first frame update
    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        if (m_idleParticleSystem != null)
        {
            ParticleSystem newparticleSystem = Instantiate(m_idleParticleSystem, transform.position, this.transform.rotation);
            newparticleSystem.transform.parent = gameObject.transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (m_pickupSound != null)
            {
                m_audioSource.PlayOneShot(m_pickupSound);
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
