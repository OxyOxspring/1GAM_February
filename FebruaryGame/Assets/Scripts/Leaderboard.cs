using UnityEngine;
using System.Collections;

public class Leaderboard : MonoBehaviour
{
	public GameObject networkManager;
	
	private string[] playersconnected = new string[10];
	private string[] names = new string[10];
	private float[] times = new float[10];
	private int entrycounter = 0;
	
	// Use this for initialization
	void Start ()
	{
		ClearEntries ();	
	}
	
	// Update is called once per frame
	void Update ()
	{		
		GameObject[] spirits = GameObject.FindGameObjectsWithTag ("Spirit");
		
		int i;
		foreach (Transform entry in transform)
		{			
			TextMesh name = (TextMesh)entry.GetChild (0).GetComponent ("TextMesh");
			TextMesh time = (TextMesh)entry.GetChild (1).GetComponent ("TextMesh");
			
			i = System.Convert.ToInt32 (entry.name[entry.name.Length - 1].ToString ());
			
			if (names[0] == "")	// If there are no entries.
			{
				if (playersconnected[i] != "")
				{
					name.text = playersconnected[i];
					time.text = " has joined...";
				}
				else
				{
					name.text = "";
					time.text = "";
				}
			}
			else
			{
				if (i < spirits.Length)
				{
					//entry.renderer.enabled = true;
					name.text = names[i];
					
					int minutes = (int)times[i] / 60;
					int seconds = (int)times[i] % 60;
					
					time.text = "survived for " + minutes.ToString () + ":" + seconds.ToString ();
				}
				else
				{
					//entry.renderer.enabled = false;
					name.text = "";
					time.text = "";
				}
			}
		}
		
		
		SyncEntries();
	}
	
	public void SortEntries()
	{
		string tempName;
		float tempTime;
		
		// Sort descending.
		for (int i = 0; i < 10; i++)
		{
			for (int j = i + 1; j < 10; j++)
			{
				if (times[i] < times[j])	
				{
					tempTime = times[i];
					times[i] = times[j];
					times[j] = tempTime;
					
					tempName = names[i];
					names[i] = names[j];
					names[j] = tempName;
				}
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
		
		SortEntries ();
	}
	
	public void RecordPlayer(string player)
	{
		for (int i = 0; i < playersconnected.Length; i++)
		{
			if (playersconnected[i] == player)
			{
				break;	
			}
			
			if (playersconnected[i] == "")
			{
				playersconnected[i] = player;
				break;	
			}
		}
	}
	
	public void ClearPlayers()
	{
		for (int i =0 ;i < 10; i++)
		{
			playersconnected[i] = "";
		}
	}
	
	public void NextIndex()
	{
		if (entrycounter < 10)
		{
			entrycounter++;
		}
		else
		{
			entrycounter = 0;	
		}
	}
	
	public void SetIndex(int i)
	{
		entrycounter = i;	
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
			playersconnected[i] = "";
		}
		entrycounter = 0;
	}
	
	public void SyncEntries()
	{
		if (Network.isServer)
		{
			for (int i = 0; i < 10; i++)
			{
				networkManager.networkView.RPC ("syncLeaderboardEntry", RPCMode.All, names[i], times[i], i, playersconnected[i]);
			}				
		}
	}
	
	public void Rename(string name)
	{
		for (int i = 0; i < 10; i++)	
		{
			
 			if (playersconnected[i] == name)	
			{
				playersconnected[i] = name + "0";
				break;
			}
		}
	}
	
}
