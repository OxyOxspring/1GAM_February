using UnityEngine;
using System.Collections;

public class SpiritScript : MonoBehaviour
{
	public GameObject TetheredObject;
	public float FixedYPosition = 0;
	private float waveTimer = 0;
	
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
			
			// Invis-players.
			foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
			{
				// Mesh is necessary for players as spirits can be tethered to them. Makes them invisible NOT nonexistent.
				MeshRenderer mesh = player.GetComponentInChildren<Transform>().GetComponentInChildren<MeshRenderer>();
				if (mesh != null)
				{
					mesh.enabled = false;
				}
			}
			
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (networkView.isMine)
		{
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
