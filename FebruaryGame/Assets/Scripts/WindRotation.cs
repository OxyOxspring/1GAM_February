using UnityEngine;
using System.Collections;

public class WindRotation : MonoBehaviour
{
	public int Speed;
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.Rotate(new Vector3(0, Time.deltaTime * Speed, 0));
	}
}
