using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class transfomParent : MonoBehaviour
{
    public GameObject character;
    public Vector3 offset;

	// Use this for initialization
	void Start ()
    {
        transform.position = character.transform.position + offset;
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.position = character.transform.position + offset;
    }
}
