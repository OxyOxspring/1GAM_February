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
	
	//The current player targetted by the Goblin:
	static GameObject targetPlayer;
	
	//Variable for wandering:
	Vector3 wayPoint;
	int moveDelay;
	int nextDelayCount;
	
	//Variables for A.I. Navigation:
	float playerRange;
	float closeToLastLocation;
	Vector3 lastKnownLocation;
	
	// Initialisation of variables and objects:
	void Start () 
	{
		Wander();
		moveSpeed = 3f;
		currentState = 0;
		targetPlayer = GameObject.Find("Player(Clone)");
		controller = GetComponent<CharacterController>();
		nextDelayCount = 100;
	}
	
	void Wander()
	{
	wayPoint = Random.insideUnitSphere*300;
	wayPoint.x = wayPoint.x + transform.position.x;
	wayPoint.z = wayPoint.z + transform.position.z;
    wayPoint.y = 0;
	nextDelayCount = Random.Range (100,250);
	}
	
	// Update is called once per frame:
	void Update () 
	{
		//Calculate how the far the targetted player is from the Goblin:
		playerRange = Vector3.Distance(transform.position, targetPlayer.transform.position);
		
		/////////////////////////////////////
		//                                 //
		//          DORMANT MODE!          //
		//                                 //
		/////////////////////////////////////
		
		//While the Goblin isn't chasing or searching for the player it
		//goes into a dormant state where it checks if the player is close
		//enough. It then 'Hears' the player and is alarmed.
		
		if ((currentState != 2)) //I'm not chasing the player...
		{
			if((currentState != 3)) //...nor am I looking for him...
			{
				//So I'll stay where I am and chill out until...
				
				closeToLastLocation = Vector3.Distance(transform.position,wayPoint);
				Debug.Log(closeToLastLocation);
				
				moveDelay++;
				if (moveDelay >= nextDelayCount)
				{
					Wander();
					moveDelay = 0;
				}
				
				Quaternion targetRotation = Quaternion.LookRotation(wayPoint - transform.position);
		        targetRotation = new Quaternion(0,targetRotation.y,0,targetRotation.w);
		      	transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.1f);
   		     	
  		   	     var delta = wayPoint - transform.position;
      			 delta.Normalize();
				 controller.SimpleMove(delta * moveSpeed);
  		   		
				targetPlayer = GameObject.Find("Player(Clone)");
				if(playerRange < 12.5f) //I CAN HEAR THE PLAYER!
				{
				//I'M ALARMED BY HIS NOISES!
				currentState = 1;
				}
				if(playerRange > 13.0f) //I CANNOT HEAR THE PLAYER!
				{
				//I'M NOT ALARMED BY ANY NOISES!
				currentState = 0;
				}
			}
		}
		

		/////////////////////////////////////
		//                                 //
		//           ALARMED MODE          //
		//                                 //
		/////////////////////////////////////
		
		if(currentState == 1)
		{
			//IF THE PLAYER IS SEEN BY THE GOBLIN (Using a Linecast...)
			//This could be improved by adding an angle of view, but I've not added that in yet:
			RaycastHit playerSensor;
			if(Physics.Linecast (transform.FindChild("Head").transform.position,targetPlayer.transform.position,out playerSensor))
			{
				if (playerSensor.collider.gameObject.name == "You")
				{
					//Start Chasing
					currentState = 2;
				}
			}
		}
		
		/////////////////////////////////////
		//                                 //
		//          CHASING MODE           //
		//                                 //
		/////////////////////////////////////
		
		if(currentState == 2)
		{
	
			
			if(Vector3.Distance(transform.position,lastKnownLocation) < 3.0f)
			{
				moveSpeed = 5f;
			}
			else
			{
				moveSpeed = 3.5f;
			}
			
			//LOOK AT THE PLAYER!
			Quaternion targetRotation = Quaternion.LookRotation(targetPlayer.transform.position - transform.position);
			targetRotation = new Quaternion(0,-targetRotation.y,0,targetRotation.w);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.1f);
			
			//This could be improved by adding an angle of view, but I've not added that in yet
			RaycastHit playerSensor;
			if(Physics.Linecast (transform.FindChild("Head").transform.position,targetPlayer.transform.position,out playerSensor))
			{
				if (playerSensor.collider.gameObject.name == "You")
				{
					lastKnownLocation = targetPlayer.transform.position;
				}
				else
				{
					currentState = 3;
				}
			}
			

			//Charge the player
			var delta = lastKnownLocation - transform.position;
      		delta.Normalize();
			controller.SimpleMove(-delta * moveSpeed);
			
		}
		
		/////////////////////////////////////
		//                                 //
		//         SEARCHING MODE          //
		//                                 //
		/////////////////////////////////////
		
		if(currentState == 3)
		{
			
			//Move to the last location the Goblin saw the player:
			var delta = lastKnownLocation - transform.position;
      		delta.Normalize();
			controller.SimpleMove(delta * moveSpeed);
			
			//Keep an eye out for the player's whereabouts:
			RaycastHit playerSensor;
			if(Physics.Linecast (transform.FindChild("Head").transform.position,targetPlayer.transform.position,out playerSensor))
			{
				if (playerSensor.collider.gameObject.name == "You")
				{
					//Start chasing him again:
					currentState = 2;
				}
			}
			
			//If the goblin gets to the location where he last saw the player and has
			//had no luck finding him, he goes back to dormant mode. (0.35f is roughly
			//the float value of the distance when the goblin stops moving.)
			closeToLastLocation = Vector3.Distance(transform.position,lastKnownLocation);
			Debug.Log(closeToLastLocation);
			if (closeToLastLocation < 1.35f)
			{
			//Lost him... better go back to chilling out!
			currentState = 0;	
			}	
		}
		
		
	}

	
}
