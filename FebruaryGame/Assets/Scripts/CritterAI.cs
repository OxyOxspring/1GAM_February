using UnityEngine;
using System.Collections;

/// <summary>
/// Critter A.I.
/// Logic for the critters...
/// Coded by Sean Feb 2013
/// </summary>

public class CritterAI : MonoBehaviour {
	
	//GOBLIN STATE STORAGE:
	int currentState;
	//0 - Dormant
	//1 - Alarmed
	//2 - Running From Player
	//3 - Hiding From Player
	
	
	//Character Controller:
	CharacterController controller = new CharacterController();
	
	//Movement related Variables
	static float moveSpeed;
	
	
	//Variable for wandering:
	Vector3 wayPoint;
	int moveDelay;
	int nextDelayCount;
	
	//Variables for A.I. Navigation:
	float playerRange;

	// Initialisation of variables and objects:
	void Start () 
	{
		Wander();
		moveSpeed = 8f;
		currentState = 0;
		controller = GetComponent<CharacterController>();
		nextDelayCount = 40;
	}
	
	void Wander()
	{
		wayPoint = Random.insideUnitSphere*5;
		wayPoint.x = wayPoint.x + transform.position.x;
		wayPoint.z = wayPoint.z + transform.position.z;
	    wayPoint.y = 0;
		nextDelayCount = Random.Range (30,40);
	}
	
	// Update is called once per frame:
	void Update () 
	{

		
		/////////////////////////////////////
		//                                 //
		//          DORMANT MODE!          //
		//                                 //
		/////////////////////////////////////
		
		//While the Goblin isn't chasing or searching for the player it
		//goes into a dormant state where it checks if the player is close
		//enough. It then 'Hears' the player and is alarmed.
		
		if ((currentState != 1)) //I'm not chasing the player...
		{

				//So I'll stay where I am and chill out until...

				moveDelay++;
				if (moveDelay >= nextDelayCount)
				{
				moveSpeed = 8;
					Wander();
					moveDelay = 0;
				}
				
				Quaternion targetRotation = Quaternion.LookRotation(wayPoint - transform.position);
		        targetRotation = new Quaternion(0,-targetRotation.y,0,targetRotation.w);
		      	transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.1f);
   		     	
  		   	     var delta = wayPoint - transform.position;
      			 delta.Normalize();
				 controller.SimpleMove(delta * moveSpeed);
  		   		

		}
		

	}
}
