using UnityEngine;
using System.Collections;

public class Peeping : MonoBehaviour {
	
	public float xModifier;
	public float yModifier;
	public float zModifier;
	
	private Quaternion uncorrectedRotation;

	// Use this for initialization
	void Start ()
	{
		uncorrectedRotation = transform.rotation;
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
				Quaternion newRotation = Quaternion.LookRotation(closestPlayer.transform.position - transform.position,Vector3.up); 
				
				// Smoothly rotate towards the target .
				transform.rotation = uncorrectedRotation = Quaternion.Slerp(uncorrectedRotation, newRotation, 2*Time.deltaTime);
				
				// Apply the rotation fix.
				transform.Rotate (new Vector3(90, 0, 0));
			}
		}	
	}
}
