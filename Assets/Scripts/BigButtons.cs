using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BigButtons : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip buttonNoise;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void playNoise()
    {
        audioSource.PlayOneShot(buttonNoise, 1.0f);
    }

    public void toMainMenuButton()
    {
        SceneManager.LoadScene("02 Main Menu");
    }
    
    public void toTitleButton()
    {
        SceneManager.LoadScene("01 Title");
    }

    public void toLevelSelect()
    {
        SceneManager.LoadScene("03 Level Select");
    }

    public void toHowToPlay()
    {
        SceneManager.LoadScene("04 Tutorial");
    }

    public void toStory()
    {
        SceneManager.LoadScene("05 Story");
    }

    public void toLevel1()
    {
        SceneManager.LoadScene("11 Level 1");
    }
}
