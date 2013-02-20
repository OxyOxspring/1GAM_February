using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour
{
	private float badspawntimer = 0;
	private GameObject networkManager;
	
	// Use this for initialization
	void Start () {
		if (networkView.isMine)
		{
			networkManager = GameObject.FindGameObjectWithTag ("NetworkManager");
			light.enabled = true;
			transform.Find ("RainBox").particleSystem.Play ();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (networkView.isMine)
		{
			if (badspawntimer < 5)
			{
				badspawntimer += Time.deltaTime;
				
				GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
				
				foreach (GameObject player in players)
				{
					if (player != gameObject)
					{
						if (Vector3.Distance (transform.position, player.transform.position) < 5)	
						{
							networkManager.SendMessage ("swapSpiritForPlayer", gameObject);
							Debug.Log ("BAD SPAWN!");
							return;
						}
					}
				}
				Debug.Log (badspawntimer);
			
			} 
			
			foreach (GameObject spirit in GameObject.FindGameObjectsWithTag ("Spirit"))
			{
				// Disable light.
				spirit.light.enabled = false;
				
				// Disable light's light.
				spirit.GetComponent<Light>().enabled = false;
				
				// Disable particle system.
				spirit.GetComponent<ParticleSystem>().Stop();
			}
		}
	}
}
