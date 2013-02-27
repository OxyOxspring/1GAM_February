using UnityEngine;
using System.Collections;

public class SummoningJoinText : MonoBehaviour
{
	public GameObject networkManager;
	private int potatoes = 0;	// Number of players inside the circle.
	private float thyme = 0;	// Time until the game starts.
	private float mashed = 0;	// Total number of players and spirits.

	
	void Update ()
	{
		GameObject[] spirits = GameObject.FindGameObjectsWithTag("Spirit");
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		TextMesh textMesh = (TextMesh)GetComponent ("TextMesh");
		
		mashed = spirits.Length + players.Length;
		potatoes = 0;
		
		foreach (GameObject spirit in spirits)
		{			
			if (spirit.networkView.isMine)
			{
				transform.LookAt (new Vector3(spirit.transform.position.x, spirit.transform.position.y, spirit.transform.position.z));
				transform.Rotate (new Vector3(0, 180, 0));
			}
			
			if (Vector3.Distance (transform.position, spirit.transform.position) < 9f)
			{
				potatoes++;	
			}
		}
		
		if (Network.isServer)
		{
			if (potatoes == mashed && potatoes > 1)
			{
				thyme += Time.deltaTime;
				
				// Check the timer.
				if (thyme >= 5)
				{
					networkManager.networkView.RPC ("forceAllSpawn", RPCMode.All);
				}
			}
			else
			{
				if (thyme > 0)	
				{
					thyme -= Time.deltaTime;
				}
				else
				{
					thyme = 0;	
				}
			}			
		}
		
		if (mashed == 1)
		{
			textMesh.text = "Waiting for souls...";
		}
		else
		{
			textMesh.text = potatoes.ToString () + "/" + mashed.ToString ();
		}
	}
}
