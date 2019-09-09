using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFade : MonoBehaviour {

    private Animator m_animator;
    private int m_levelToLoad;

	void Start ()
    {
        m_animator = GetComponent<Animator>();
	}
	
    public void FadeLevel(int levelIndex)
    {
        m_animator.SetTrigger("FadeOut");
        m_levelToLoad = levelIndex;
    }

    public void onFadeComplete()
    {
        SceneManager.LoadScene(m_levelToLoad);
    }


}
