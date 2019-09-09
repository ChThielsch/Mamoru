using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditScroller : MonoBehaviour
{
    [Tooltip("The Music which will play in the background")]
    public AudioSource m_music;
    //the speed in which the music play and text will move
    private float m_speedScale = 1;

    void Update()
    {
        m_speedScale += Input.GetAxis("Mouse ScrollWheel");
        m_speedScale = Mathf.Clamp(m_speedScale,0.25f,2);
        
        transform.Translate(Vector3.up * Time.deltaTime * (m_speedScale*m_speedScale*50));
        m_music.pitch = 0.5f*m_speedScale+0.5f;

        if (Input.GetButton("Fire1"))
        {
            SceneManager.LoadScene(0);
        }
    }
}
