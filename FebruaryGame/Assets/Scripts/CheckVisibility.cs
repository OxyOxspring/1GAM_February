using UnityEngine;
using System.Collections;

public class CheckVisibility : MonoBehaviour
{
	public Camera ChildCamera;
	public float Sanity = 0;
	
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (networkView.isMine)
		{
			Plane[] planes = GeometryUtility.CalculateFrustumPlanes(ChildCamera);
			GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
			
			foreach (GameObject otherPlayer in players)
			{
				if (otherPlayer.transform != transform)
				{
					if (GeometryUtility.TestPlanesAABB (planes, otherPlayer.collider.bounds))
					{	
						Debug.Log ("Gotcha, foo'");	
						if (Sanity < 100)
						{
							Sanity += Time.deltaTime;
						}
						else
						{
							Sanity = 100;
							
							// OHSHITYOUDIED
						}
					}
					else
					{
						Debug.Log ("Where'd ya go?");
						if (Sanity > 0)
						{
							Sanity -= Time.deltaTime;
						}
						else
						{
							Sanity = 0;
						}
						
					}
				}
			}
		}
	}
}
