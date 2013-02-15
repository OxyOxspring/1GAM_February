using UnityEngine;
using System.Collections;

public class Leaderboard : MonoBehaviour
{
	private string[] names = new string[10];
	private float[] times = new float[10];
	private int entrycounter = 0;
	
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{		
		GameObject[] spirits = GameObject.FindGameObjectsWithTag ("Spirit");
		
		int i;
		foreach (Transform entry in transform)
		{
			i = System.Convert.ToInt32 (entry.name[entry.name.Length - 1].ToString ());
			if (i < spirits.Length)
			{
				entry.gameObject.SetActive(true);
				TextMesh name = (TextMesh)entry.GetChild (0).GetComponent ("TextMesh");
				TextMesh time = (TextMesh)entry.GetChild (1).GetComponent ("TextMesh");
				name.text = names[i];
				time.text = times[i].ToString ();
			}
			else
			{
				entry.gameObject.SetActive(false);
			}
		}
	}
	
	public void RecordName(string name)
	{		
		names[entrycounter] = name;
	}
	
	public void RecordTime(float time)
	{		
		times[entrycounter] = time;
		
		if (entrycounter < 10)
		{
			entrycounter++;
		}
		else
		{
			entrycounter = 0;	
		}
	}
	
	public string GetName(int i)
	{
		return names[i];
	}
	
	public float GetTime(int i)
	{
		return times[i];
	}
	
	public void ClearEntries()
	{
		for (int i = 0; i < 10; i++)	
		{
			names[i] = "";
			times[i] = 0;
		}
		entrycounter = 0;
	}
	
	
	
}
