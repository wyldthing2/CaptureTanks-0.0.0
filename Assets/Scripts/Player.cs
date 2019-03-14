using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;

[System.Serializable]
public class ToggleEvent : UnityEvent<bool> { }

public class Player : NetworkBehaviour
{
    [SyncVar(hook = "OnNameChanged")] public string playerName;
    [SyncVar(hook = "OnColorChanged")] public Color playerColor;

    [SerializeField] public bool isAI;

    [SerializeField] ToggleEvent onToggleShared;
    [SerializeField] ToggleEvent onToggleLocal;
    [SerializeField] ToggleEvent onToggleRemote;
    [SerializeField] ToggleEvent onToggleTopDown;
    [SerializeField] ToggleEvent onToggleFirstPerson;
    [SerializeField] bool TopDown = false;
    [SerializeField] float respawnTime = 0f;
    [SerializeField] public int TeamID;


    [SerializeField] [SyncVar(hook = "OnElapsedTimeNoTagBackChanged")] float elapsedTimeNoTagBack;
    float countDown;
    [SerializeField] float NoTagBackTime = 5f;

    
    public static List<GameObject> players = new List<GameObject>();
    public static int RedTeamCount = 0;
    public static int BlueTeamCount = 0;
    public static int YellowTeamCount = 0;
    public static int GreenTeamCount = 0;
    public static List<int> TeamCountsList = new List<int>();

    public static int playerNumber = 0;

    bool Grounded = false;
    private GameObject errorLog;
    private Text errorText;
    [SerializeField] Text playerNameText;
    [SerializeField] List<MeshRenderer> ListOFObjectsToColor = new List<MeshRenderer>();
    [SerializeField] List<Material> MaterialsList = new List<Material>();

    [SerializeField] GameObject HomeSpawn;
    [SerializeField] public Button RedTeamButton;
    [SerializeField] public Button BlueTeamButton;
    [SerializeField] public Button YellowTeamButton;
    [SerializeField] public Button GreenTeamButton;
    [SerializeField] public Button StartMatchButton;
    [SerializeField] public TeamID ThisTeamID;

    public List<GameObject> ListOfSpawnPoints = new List<GameObject>();






    GameObject mainCamera;

    private void OnTriggerStay(Collider collis)
    {

        if (collis.gameObject.tag == "Blob" && elapsedTimeNoTagBack >= NoTagBackTime)
        {
            //Not optimized, get rid of get component
            
            GetComponent<PlayerHealth>().TakeDamage();
            elapsedTimeNoTagBack = 0;
            
            
        }
    }

    [SerializeField] GameObject MatchMenu;

    void Awake()
    {
        //mainCamera = Camera.main.gameObject;
        errorLog = GameObject.Find("LogText");
        MatchMenu = GameObject.Find("MatchMenu");
        RedTeamButton = GameObject.Find("RedTeam").GetComponent<Button>();
        RedTeamButton.onClick.AddListener(() => RedTeamClick());
        BlueTeamButton = GameObject.Find("BlueTeam").GetComponent<Button>();
        BlueTeamButton.onClick.AddListener(() => BlueTeamClick());
        YellowTeamButton = GameObject.Find("YellowTeam").GetComponent<Button>();
        YellowTeamButton.onClick.AddListener(() => YellowTeamClick());
        GreenTeamButton = GameObject.Find("GreenTeam").GetComponent<Button>();
        GreenTeamButton.onClick.AddListener(() => GreenTeamClick());
        StartMatchButton = GameObject.Find("StartMatch").GetComponent<Button>();
        StartMatchButton.onClick.AddListener(() => StartMatch());
        //errorText = errorLog.GetComponent<Text>();

        RedSpawn = GameObject.Find("RedBase").transform.Find("FlagPad").gameObject;
        ListOfSpawnPoints.Add(RedSpawn);
        BlueSpawn = GameObject.Find("BlueBase").transform.Find("FlagPad").gameObject;
        ListOfSpawnPoints.Add(BlueSpawn);
        YellowSpawn = GameObject.Find("YellowBase").transform.Find("FlagPad").gameObject;
        ListOfSpawnPoints.Add(YellowSpawn);
        GreenSpawn = GameObject.Find("GreenBase").transform.Find("FlagPad").gameObject;
        ListOfSpawnPoints.Add(GreenSpawn);

        for (int i = 0; i< 4; i++)
        {
            TeamCountsList.Add(0);
        }


        TeamID = Player.playerNumber;
        Player.playerNumber++;

        //HomeSpawn = BaseSpawn.TeamList[TeamID];
        

        //EnablePlayer();

        PlayerCanvas.canvas.PlayerObject = this.gameObject;
        DisablePlayer();
    }

    GameObject ThisPlayerNameForTeamAssignment;
    [SerializeField] GameObject PlayerNameForTeamAssignmentPrefab;
    
    
    public void AddNameToTeam(GameObject TeamButton)
    {
        if (ThisPlayerNameForTeamAssignment == null)
        {
            ThisPlayerNameForTeamAssignment = Instantiate(PlayerNameForTeamAssignmentPrefab, TeamButton.transform);
            ThisPlayerNameForTeamAssignment.GetComponent<Text>().text = playerName;
        }
        else
        {
            ThisPlayerNameForTeamAssignment.transform.SetParent(TeamButton.transform);
            ThisPlayerNameForTeamAssignment.transform.localPosition = new Vector3(0, -80, 0);
        }
    }



    void RedTeamClick()
    {
        if (isLocalPlayer)
        {
            CmdRedTeamClick();
        }
    }
    void BlueTeamClick()
    {
        if (isLocalPlayer)
        {
            CmdBlueTeamClick();
        }
    }
    void YellowTeamClick()
    {
        if (isLocalPlayer)
        {
            CmdYellowTeamClick();
        }
    }
    void GreenTeamClick()
    {
        if (isLocalPlayer)
        {
            CmdGreenTeamClick();
        }
    }

    [Command]
    void CmdRedTeamClick()
    {
        SubtractFromThisTeam(ThisTeamID.TeamIDNumber);
        TeamCountsList[0]++;
        Debug.Log(TeamCountsList[0]);
        RpcRedTeamClick();
    }
    [Command]
    void CmdBlueTeamClick()
    {
        SubtractFromThisTeam(ThisTeamID.TeamIDNumber);
        TeamCountsList[1]++;
        Debug.Log(TeamCountsList[1]);
        RpcBlueTeamClick();
    }
    [Command]
    void CmdYellowTeamClick()
    {
        SubtractFromThisTeam(ThisTeamID.TeamIDNumber);
        TeamCountsList[2]++;
        Debug.Log(TeamCountsList[2]);
        RpcYellowTeamClick();
    }
    [Command]
    void CmdGreenTeamClick()
    {
        SubtractFromThisTeam(ThisTeamID.TeamIDNumber);
        TeamCountsList[3]++;
        Debug.Log(TeamCountsList[3]);
        RpcGreenTeamClick();
    }

    void SubtractFromThisTeam(int TeamNumber)
    {
        if (TeamCountsList[TeamNumber] > 0)
        {
            if (TeamNumber == 0)
            {
                TeamCountsList[0]--;
            }
            else if (TeamNumber == 1)
            {
                TeamCountsList[1]--;
            }
            else if (TeamNumber == 2)
            {
                TeamCountsList[2]--;
            }
            else if (TeamNumber == 3)
            {
                TeamCountsList[3]--;
            }
        }
    }

    [ClientRpc]
    void RpcRedTeamClick()
    {
        
            ThisTeamID.TeamIDNumber = 0;
            AddNameToTeam(RedTeamButton.gameObject);
        

    }
    [ClientRpc]
    void RpcBlueTeamClick()
    {
        
        ThisTeamID.TeamIDNumber = 1;
            AddNameToTeam(BlueTeamButton.gameObject);
    }
    [ClientRpc]
    void RpcYellowTeamClick()
    {
        
        ThisTeamID.TeamIDNumber = 2;
            AddNameToTeam(YellowTeamButton.gameObject);
    }
    [ClientRpc]
    void RpcGreenTeamClick()
    {
        
        ThisTeamID.TeamIDNumber = 3;
            AddNameToTeam(GreenTeamButton.gameObject);
    }

    [SerializeField] public GameObject ComputerPlayerPrefab;

    [Server]
    public void StartMatch()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].GetComponent<Player>().RpcStartMatch();
        }

        if (isLocalPlayer)
        {
            int highestNumber = TeamCountsList.Max();
            for (int i = 0; i < 4; i++)
            {
                int difference = highestNumber - TeamCountsList[i];
                if (difference > 0)
                {

                    for (int j = 0; j < difference; j++)
                    {

                        GameObject ComputerTank = Instantiate(ComputerPlayerPrefab, this.gameObject.transform.position, this.gameObject.transform.rotation);
                        
                        NetworkServer.Spawn(ComputerTank);
                        RpcSetComputerTeam(ComputerTank, i);
                        ComputerTank.GetComponent<NavMeshAgent>().enabled = true;
                        ComputerTank.GetComponent<MovementAI>().enabled = true;

                        ComputerTank.GetComponent<Player>().Invoke("Respawn", 5);
                    }
                }
            }
        }
        

    }

    public void RespawnCertainGameobject(GameObject ComputerTank)
    {
        ComputerTank.GetComponent<Player>().Respawn();
    }

    [ClientRpc]
    public void RpcSetComputerTeam(GameObject ComputerTank, int teamNumber)
    {
        ComputerTank.GetComponent<TeamID>().TeamIDNumber = teamNumber;
    }

    [ClientRpc]
    public void RpcStartMatch()
    {
        
        Respawn();
        MatchMenu.SetActive(false);
        
    }

    void Update()
    {
        if(elapsedTimeNoTagBack <= NoTagBackTime)
        {
            //countDown = NoTagBackTime - elapsedTimeNoTagBack;
            //errorText.text = countDown.ToString();
            
        }

        

        elapsedTimeNoTagBack += Time.deltaTime;
    }


    

    [ServerCallback]
    void OnEnable()
    {
        if (!players.Contains(this.gameObject))
            players.Add(this.gameObject);
    }

    [ServerCallback]
    void OnDisable()
    {
        if (players.Contains(this.gameObject))
            players.Remove(this.gameObject);
    }

    void DisablePlayer()
    {
        if (isLocalPlayer)
        {
            PlayerCanvas.canvas.HideReticule();
            //mainCamera.SetActive(true);

        }

        onToggleShared.Invoke(false);

        if (isLocalPlayer)
            onToggleLocal.Invoke(false);
        else
            onToggleRemote.Invoke(false);
    }

    void EnablePlayer()
    {
        if (isLocalPlayer)
        {
            PlayerCanvas.canvas.Initialize();
            //mainCamera.SetActive(false);
        }


        onToggleShared.Invoke(true);

        if (isLocalPlayer)
            onToggleLocal.Invoke(true);
        else
            onToggleRemote.Invoke(true);
    }

    [SerializeField] FlagHolder playerFlagHolder;

    public void Die()
    {
        if (isLocalPlayer)
        {
            PlayerCanvas.canvas.WriteGameStatusText("Respawn in " + respawnTime + " seconds");
            PlayerCanvas.canvas.PlayDeathAudio();
        }


        DisablePlayer();
        Debug.Log("There are " + AIToNotifyWhenYouDie.Count + " to notify of death.");
        for (int i = 0; i < AIToNotifyWhenYouDie.Count; i++)
        {
            Debug.Log("Notifying " + AIToNotifyWhenYouDie[i].name);
            MovementAI AIToNotify = AIToNotifyWhenYouDie[i].GetComponent<MovementAI>();
            if (AIToNotify != null)
            {
                AIToNotify.NotifyOfDeath(this.gameObject);
            }
            else
            {
                TurretAI TurretToNotify = AIToNotifyWhenYouDie[AIToNotifyWhenYouDie.Count - 1].GetComponent<TurretAI>();
                if (TurretToNotify != null)
                {
                    TurretToNotify.NotifyOfDeath(this.gameObject);
                }
            }

        }

        playerFlagHolder.DropFlags();
        Invoke("Respawn", respawnTime);
    }

    [SerializeField] public static GameObject RedSpawn;
    [SerializeField] public static GameObject BlueSpawn;
    [SerializeField] public static GameObject YellowSpawn;
    [SerializeField] public static GameObject GreenSpawn;


    void Respawn()
    {
        if (isLocalPlayer || isAI)
        {
            Debug.Log("Player going to respawn.");

            //Transform spawn = NetworkManager.singleton.GetStartPosition();
            Transform spawn;
            if (ThisTeamID.TeamIDNumber == 0)
            {
                spawn = RedSpawn.transform;
            }
            else if (ThisTeamID.TeamIDNumber == 1)
            {
                spawn = BlueSpawn.transform;
            }
            else if (ThisTeamID.TeamIDNumber == 2)
            {
                spawn = YellowSpawn.transform;
            }
            else if (ThisTeamID.TeamIDNumber == 3)
            {
                spawn = GreenSpawn.transform;
            }
            else return;

            transform.position = spawn.position;
            transform.rotation = spawn.rotation;
            if (playerFlagHolder.ThisMovementAI != null)
            {

                Invoke("InvokePickBase", 10);
            }
        }
        foreach (MeshRenderer meshRenderer in ListOFObjectsToColor)
        {
            meshRenderer.material = playerFlagHolder.MaterialsList[ThisTeamID.TeamIDNumber];
        }

        EnablePlayer();
    }

    void InvokePickBase()
    {
        playerFlagHolder.ThisMovementAI.PickABaseToAttack();
    }

    void OnNameChanged(string value)
    {
        playerName = value;
        gameObject.name = playerName;
        playerNameText.text = playerName;
    }

    void OnElapsedTimeNoTagBackChanged(float value)
    {
        elapsedTimeNoTagBack = value;

        if (isLocalPlayer && elapsedTimeNoTagBack <= NoTagBackTime)
        {
            countDown = NoTagBackTime - elapsedTimeNoTagBack;
            //errorText.text = countDown.ToString();
            //RpcWriteToErrorText(countDown.ToString());
        }
    }

    void OnColorChanged(Color value)
    {
        playerColor = value;
        // GetComponentInChildren ().ChangeColor(playerColor);
    }

    [Server]
    public void Won()
    {
        //Tell other players
        for (int i = 0; i < players.Count; i++)
        {
            players[i].GetComponent<Player>().RpcGameOver(netId, name);
        }

        //NetworkManager.singleton.ServerChangeScene("NetworkProof4");
        //Go back to Lobby
        
    }

    [ClientRpc]
    void RpcGameOver(NetworkInstanceId networkID, string name)
    {
        DisablePlayer();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (isLocalPlayer)
        {
            if (networkID == networkID)
                PlayerCanvas.canvas.WriteGameStatusText("Victory!");
            else
                PlayerCanvas.canvas.WriteGameStatusText("Failure.\n" + name + " Won!");
        }
        Invoke("BackToLobby", 5f);
    }

    [ClientRpc]
    void RpcWriteToErrorText (string stringToWrite)
    {
        //if (isLocalPlayer)
        //errorText.text = stringToWrite;
    }

    void BackToLobby()
    {
        //FindObjectOfType<NetworkLobbyManager>().SendReturnToLobby();
        

        MatchMenu.SetActive(true);

    }

    public List<GameObject> AIToNotifyWhenYouDie = new List<GameObject>();

    public void AddToAINotifyList(GameObject AIGameObject)
    {
        AIToNotifyWhenYouDie.Add(AIGameObject);
    }
}
