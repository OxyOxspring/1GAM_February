using UnityEngine;
using System.Collections;

public class GenericFloatyScript : MonoBehaviour
{
	public float YPosition;
	public float WaveFrequency;
	public float WaveAmplitude;
	
	private float waveTimer = 0;
	
	// Update is called once per frame
	void Update ()
	{		
		transform.position = new Vector3(transform.position.x, YPosition + WaveAmplitude * Mathf.Sin (waveTimer = (waveTimer + WaveFrequency * Time.deltaTime) % (2 * Mathf.PI)), transform.position.z);
	}
}
