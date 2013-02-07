using UnityEngine;
using System.Collections;

public class Peeping : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (renderer.isVisible)
		{
			GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
			
			float closestDistance = 5;	// Do not look for players further than closestDistance metres away.
			GameObject closestPlayer = null;
			
			// Loop through all players in the scene.
			foreach (GameObject player in players)
			{
				// Compare the distances to find the closest player.
				float distance = Vector3.Distance (transform.position, player.transform.position);
				if (distance < closestDistance)
				{
					// Check the player can be seen by the eye.
					if (Physics.Linecast (transform.position + transform.up * 0.3f, player.transform.position, 1) == false)
					{
						closestDistance = distance;
						closestPlayer = player;
					}
				}
			}
			
			// Check that a player has been found nearby.
			if (closestPlayer != null)
			{
				transform.LookAt(closestPlayer.transform.position + new Vector3(0, closestPlayer.transform.localScale.y * 0.03f, 0));
				
				transform.Rotate (new Vector3(90, 0, 0));
				
				/* TOO QUICK? SHOULD ROTATE TOWARDS PLAYER SLOWLY? */
			}
		}	
	}
}
