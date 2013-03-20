using UnityEngine;
using System.Collections;

public class SpiritScript : MonoBehaviour
{
	public GameObject TetheredObject;
	public float FixedYPosition = 0;
	private float waveTimer = 0;
	private float fadeTimer = 0;
	private float fadeDuration = 3;
	
	// Use this for initialization
	void Start ()
	{		
		FixedYPosition = transform.position.y;
		if (networkView.isMine)
		{
			audio.Stop();
			
			foreach (GameObject spirit in GameObject.FindGameObjectsWithTag ("Spirit"))
			{
				// Disable light.
				spirit.light.enabled = true;
				
				// Disable light's light.
				spirit.GetComponent<Light>().enabled = true;
				
				// Disable particle system.
				spirit.GetComponent<ParticleSystem>().Play();
				
			}
			
			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
			
			if (players.Length > 1)
			{
				// Invis-players.
				foreach (GameObject player in players)
				{
					// Mesh is necessary for players as spirits can be tethered to them. Makes them invisible NOT nonexistent.
					MeshRenderer mesh = player.GetComponentInChildren<Transform>().GetComponentInChildren<MeshRenderer>();
					if (mesh != null)
					{
						mesh.enabled = false;
					}
				}
			}
			else
			{
				GameObject[] corpses = GameObject.FindGameObjectsWithTag("Corpse");
				
				foreach (GameObject corpse in corpses)
				{
					Destroy (corpse);
					// Need to do this for all players so take it out of the networkview.ismine and use a flag.
				}
				fadeTimer = fadeDuration;				
			}			
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (networkView.isMine)
		{
			if (transform.position.y > 14f)
			{
				if (fadeTimer > 0)
				{
					fadeTimer -= Time.deltaTime;
					GetComponentInChildren<ScreenOverlay>().intensity = fadeTimer / fadeDuration;
				}
				else
				{
					GetComponentInChildren<ScreenOverlay>().intensity = 0;
					fadeTimer = 0;	
				}
			}
			
			
			waveTimer += Time.deltaTime;
			
			if (waveTimer >= Mathf.PI * 2)
			{
				waveTimer = 0;
			}
		
			// Follows the object its tethered to (so when spectating the spirit can be placed over players.
			if (TetheredObject == null)
			{
				transform.position = new Vector3(transform.position.x, FixedYPosition + 0.1f * Mathf.Sin (waveTimer), transform.position.z);
			}
			else
			{
				transform.position = new Vector3(TetheredObject.transform.position.x, TetheredObject.transform.position.y + 0.05f * Mathf.Sin (waveTimer), TetheredObject.transform.position.z);	
			}
		}
		else
		{
			if (TetheredObject != null)
			{
				transform.position = new Vector3(TetheredObject.transform.position.x, TetheredObject.transform.position.y, TetheredObject.transform.position.z);
			}
		}
	}
	
	public void Tether(GameObject tether)
	{
		TetheredObject = tether;
	}
}
