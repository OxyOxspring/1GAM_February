using UnityEngine;
using System.Collections;

public class FaceSpirit : MonoBehaviour
{
	private GameObject localSpirit;
	
	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (localSpirit == null)
		{
			GameObject[] spirits = GameObject.FindGameObjectsWithTag("Spirit");
			foreach (GameObject spirit in spirits)
			{
				if (spirit.networkView.isMine)
				{
					localSpirit = spirit;
					break;
				}
			}	
		}
		else
		{
			transform.LookAt (localSpirit.transform.position);
			transform.Rotate (new Vector3(0, 180, 0));
		}
	}
}
