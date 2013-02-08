using UnityEngine;
using System.Collections;

public class Lightning : MonoBehaviour
{
	public float FlashIntensity = 8.0f;
	public float flashDelay = 0.0f;
	private float flashTimer = 0.0f;
	private float thunderDelay = 0.0f;
	private float thunderTimer = 0.0f;
	
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (light.intensity > 0.1f)
		{
			light.intensity *= 0.8f;
		}
		else
		{
			light.intensity = 0.0f;
		}

		// Basic timer for flash.
		if (flashTimer >= flashDelay)
		{
			light.intensity = FlashIntensity;
			
			// Make a random delay until the next lightning strike to ensure a lightning strike WILL happen.
			flashDelay = Random.Range(20, 60);

			// Give the thunder a random delay.
			thunderDelay = Random.Range (1, 4);	

			flashTimer = 0;
		}
		else
		{
			flashTimer += Time.deltaTime;
		}
		
		if (thunderDelay != 0)
		{
			// Basic timer for thunder.
			if (thunderTimer >= thunderDelay)
			{
				audio.volume = 1 - thunderDelay / 5;
				audio.Play ();
				
				// Tell the timer to wait.
				thunderDelay = 0;
			}
			else
			{
				thunderTimer += Time.deltaTime;
			}
		}
	}
}
