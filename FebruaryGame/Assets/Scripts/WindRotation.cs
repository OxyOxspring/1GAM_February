using UnityEngine;
using System.Collections;

public class WindRotation : MonoBehaviour
{
	private float rotation = 0.0f;
	
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		rotation += Time.deltaTime / 200;
		
		if (rotation > 2 * Mathf.PI)
		{
			rotation = 0.0f;
		}
		
		transform.Rotate(new Vector3(0, rotation,0));
	}
}
