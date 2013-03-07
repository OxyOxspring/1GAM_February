using UnityEngine;
using System.Collections;

public class WibbleScript : MonoBehaviour
{
	float seed;
	float scale;
	
	// Use this for initialization
	void Start ()
	{
		
		seed = (transform.position.x * transform.position.y * transform.position.z * Random.Range(1,10000000000) * Time.deltaTime * transform.rotation.w) % Mathf.PI;
		scale = transform.localScale.x;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (seed >= 2 * Mathf.PI)
		{
			seed = 0;
		}
		else
		{
			seed += Time.deltaTime;
		}
		
		float seedything = Mathf.Sin(seed);
		
		transform.localScale = new Vector3(scale, scale + (seedything * 2), scale + (seedything * 1));
		transform.Rotate (new Vector3(0,1,0), seedything / 20);
	}
}
