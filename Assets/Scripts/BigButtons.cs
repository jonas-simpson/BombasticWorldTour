using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BigButtons : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void toMainButton()
    {
        SceneManager.LoadScene("02 Main");
    }
    
    public void toTitleButton()
    {
        SceneManager.LoadScene("01 Title");
    }
}
