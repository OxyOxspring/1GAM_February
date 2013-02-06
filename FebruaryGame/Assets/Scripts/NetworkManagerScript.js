var playerPrefab:GameObject;
var RatPrefab:GameObject;
var spawn1:GameObject;
var spawn2:GameObject;
var spawn3:GameObject;
var spawn4:GameObject;
var spawn5:GameObject;
private var spawnObject:GameObject;
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
	playerPrefab.transform.Find("Camera").camera.enabled = true;
}

function startServer(){
Network.InitializeServer(32,25001,!Network.MovePublicAddress);
MasterServer.RegisterHost(gameName,"Hideous", "Join Hideous Game");
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
}

function spawnPlayer(){
networkView.RPC("updateString",RPCMode.AllBuffered,stringToEdit);
chooseSpawn();
	Network.Instantiate(playerPrefab, spawnObject.transform.position, Quaternion.identity,0);
	Network.Instantiate(RatPrefab, spawnObject.transform.position, Quaternion.identity,0);
	playerPrefab.transform.Find("Camera").camera.enabled = false;
	networkView.RPC("lightsOff",RPCMode.AllBuffered,stringToEdit);
	playerPrefab.light.enabled = true;
}

function chooseSpawn(){
var randomnumber = Random.Range(1,5);
switch (randomnumber) {
case 1:
spawnObject = spawn1;
break;
case 2:
spawnObject = spawn2;
break;
case 3:
spawnObject = spawn3;
break;
case 4:
spawnObject = spawn4;
break;
case 5:
spawnObject = spawn5;
break;
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
function lightsOff(str:String){
playerPrefab.light.enabled = false;
}

