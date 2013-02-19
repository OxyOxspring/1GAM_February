using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour
{

	// Use this for initialization
	void Start () {
		if (networkView.isMine)
		{
			light.enabled = true;
			transform.Find ("RainBox").particleSystem.Play ();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		foreach (GameObject spirit in GameObject.FindGameObjectsWithTag("Spirit"))
		{
			if (!spirit.networkView.isMine)
			{
				spirit.active = false;
			}
		}
	}
}
