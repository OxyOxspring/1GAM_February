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
		if (Nommage < 0)
		{
			Network.Destroy (gameObject);	
		}
	}
	
	public void ReduceNommage()
	{
		Nommage--;	
	}
	

}
