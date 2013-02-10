using UnityEngine;
using System.Collections;

public class CheckVisibility : MonoBehaviour
{
	public Camera CameraObject;
	public AudioSource InsaneNoise;
	public float CameraShake = 0.2f;
	public float Insanity = 0;
	public float InsanityRate = 80;
	public bool IsLookingAtSomeone = false;
	public Color SafeColour;
	public Color AlarmColour;
	public int audiolevel = 0;
	
	public float InsanityProgress
	{
		get { return Insanity / 100; }
	}
	
	public static bool IsTransformLit(Transform transform)
	{
		// Find all lights labelled Light.
		GameObject[] pointlights = GameObject.FindGameObjectsWithTag("Light");
		
		foreach (GameObject pointlight in pointlights)
		{
			// Check if the transform is in the range of a light.
			if (Vector3.Distance (transform.position, pointlight.transform.position) < pointlight.light.range)
			{
				// Check shadows. (May have to check from top of model not centre).
				if (Physics.Linecast (transform.position, pointlight.transform.position, 1) == false)
				{
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
		Debug.Log (IsTransformLit (transform) ? "I'm in the light!!" : "I'm not in the light!!");
		
		if (networkView.isMine)
		{			
			HandleVision ();
			
			HandleSanity ();
			
			Debug.Log (audiolevel.ToString());
			
			if(audiolevel > 100)
			{
				audiolevel = 100;
			}
			if(audiolevel < 0)
			{
				audiolevel = 0;
			}
			
			InsaneNoise.volume = (float) audiolevel / 100;
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
				if (Vector3.Distance (transform.position, otherPlayer.transform.position) < 7 || IsTransformLit (otherPlayer.transform))
				{
					// Check if the other player is within the view of this player's camera view.
					if (GeometryUtility.TestPlanesAABB (planes, otherPlayer.collider.bounds))
					{	
						// Check if the other player is behind a surface (default layer 1 to ignore players)
						if (Physics.Linecast (transform.position, otherPlayer.transform.position, 1) == false)
						{
							IsLookingAtSomeone = true;
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
			if (Insanity == 0)
			{
				//SCREAM	
			}
			
			Insanity += Time.deltaTime * InsanityRate;
			audiolevel += Mathf.CeilToInt(Time.deltaTime);
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
			}
		}
		
		light.color = Color.Lerp (SafeColour, AlarmColour, InsanityProgress);
		
		// Shake the camera more as the player gets more scared.
		CameraObject.transform.localPosition = new Vector3 (Random.Range (-CameraShake, CameraShake) * InsanityProgress, Random.Range (-CameraShake, CameraShake) * InsanityProgress, 0);
	}
}
