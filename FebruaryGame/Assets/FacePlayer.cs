using UnityEngine;
using System.Collections;

public class FacePlayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		if (players.Length > 0)
		{
			foreach (GameObject player in players)
			{
				if (player.networkView.isMine)
				{
					transform.LookAt (new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
					transform.Rotate (90, 0, 0);
				}
			}
		}	
	}
}
