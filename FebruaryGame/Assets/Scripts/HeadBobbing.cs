using UnityEngine;
using System.Collections;

public class HeadBobbing : MonoBehaviour 
{
	public float BobAmplitude = 1f;
	public float BobFrequency = 1.0f;
	private float bobTimer = 0.0f;
	private float crouchFactor = 1;
	private bool footBool = false;
	
	public GameObject Player;
	public AudioSource leftFoot;
	public AudioSource rightFoot;
	
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (networkView.isMine)
		{
			float amount = BobFrequency * Time.deltaTime / crouchFactor;
			
			if (Input.GetAxis ("Horizontal") != 0 || Input.GetAxis ("Vertical") != 0)
			{
				bobTimer += amount;
				
				if (bobTimer >= Mathf.PI)
				{
					bobTimer = 0;
					
					footBool = !footBool;
					
					if (footBool == true)	
					{
						leftFoot.Play ();
					}
					else
					{
						rightFoot.Play ();
					}
				}
			}
			else if(bobTimer > 0 && bobTimer < Mathf.PI)
			{
				if(bobTimer < Mathf.PI / 2)
				{
					bobTimer -= amount;
				}
				else if(bobTimer >= Mathf.PI / 2)
				{
					bobTimer += amount;
				}
			}
			else
			{
				bobTimer = 0;	
			}
			
			transform.position = new Vector3(transform.position.x, Player.transform.position.y + Player.transform.localScale.y / 2 + BobAmplitude / crouchFactor * Mathf.Sin (bobTimer), transform.position.z);
		}
	}
			
	void Crouch()
	{
		if (networkView.isMine)
		{
			if (crouchFactor == 2)	
			{
				crouchFactor = 1;
			}
			else
			{
				crouchFactor = 2;
			}
		}
	}
}
