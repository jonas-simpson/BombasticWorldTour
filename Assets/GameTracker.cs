using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTracker : MonoBehaviour
{
    public int teamActionCount = 5;
    public bool playerActive;

    public Text teamActionText;

	// Use this for initialization
	void Start ()
    {
        teamActionCount = 5;
        playerActive = true;

        SetActionText();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void SetActionText()
    {
        teamActionText.text = "Team Actions: " + teamActionCount.ToString();
    }
}
