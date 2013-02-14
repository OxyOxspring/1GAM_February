using UnityEngine;
using System.Collections;

public class Leaderboard : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{		
		GameObject[] spirits = GameObject.FindGameObjectsWithTag ("Spirit");
		
		int i = 0;
		foreach (Transform entry in transform)
		{
			i++;
			if (i <= spirits.Length)
			{
				entry.gameObject.SetActive(true);
			}
			else
			{
				entry.gameObject.SetActive(false);
			}
		}
	}
}
