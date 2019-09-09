using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleMenu : MonoBehaviour
{
    public GameObject m_titlePanel;
    public GameObject m_mainPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            m_mainPanel.SetActive(true);
            m_titlePanel.SetActive(false);
        }
    }
}
