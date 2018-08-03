using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextCopy : MonoBehaviour
{
    public Text target;
    Text output;

	// Use this for initialization
	void Start ()
    {
        //Debug.Log(target.text);
        output = gameObject.GetComponent<Text>();
        //Debug.Log(output.text);
        output.text = target.text;
    }

    // Update is called once per frame
    void Update ()
    {
        output.text = target.text;
    }
}
