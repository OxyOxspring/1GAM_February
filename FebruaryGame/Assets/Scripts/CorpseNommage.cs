using UnityEngine;
using System.Collections;

public class CorpseNommage : MonoBehaviour {
	
	public int Nommage = 3;
	
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{

	}
	
	[RPC]
	public void Nom(int amount)
	{
		Nommage -= amount;	
		
		if (Nommage <= 0)
		{
			audio.Play ();
			Network.Destroy (gameObject);	
		}
		else if (Nommage == 2)
		{
			transform.FindChild ("Head").renderer.enabled = false;	
		}
		else if (Nommage == 1)
		{
			transform.FindChild ("Body").renderer.enabled = false;	
		}
	}
}
