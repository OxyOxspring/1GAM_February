using UnityEngine;
using System.Collections;

public class CheckVisibility : MonoBehaviour
{
	public Camera CameraObject;
	public float CameraShake = 0.2f;
	public float Insanity = 0;
	public float InsanityRate = 80;
	public bool IsLookingAtSomeone = false;
	public Color SafeColour;
	public Color AlarmColour;
	
	float InsanityProgress
	{
		get { return Insanity / 100; }
	}
	
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (networkView.isMine)
		{			
			HandleVision ();
			
			HandleSanity ();
			
			Debug.Log (Insanity.ToString());
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
				// Check if the other player is in range.
				if (Vector3.Distance (transform.position, otherPlayer.transform.position) < 6)
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
			Insanity += Time.deltaTime * InsanityRate;
		}
		else
		{
			if (Insanity > 0)
			{
				Insanity -= Time.deltaTime * InsanityRate;
			}
		}
		
		light.color = Color.Lerp (SafeColour, AlarmColour, InsanityProgress);
		
		// Shake the camera more as the player gets more scared.
		CameraObject.transform.localPosition = new Vector3 (Random.Range (-CameraShake, CameraShake) * InsanityProgress, Random.Range (-CameraShake, CameraShake) * InsanityProgress, 0);
	}
}
