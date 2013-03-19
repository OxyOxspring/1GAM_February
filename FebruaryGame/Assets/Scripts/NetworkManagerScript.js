var PlayerPrefab:GameObject;
var RatPrefab:GameObject;
var SpiritPrefab:GameObject;
var SpiritPrefabUntagged:GameObject;
var CorpsePrefab:GameObject;
var LeaderBoard:GameObject;
var GameTimer:float;
var GameTimerRunning:boolean;

var plrspawn1:GameObject;
var plrspawn2:GameObject;
var plrspawn3:GameObject;
var plrspawn4:GameObject;
var plrspawn5:GameObject;
var plrspawn6:GameObject;
var plrspawn7:GameObject;
var plrspawn8:GameObject;
var plrspawn9:GameObject;
var plrspawn10:GameObject;

var sprspawn1:GameObject;
var sprspawn2:GameObject;
var sprspawn3:GameObject;
var sprspawn4:GameObject;
var sprspawn5:GameObject;

private var isAlive:boolean = false;
var SpiritRealm:GameObject;

private var players:int = -1;
private var player:int = -1;

private var spawnObject:GameObject;
private var spiritspawnObject:GameObject;
var gameName:String = "HideousNetworking";
private var stringToEdit : String = "";
private var displayString : String = "";
private var connectedTime:float = 0;

private var refreshing:boolean = false;
private var hostData:HostData[];

private var btnX:float;
private var btnY:float;
private var btnW:float;
private var btnH:float;

private var fogDensity:float;

MasterServer.ipAddress = "192.168.0.27";
MasterServer.port = 23466;

//MasterServer.ipAddress = "67.255.180.25";
//MasterServer.port = 50005;

function Start(){

	btnX = Screen.width * 0.05;
	btnY = Screen.width * 0.05;
	btnW = Screen.width * 0.1;
	btnH = Screen.width * 0.1;
	
	Screen.SetResolution(1280, 720, false, 60);
	
	fogDensity = RenderSettings.fogDensity;
	RenderSettings.fogDensity = 0;

}

function startServer(){

Network.InitializeServer(9,25001,false);
MasterServer.RegisterHost(gameName,"Hideous", "Become Hideous...");
}

function refreshHostList(){
	MasterServer.RequestHostList(gameName);
	refreshing = true;
}

function Update()
{
	if(refreshing == true)
	{
		if(MasterServer.PollHostList().Length > 0)
		{
			refreshing = false;
			Debug.Log(MasterServer.PollHostList().Length);
			hostData = MasterServer.PollHostList(); 
		}

	}
	
	if (Network.isServer)
	{
		if (GameTimerRunning == true)
		{
			GameTimer += Time.deltaTime;
		}
	}
	
	var players:GameObject[] = GameObject.FindGameObjectsWithTag("Player");
	if (players.Length == 1)
	{
		// isAlive will only be true for the client who is alive.
		if (isAlive)
		{
			players[0].SendMessage("Win");
			GametimerRunning = false;
			GameTimer = 0;
		}
	}
	
	killPlayers();
}

function spawnPlayer(){
networkView.RPC("updateString",RPCMode.AllBuffered,stringToEdit);
chooseSpiritSpawn();
	Network.Instantiate(SpiritPrefab, spiritspawnObject.transform.position, Quaternion.identity,0);
	networkView.RPC("leaderboardRecordPlayer", RPCMode.All, stringToEdit);
	networkView.RPC("IncrementPlayerCount", RPCMode.All);
	removeShit();
}

function choosePlayerSpawn()
{
	switch (player)
	{
		case 0:
			spawnObject = plrspawn1;
			break;
		case 1:
			spawnObject = plrspawn2;
			break;
		case 2:
			spawnObject = plrspawn3;
			break;
		case 3:
			spawnObject = plrspawn4;
			break;
		case 4:
			spawnObject = plrspawn5;
			break;
		case 5:
			spawnObject = plrspawn6;
			break;
		case 6:
			spawnObject = plrspawn7;
			break;
		case 7:
			spawnObject = plrspawn8;
			break;
		case 8:
			spawnObject = plrspawn9;
			break;
		case 9:
			spawnObject = plrspawn10;
			break;		
	}
}

function chooseSpiritSpawn()
{
	var randomnumber = Random.Range(1,5);
	switch (randomnumber)
	{
		case 1:
		spiritspawnObject = sprspawn1;
		break;
		case 2:
		spiritspawnObject = sprspawn2;
		break;
		case 3:
		spiritspawnObject = sprspawn3;
		break;
		case 4:
		spiritspawnObject = sprspawn4;
		break;
		case 5:
		spiritspawnObject = sprspawn5;
		break;
	}

}

function killPlayers(){
	var players:GameObject[] = GameObject.FindGameObjectsWithTag("Player");
	
	for (var i:int = 0; i < players.Length; i++)
	{
		if (players[i].transform.GetComponent("CheckVisibility").Dead)
		{
			swapPlayerForSpirit(players[i]);
		}
	}

}

function swapPlayerForSpirit (player:GameObject){

	if (player.networkView.isMine)
	{
		RenderSettings.fogDensity = 0;
		Debug.Log("SKND");
		isAlive = false;
		
		var position:Vector3 = player.transform.position;
	
		networkView.RPC("leaderboardRecordEntry", RPCMode.All, stringToEdit);
		//networkView.RPC("removeShit", RPCMode.All);
		
		chooseSpiritSpawn();
		Network.Instantiate(SpiritPrefab, spiritspawnObject.transform.position, Quaternion.identity, 0);
		Network.Destroy(player);
		Network.Instantiate(CorpsePrefab, position, Quaternion.identity, 0);
		
		for (var child:Transform in SpiritRealm.GetComponentsInChildren(Transform))
		{			
			if (child.name == "SmokeRing")
			{
				child.particleSystem.Play();
			}
			else if (child.name == "Light")
			{
				child.light.enabled = true;
			}
			else
			{
				var fudge:MeshRenderer = child.GetComponentInChildren(Transform).GetComponentInChildren(MeshRenderer);
				
				if (fudge != null)
				{
					fudge.enabled = true;
				}
			}
		}
		
		// Tether an untagged spirit to every existing player.
		for (var alive:GameObject in GameObject.FindGameObjectsWithTag("Player"))
		{
			if (alive != player && alive.GetComponent("PlayerScript").HasBeenTethered == false)	// Only the player owning the network connection will have a camera enabled.
			{
				var instance:GameObject = Instantiate(SpiritPrefabUntagged, alive.transform.position, Quaternion.identity);
				instance.GetComponent("SpiritScript").SendMessage("Tether", alive);
				alive.GetComponent("PlayerScript").HasBeenTethered = true;
			}
		}
		
	}
	
	removeShit();
}

function swapSpiritForPlayer(spirit:GameObject)
{
	if (spirit.networkView.isMine)
	{	
		RenderSettings.fogDensity = fogDensity;	
		Debug.Log(fogDensity.ToString() + "Hello: ");
		isAlive = true;
		choosePlayerSpawn();
		PlayerPrefab.transform.FindChild("Graphics").renderer.enabled = false;
		Network.Instantiate(PlayerPrefab, spawnObject.transform.position, Quaternion.identity, 0);
		PlayerPrefab.transform.FindChild("Graphics").renderer.enabled = true;
		Network.Destroy(spirit);
		
		
		for (var child:Transform in SpiritRealm.GetComponentsInChildren(Transform))
		{
			if (child.name == "SmokeRing")
			{
				child.particleSystem.Stop();
				child.particleSystem.Clear();
			}
			else if (child.name == "Light")
			{
				child.light.enabled = false;
			}
			else
			{
				var fudge:MeshRenderer = child.GetComponentInChildren(Transform).GetComponentInChildren(MeshRenderer);
				
				if (fudge != null)
				{
					fudge.enabled = false;
				}
			}
		}
		
		for (var corpse:GameObject in GameObject.FindGameObjectsWithTag("Corpse"))
		{
				corpse.transform.FindChild("CorpseBeam").GetComponentInChildren(MeshRenderer).enabled = true;
		}
	}
}

function beginTimer()
{
	 if (Network.isServer)
	{
		GameTimer = 0;
		GameTimerRunning = true;
		LeaderBoard.GetComponent("Leaderboard").SendMessage("ClearEntries");
	}
}

//Messages
function OnServerInitialized(){
	Debug.Log("Server Initialised!");
	//script.enabled = true;
	spawnPlayer();
}

function OnConnectedToServer(){
	spawnPlayer();	
	connectedTime = Time.time;
	networkView.RPC("CheckPlayer", RPCMode.All, stringToEdit, connectedTime);
	//script.enabled = true;
}

function OnDisconnectedFromServer(){
	Application.LoadLevel(0);
	//script.enabled = false;
}

function OnPlayerDisconnected(player: NetworkPlayer){
	Debug.Log("Clean up after player " +  player);
    Network.RemoveRPCs(player);
    Network.DestroyPlayerObjects(player);
}

function OnMasterServerEvent(mse:MasterServerEvent){
	if(mse == MasterServerEvent.RegistrationSucceeded){
	Debug.Log("Registered Server");
	}
}

//GUI
function OnGUI(){
	if(!Network.isClient && !Network.isServer){
	stringToEdit = GUI.TextField(Rect(btnX,btnY,btnW,btnH/5),stringToEdit,12);
		
		if(GUI.Button(Rect(btnX,btnY * 1.5,btnW,btnH),"Start Server"))
		{
			Debug.Log("Starting Server");
			startServer();
		}
		if(GUI.Button(Rect(btnX,btnY * 1.6 + btnH,btnW,btnH),"Refresh Hosts"))
		{
			Debug.Log("Refreshing");
			refreshHostList();
		}
		
		if(hostData){
			for(var i:int = 0; i < hostData.length; i++){
				if(GUI.Button(Rect(btnX * 2 + btnW, btnY * 1.2 + (btnH*i),btnW*3,btnH*0.5),hostData[i].comment))
				{
					Network.Connect(hostData[i]);
				}
			}
		}
	}
	else
	{
		GUI.Label(Rect(btnX-30,btnY-40,btnW*4,btnH/5),displayString);
	}
}



@RPC
function updateString(str:String){
displayString = str + " has joined the game!";
}

@RPC
function leaderboardRecordEntry(name:String)
{
	if (Network.isServer)
	{
		// Send the name passed through.
		LeaderBoard.GetComponent("Leaderboard").SendMessage("RecordName", name);
		
		// Send the timer the server runs.
		LeaderBoard.GetComponent("Leaderboard").SendMessage("RecordTime", GameTimer);
		
		// Increment the index.
		LeaderBoard.GetComponent("Leaderboard").SendMessage("NextIndex");
	}
}

@RPC
function leaderboardRecordPlayer(player:String)
{
	if (Network.isServer)
	{
		LeaderBoard.GetComponent("Leaderboard").SendMessage("RecordPlayer", player);
	}
}

@RPC
function forceAllSpawn()
{
	if (Network.isServer)
	{
		beginTimer();
	}
	
	for (var spirit:GameObject in GameObject.FindGameObjectsWithTag("Spirit"))
	{
		swapSpiritForPlayer(spirit);
	}
	
	//unspawnallspawns();
}

@RPC
function syncLeaderboardEntry(name:String, time:float, index:int, player:String)
{	
	Debug.Log("Syncing...");

	if (Network.isClient)
	{		
		// Set the index.
		LeaderBoard.SendMessage("SetIndex", index);
	
		// Record a name.
		LeaderBoard.SendMessage("RecordName", name);
		
		// Record a time (and push to the next index).
		LeaderBoard.SendMessage("RecordTime", time);
		
		// This is really stupid because its only shown at the start of the game, but I guess it could be used somewhere later.
		LeaderBoard.SendMessage("RecordPlayer", player);
	}
}	

@RPC
function IncrementPlayerCount()
{
	if (Network.isServer)
	{
		players++;
		
		if (player == -1)
		{
			player = players;
		}
		
		networkView.RPC("ClientPlayerCount", RPCMode.All, players);
	}
}

@RPC
function ClientPlayerCount(amount:int)
{
	if (Network.isClient)
	{
		players = amount;
		
		if (player == -1)
		{
			player = players;
		}
	}
}




function removeShit()
{
	if (isAlive == false)
	{
		for (var corpse:GameObject in GameObject.FindGameObjectsWithTag("Corpse"))
		{
			corpse.transform.FindChild("CorpseBeam").GetComponentInChildren(MeshRenderer).enabled = false;
		}
		
		// Unspawn spirits.
		for (var tethered:GameObject in GameObject.FindGameObjectsWithTag("SpiritAlive"))
		{
			var pep_pep:GameObject = tethered.GetComponent("SpiritScript").TetheredObject;
			Debug.Log(pep_pep.ToString());
		
			var found:boolean = false;
			for (var player:GameObject in GameObject.FindGameObjectsWithTag("Player"))
			{
				if (pep_pep == player)
				{
					if (player.active == false)
					{
						found = true;
					}
					Debug.Log(player);
					break;
				}
			}	
		
			if (found == false)
			{
				Destroy(tethered);
			}
		}
	}
}

@RPC
function CheckPlayer(name:String, time:float)
{
	if (stringToEdit == name)
	{
		if (connectedTime < time)
		{
			// If this time is older than the time sent through the function, echo the function back.
			networkView.RPC("CheckPlayer", RPCMode.All, stringToEdit, connectedTime);
		}
		else if (connectedTime > time)
		{
			// If this time is younger than the time sent through the function, modify the name.
			networkView.RPC("Rename", RPCMode.All, stringToEdit, stringToEdit + "0");
			stringToEdit += "0";
			networkView.RPC("CheckPlayer", RPCMode.All, stringToEdit, connectedTime);
		}
	}
}

@RPC
function Rename(name:String, newname:String)
{
	if (Network.isServer)
	{
		LeaderBoard.GetComponent("Leaderboard").SendMessage("RecordPlayer", newname);
		//LeaderBoard.GetComponent("Leaderboard").SendMessage("Rename", name, newname);
	}
}