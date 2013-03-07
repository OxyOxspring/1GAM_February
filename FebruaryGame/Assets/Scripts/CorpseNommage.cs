using UnityEngine;
using System.Collections;

public class CorpseNommage : MonoBehaviour {
	
	private float Nommage = 5;
	
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
		Nommage -= Time.deltaTime * amount;			
		
		transform.localScale = new Vector3(1, 1, 1) * (Nommage / 5);
		Debug.Log (Nommage);
		if (Nommage <= 0)	// Gone.
		{
			audio.Play ();
			Network.Destroy (gameObject);	
		}
		else if (Nommage < 33)	// Decomposed.
		{

		}
		else if (Nommage < 66)	// Partially decomposed.
		{
	
		}
		else // Fully composed.
		{
			
		}
	}
}
