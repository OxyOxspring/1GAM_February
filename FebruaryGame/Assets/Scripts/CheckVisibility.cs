using UnityEngine;
using System.Collections;

public class CheckVisibility : MonoBehaviour
{
	public Camera CameraObject;
	public AudioSource InsaneNoise;
	public float CameraShake = 0.2f;
	public float Insanity = 0;
	public float InsanityRate = 20;
	public bool IsLookingAtSomeone = false;
	public Color SafeColour;
	public Color AlarmColour;
	public int audiolevel = 0;
	public float Hunger = 50;
	public float HungerRate = 0.6f;
	private float heart1 = 0;
	private float heart2 = 2 * Mathf.PI * 0.4f;
	private float heart3 = 0;
	
	public float InsanityProgress
	{
		get { return Insanity / 100; }
	}
	
	public float HungerProgress
	{
		get { return Hunger / 100; }	
	}
	
	public bool Dead
	{
		get { return Insanity >= 100 || Hunger >= 100; }
	}
	
	public static bool IsTransformLit(Transform transform)
	{
		// Find all lights labelled Light.
		GameObject[] pointlights = GameObject.FindGameObjectsWithTag("Light");
		
		foreach (GameObject pointlight in pointlights)
		{
			// Check if the transform is in the range of a light.
			if (Vector3.Distance (transform.position, pointlight.transform.position) < pointlight.light.range + transform.localScale.x * 0.5f)
			{
				// Check shadows. (May have to check from top of model not centre).
				if (Physics.Linecast (transform.position + new Vector3(0, transform.localScale.y * 0.9f, 0), pointlight.transform.position, 1) == false || 
					Physics.Linecast (transform.position - new Vector3(0, transform.localScale.y * 0.9f, 0), pointlight.transform.position, 1) == false ||
					Physics.Linecast (transform.position, pointlight.transform.position, 1) == false)
				{
					Debug.DrawLine (transform.position + new Vector3(0, transform.localScale.y * 0.9f, 0), pointlight.transform.position);
					Debug.DrawLine (transform.position - new Vector3(0, transform.localScale.y * 0.9f, 0), pointlight.transform.position);
					Debug.DrawLine (transform.position, pointlight.transform.position);
					return true;
				}
			}
		}
		
		return false;
	}
	
	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
		//Debug.Log (IsTransformLit (transform) ? "I'm in the light!!" : "I'm not in the light!!");
		
		if (networkView.isMine)
		{			
			HandleVision ();
			
			HandleSanity ();
			
			HandleHunger();
			
			InsaneNoise.volume = (float)audiolevel / 100;
			
			if(audiolevel > 100)
			{
				audiolevel = 100;
			}
			if(audiolevel < 0)
			{
				audiolevel = 0;
				InsaneNoise.volume = 0;
			}
		}
	}
	
	void HandleVision()
	{
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(CameraObject);
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
			
		// Reset the isLookingAtSomeone variable.
		IsLookingAtSomeone = false;
		
		foreach (GameObject otherPlayer in players)
		{
			/* Order tests by increasing computational complexity. */
			
			// Ignore self.
			if (otherPlayer.transform != transform)
			{
				// Check if the other player is in range or lit by a point light.
				if (Vector3.Distance (transform.position, otherPlayer.transform.position) < 13f || IsTransformLit (otherPlayer.transform))
				{
					// Check if the other player is within the view of this player's camera view.
					if (GeometryUtility.TestPlanesAABB (planes, otherPlayer.collider.bounds))
					{	
						// Check if the other player is behind a surface (default layer 1 to ignore players)
						if (Physics.Linecast (transform.position, otherPlayer.transform.position + new Vector3(0, otherPlayer.transform.localScale.y * 0.9f, 0), 1) == false ||
							
							Physics.Linecast (transform.position, otherPlayer.transform.position, 1) == false)
						{
							IsLookingAtSomeone = true;
							Debug.DrawLine (transform.position, otherPlayer.transform.position + new Vector3(0, otherPlayer.transform.localScale.y * 0.9f, 0));
							//Debug.DrawLine (transform.position, otherPlayer.transform.position - new Vector3(0, otherPlayer.transform.localScale.y * 0.9f, 0));
							Debug.DrawLine (transform.position, otherPlayer.transform.position);
							break;
						}
					}
				}
			}
		}
	}
	
	void HandleSanity()
	{		
		// If the player is looking at someone, increase their insanity, else decrease the insanity until it reaches zero.
		if (IsLookingAtSomeone)
		{			
			Insanity += Time.deltaTime * InsanityRate;
			audiolevel += Mathf.CeilToInt(Time.deltaTime) * 2;
		}
		else
		{
			if (Insanity > 0)
			{
				Insanity -= Time.deltaTime * InsanityRate;
				audiolevel -= Mathf.CeilToInt(Time.deltaTime);
			}
			else
			{
				Insanity = 0;
				audiolevel = 0;
			}
		}
		
		light.color = Color.Lerp (SafeColour, AlarmColour, InsanityProgress);
		
		// Shake the camera more as the player gets more scared.
		CameraObject.transform.localPosition = new Vector3 (Random.Range (-CameraShake, CameraShake) * InsanityProgress, Random.Range (-CameraShake, CameraShake) * InsanityProgress, 0);
	}
	
	void HandleHunger()
	{	
		ScreenOverlay[] over = transform.Find ("Camera").GetComponents<ScreenOverlay>();
		Vignetting vig = transform.Find ("Camera").GetComponent<Vignetting>();
		Fisheye fish = transform.Find ("Camera").GetComponent<Fisheye>();
		
		//Heartbeat Stuff:
		if (heart1 >= Mathf.PI * 2)
		{
			heart1 = 0;
		}
		else
		{
			heart1 += Time.deltaTime * 3;
		}
		
		if (heart2 >= Mathf.PI * 2)
		{
			heart2 = 0;
		}
		else
		{
			heart2 += Time.deltaTime * 3;
		}	
		
		if (heart3 >= Mathf.PI * 2)
		{
			heart3 = 0;
		}
		else
		{
			heart3 += Time.deltaTime * 1.5f;	
		}
		
		float sinheart = Mathf.Cos (heart1 / 2);
		
		float heartsin = 0;
		
		if (sinheart > 0)
		{
			heartsin = Mathf.Max(0, Mathf.Max(Mathf.Sin(heart1), Mathf.Sin (heart2)));
		}
		
		if (HungerProgress > 0.25f)
		{
			float modhung = HungerProgress - 0.25f;
			
			over[0].intensity = modhung * 0.8f + 0.4f * heartsin;
			over[1].intensity = modhung * 0.8f - 0.4f * heartsin;
			vig.intensity = 5 + modhung * 3 + 2 * heartsin;
			vig.blur = 1;
			fish.strengthX = fish.strengthY = 0.15f + modhung * 0.75f + (modhung / 6) * heartsin + 0.02f * Mathf.Sin (heart1);;
		}
		else
		{
			over[0].intensity = 0;
			over[1].intensity = 0;
			vig.intensity = 5 ;
			vig.blur = 0;
			fish.strengthX = fish.strengthY = 0.15f;
		}

		
		if (HungerProgress < 1)	
		{
			Hunger += Time.deltaTime * HungerRate;

			
			if (HungerProgress >= 0.25)
			{
				if (transform.GetComponent<CharacterMotor>().GetCrouched() == true)
				{
					foreach (GameObject corpse in GameObject.FindGameObjectsWithTag("Corpse"))
					{
						// If the player is over a corpse, eat it.
						if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(corpse.transform.position.x, 0, corpse.transform.position.z)) < 1.0f)
						{
							Hunger = 0;
							
							corpse.networkView.RPC ("Nom", RPCMode.All, 1);
						}
					}
				}
			}
		}
		
		
	}
}
