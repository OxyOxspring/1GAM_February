var PlayerPrefab:GameObject;
var RatPrefab:GameObject;
var SpiritPrefab:GameObject;
var SpiritPrefabUntagged:GameObject;
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

var SpiritRealm:GameObject;

private var spawnObject:GameObject;
private var spiritspawnObject:GameObject;
var gameName:String = "OxyOxspringFebNetworking";
private var stringToEdit : String = "Input Username";
private var displayString : String = "";

private var refreshing:boolean = false;
private var hostData:HostData[];

private var btnX:float;
private var btnY:float;
private var btnW:float;
private var btnH:float;


function Start(){
	btnX = Screen.width * 0.05;
	btnY = Screen.width * 0.05;
	btnW = Screen.width * 0.1;
	btnH = Screen.width * 0.1;
}

function startServer(){
Network.InitializeServer(10,25001,!Network.MovePublicAddress);
MasterServer.RegisterHost(gameName,"Hideous", "Become Hideous...");
}

function refreshHostList(){
	MasterServer.RequestHostList(gameName);
	refreshing = true;
}

function Update(){
	if(refreshing == true){
		if(MasterServer.PollHostList().Length > 0){
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
	
	killPlayers();
}

function spawnPlayer(){
networkView.RPC("updateString",RPCMode.AllBuffered,stringToEdit);
chooseSpiritSpawn();
	Network.Instantiate(SpiritPrefab, spiritspawnObject.transform.position, Quaternion.identity,0);
}

function choosePlayerSpawn(){
	var foundSpawn = false;
	for (var spawn:GameObject in GameObject.FindGameObjectsWithTag("PlayerSpawn"))
	{
		for (var player:GameObject in GameObject.FindGameObjectsWithTag("Player"))
		{
			if(Vector3.Distance(spawn.transform.position,player.transform.position) > 4.0f)
			{
			spawnObject = spawn;
			foundSpawn = true;
			}
		}
	}
	if(foundSpawn == false)
	{
	spawnObject = plrspawn1;
	}
}

function chooseSpiritSpawn(){
	var randomnumber = Random.Range(1,5);
	switch (randomnumber) {
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
		networkView.RPC("leaderboardRecordEntry", RPCMode.All, stringToEdit);
		
		chooseSpiritSpawn();
		Network.Instantiate(SpiritPrefab, spiritspawnObject.transform.position, Quaternion.identity, 0);
		Network.Destroy(player);
		
		for (var child:Transform in SpiritRealm.GetComponentsInChildren(Transform))
		{			
			if (child.name == "SmokeRing")
			{
				child.particleSystem.Play();
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
			//SpiritPrefabUntagged.BroadcastMessage("Tether", alive); TETHER
			var instance:GameObject = Instantiate(SpiritPrefabUntagged, alive.transform.position, Quaternion.identity);
		}
		
	}
}

function swapSpiritForPlayer(spirit:GameObject)
{
		if (spirit.networkView.isMine)
		{	
		choosePlayerSpawn();
		Network.Instantiate(PlayerPrefab, spawnObject.transform.position, Quaternion.identity, 0);
		Network.Destroy(spirit);
		
		for (var child:Transform in SpiritRealm.GetComponentsInChildren(Transform))
		{
			if (child.name == "SmokeRing")
			{
				child.particleSystem.Stop();
				child.particleSystem.Clear();
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
	stringToEdit = GUI.TextField(Rect(btnX,btnY,btnW,btnH/5),stringToEdit,16);
		
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
				if(GUI.Button(Rect(btnX * 2 + btnW, btnY * 1.2 + (btnH*i),btnW*3,btnH*0.5),hostData[i].comment)){
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
}

@RPC
function syncLeaderboardEntry(name:String, time:float, index:int)
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
	}
}




