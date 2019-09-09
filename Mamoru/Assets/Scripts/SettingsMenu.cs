using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Linq;
using System.IO;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer m_audioMixer;
    public AudioClip m_buttonSound;

    [Header("Audio")]
    public Slider m_masterVolumeSlider;
    public Slider m_musicVolumeSlider;
    public Slider m_otherVolumeSlider;

    [Header("Mouse")]
    public Slider m_mouseSensibilitySlider;
    public Toggle m_mouseInvertXToggle;
    public Toggle m_mouseInvertYToggle;

    [Header("Graphics")]
    public Dropdown m_resolutionDropdown;
    public Dropdown m_qualityDropdown;
    public Toggle m_isFullscreen;

    Settings m_toSaveSettings = new Settings();
    Resolution[] resolutions
    {
        get
        {
            if (m_resolutions == null)
            {
                m_resolutions = Screen.resolutions.Where(SelectResolutions).ToArray();
            }
            return m_resolutions;
        }
    }

    private Camera m_mainCamera;
    private AudioSource ButtonSoundSource
    {
        get
        {
            if (m_buttonSoundSource == null)
            {
                m_buttonSoundSource = GameObject.Find("OtherSource").GetComponent<AudioSource>();
            }
            return m_buttonSoundSource;
        }
    }
    private AudioSource m_buttonSoundSource;
    private Resolution[] m_resolutions;

    private void Start()
    {
        m_mainCamera = Camera.main;

        LoadSettings();
 

        m_resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        m_resolutionDropdown.AddOptions(options);
        m_resolutionDropdown.value = currentResolutionIndex;
        m_resolutionDropdown.RefreshShownValue();
    }

    private bool SelectResolutions(Resolution resolution)
    {
        if (resolution.height < 600 || resolution.width < 800)
        {
            return false;
        }
        else
            return true;
    }

    public void OnEnable()
    {
        LoadSettings();
    }


    public void SetMasterVolume(float masterVolume)
    {
        m_audioMixer.SetFloat("MasterVolume", masterVolume);
        m_toSaveSettings.MasterVolume = masterVolume;
    }

    public void SetMusicVolume(float musicVolume)
    {
        m_audioMixer.SetFloat("MusicVolume", musicVolume);
        m_toSaveSettings.MusicVolume = musicVolume;
    }

    public void SetOtherVolume(float otherVolume)
    {
        m_audioMixer.SetFloat("OtherVolume", otherVolume);
        m_toSaveSettings.OtherVolume = otherVolume;
    }


    public void SetResolution(int resolutionIdex)
    {
            Resolution resolution = resolutions[resolutionIdex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            m_toSaveSettings.ResolutionIndex = resolutionIdex;
            playButtonSound();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        m_toSaveSettings.QualityIndex = qualityIndex;
        playButtonSound();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        m_toSaveSettings.Fullscreen = isFullscreen;
        playButtonSound();
    }


    public void SetMouseSensibility(float sensibility)
    {
        m_toSaveSettings.MouseSensibility = sensibility;
    }

    public void SetMouseInvertX(bool invertX)
    {
        m_toSaveSettings.MouseInvertX = invertX;
        playButtonSound();
    }

    public void SetMouseInvertY(bool invertY)
    {
        m_toSaveSettings.MouseInvertY = invertY;
        playButtonSound();
    }

    public void SaveSettings()
    {
        Debug.Log("Settings Saved");
        XMLOop.Serialize(m_toSaveSettings, "settings.xml");
        playButtonSound();
    }

    public void playButtonSound()
    {
        ButtonSoundSource.PlayOneShot(m_buttonSound);
    }

    public void LoadSettings()
    {
        if (File.Exists("settings.xml"))
        {
            m_toSaveSettings = XMLOop.Deserialize<Settings>("settings.xml");
        }
        else
        {
            m_toSaveSettings.ResolutionIndex = resolutions.Length - 1;
        }

        //Audio Settings
        m_masterVolumeSlider.value = m_toSaveSettings.MasterVolume;
        m_musicVolumeSlider.value = m_toSaveSettings.MusicVolume;
        m_otherVolumeSlider.value = m_toSaveSettings.OtherVolume;

        //Mouse Settings
        m_mouseSensibilitySlider.value = m_toSaveSettings.MouseSensibility;
        m_mouseInvertXToggle.isOn = m_toSaveSettings.MouseInvertX;
        m_mouseInvertYToggle.isOn = m_toSaveSettings.MouseInvertY;

        //Graphics Settings
        m_qualityDropdown.value = m_toSaveSettings.QualityIndex;
        m_resolutionDropdown.value = m_toSaveSettings.ResolutionIndex;
        m_isFullscreen.isOn = m_toSaveSettings.Fullscreen;
    }
}
