using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private bool m_isPaused;
    private GameObject m_pauseMenu;
    private Camera m_mainCamera;
    private AudioSource m_buttonSound;

    public CameraController cameraController;
    public GameObject m_optionsMenu;
    public GameObject m_soundMenu;
    public GameObject m_mouseMenu;
    public GameObject m_videoMenu;

    void Start()
    {
        m_mainCamera = Camera.main;
        m_buttonSound = m_mainCamera.GetComponent<AudioSource>();
        m_pauseMenu = GameObject.Find("PauseMenu");
        m_pauseMenu.SetActive(false);
        Cursor.visible = false;
        m_isPaused = false;
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Pause();
        }
    }

    public void QuitScene()
    {
        Debug.Log("Quit");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void Pause()
    {
        Debug.Log("Paused");

        if (!m_isPaused)
        {
            Time.timeScale = 0;
            m_pauseMenu.SetActive(true);
            m_isPaused = true;
            cameraController.m_isPaused = true;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1;
            Cursor.visible = false;
            m_pauseMenu.SetActive(false);
            m_optionsMenu.SetActive(false);
            m_soundMenu.SetActive(false);
            m_mouseMenu.SetActive(false);
            m_videoMenu.SetActive(false);
            m_isPaused = false;
            cameraController.m_isPaused = false;
        }
    }
}
