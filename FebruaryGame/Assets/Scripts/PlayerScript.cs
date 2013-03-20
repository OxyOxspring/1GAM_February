using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour
{
	private float badspawntimer = 0;
	private GameObject networkManager;
	public bool HasBeenTethered = false;
	private float distanceToRealm = 0;
	public GameObject HolyLight;
	
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
							return;
						}
					}
				}	
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
			
			if (distanceToRealm != 0)
			{
				float progress = Mathf.Max(1 - ((22 - transform.position.y) / distanceToRealm), 0);
				ScreenOverlay[] overlays = transform.GetComponentsInChildren<ScreenOverlay>();
				overlays[0].intensity = 0;
				overlays[1].intensity = 0;
				overlays[2].intensity = 0;
				overlays[3].intensity = progress * 1f;
				transform.GetComponent<CharacterController>().enabled = false;
				transform.position += Vector3.up * Time.deltaTime * 1.25f;
				RenderSettings.fogDensity = 0;
			}
		}
	}
	
	public void Win()
	{
		this.GetComponentInChildren<Light>().enabled = false;

		transform.FindChild ("RainBox").particleSystem.Stop ();
		transform.FindChild ("Fog").particleSystem.Stop ();
		transform.FindChild ("WibblyWobbly").particleSystem.Play();
		// Set hunger to 0
		// Set insanity to 0
		
		if (distanceToRealm == 0)
		{
			distanceToRealm = 18 - transform.position.y;
			Instantiate (HolyLight, new Vector3(transform.position.x, 22, transform.position.z), Quaternion.identity);
		}
	}
}
