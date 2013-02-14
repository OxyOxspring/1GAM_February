var PlayerPrefab:GameObject;
var RatPrefab:GameObject;
var SpiritPrefab:GameObject;
var plrspawn1:GameObject;
var plrspawn2:GameObject;
var plrspawn3:GameObject;
var plrspawn4:GameObject;
var plrspawn5:GameObject;
var sprspawn1:GameObject;
var sprspawn2:GameObject;
var sprspawn3:GameObject;
var sprspawn4:GameObject;
var sprspawn5:GameObject;
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
Network.InitializeServer(32,25001,!Network.MovePublicAddress);
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
	
	killPlayers();
}

function spawnPlayer(){
networkView.RPC("updateString",RPCMode.AllBuffered,stringToEdit);
chooseSpawn();
	Network.Instantiate(SpiritPrefab, spiritspawnObject.transform.position, Quaternion.identity,0);
}

function chooseSpawn(){
	var randomnumber = Random.Range(1,5);
	switch (randomnumber) {
	case 1:
	spawnObject = plrspawn1;
	spiritspawnObject = sprspawn1;
	break;
	case 2:
	spawnObject = plrspawn2;
	spiritspawnObject = sprspawn2;
	break;
	case 3:
	spawnObject = plrspawn3;
	spiritspawnObject = sprspawn3;
	break;
	case 4:
	spawnObject = plrspawn4;
	spiritspawnObject = sprspawn4;
	break;
	case 5:
	spawnObject = plrspawn5;
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
		chooseSpawn();
		Network.Instantiate(SpiritPrefab, spiritspawnObject.transform.position, Quaternion.identity, 0);
		Network.Destroy(player);
	}
}

function swapSpiritForPlayer(spirit:GameObject)
{
	if (spirit.networkView.isMine)
	{
		chooseSpawn();
		Network.Instantiate(PlayerPrefab, spawnObject.transform.position, Quaternion.identity, 0);
		Network.Destroy(spirit);
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


