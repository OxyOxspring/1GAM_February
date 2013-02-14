using UnityEngine;
using System.Collections;

public class SummoningJoinText : MonoBehaviour
{
	public GameObject networkManager;
	private int potato = 0;	// Number of players inside the circle.
	private float carrot = 0;	// Time until the game starts.
	
	void Update ()
	{
		GameObject[] spirits = GameObject.FindGameObjectsWithTag("Spirit");
		TextMesh textMesh = (TextMesh)GetComponent ("TextMesh");
		
		potato = 0;
		
		foreach (GameObject spirit in spirits)
		{
			if (carrot >= 5)
			{
				networkManager.SendMessage ("swapSpiritForPlayer", spirit);
				break;	
			}
			
			if (spirit.networkView.isMine)
			{
				transform.LookAt (new Vector3(spirit.transform.position.x, spirit.transform.position.y, spirit.transform.position.z));
				transform.Rotate (new Vector3(0, 180, 0));
			}
			
			if (Vector3.Distance (transform.position, spirit.transform.position) < 9f)
			{
				potato++;	
			}
		}
		
		if (spirits.Length > 0 && potato == spirits.Length)
		{
			carrot += Time.deltaTime;
		}
		else
		{
			if (carrot > 0)	
			{
				carrot -= Time.deltaTime;
			}
			else
			{
				carrot = 0;	
			}
		}
		
		textMesh.text = potato.ToString () + "/" + spirits.Length.ToString () ;
	}
}
