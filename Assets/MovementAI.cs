using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections.Generic;
using Prototype.NetworkLobby;
using System;
using System.Linq;




public class MovementAI : NetworkBehaviour {
    int _debugCount =0;
    //Array of players (added to whenever people join[still need loop for this, right now it's just finding the player])
    private GameObject playerObj = null;
    private GameObject errorLog;
    private Text errorText;
    [SerializeField] public GameObject Target = null;
    [SerializeField] public GameObject BaseTarget = null;
    [SerializeField] public float SpawnTimeStamp;


    //[SyncVar (hook = "OnBlobHealthChanged")] int blobHealth;
    [SerializeField] [SyncVar(hook = "OnBlobDestinationChanged")] Vector3 BlobDestination;
    [SerializeField] [SyncVar(hook = "OnBlobPositionChanged")] Vector3 BlobPosition;

    [SerializeField] Text healthDisplay;
    Text healthDisplayText;
    [SerializeField] float detectionTime = .5f;
    [SerializeField] float giveUpTime = 40f;
    [SerializeField] float startWanderAgainTime = 10f;
    [SerializeField] float syncPositionTime = 5f;
    [SerializeField] float syncPositionDistance = 5f;
    [SerializeField] float distanceBeforeSharingToClients = 5f;
    [SerializeField] float syncSmoothing = 5f;
    [SerializeField] float jumpForce = 40f;
    [SerializeField] float timeNewTarget;
    [SerializeField] float wanderRadius = 100f;
    [SerializeField] float DistanceBeforeArrived = 5;
    [SerializeField] float DistanceForDetection = 20;
    [SerializeField] float DistanceBeforeReturningToBase = 50;
    [SerializeField] bool arrived = true;
    [SerializeField] bool readyToWander = false;
    [SerializeField] bool hasADestination = false;

    [SerializeField] Vector3 previousPatrolZone;
    [SerializeField] public GameObject HomeBase;
    [SerializeField] public int ThisTeamID;
    [SerializeField] public GameObject TargetToChase;
    [SerializeField] public bool TurnInPlaceBool;
    [SerializeField] public int AIMode;
    //0 Capture and return flags
    //1 ChaseTarget
    //2 Flee
    //3 Patrol

    [SerializeField] public GameObject CaptureTarget;

    [SerializeField] private Player playerFunctions;
    [SerializeField] private FlagHolder _myFlagHolder;

    [SerializeField] List<GameObject> TurnTestList = new List<GameObject>();
    public static List<MovementAI> ListOfMovementAIs = new List<MovementAI>();

    //[SerializeField] public NetworkProximityChecker ProximityChecker;





    //LobbyPlayerList pList;
    //public List<GameObject> bList = new List<GameObject>();

    float jumpSpeed;


    [SerializeField] public NavMeshAgent agent;

    //static counter on class, constructor you implenet it (counter = % 
    //set private variabel for myFrame
    //if (time.framecount % 60 == counter
    //time.realTimeSinceSetup of time.Time

    //Later, for making AI check position every x seconds
    [SerializeField] float elapsedTimeforDetection = 0f;
    float elapsedTimeforJump = 0f;
    float elapsedTimetooFar = 0f;
    [SerializeField] float elapsedTimetoNextWander = 0f;
    [SerializeField] float elapsedTimeForNotMoving = 0f;
    [SerializeField] float elapsedTimeForSync = 0f;
    Vector3 previousLocationAtLastCheck;

    bool Grounded = false;

    [Server]
    public void ServerPositionShare()
    {

        if (Vector3.Distance(BlobPosition, this.transform.position) > distanceBeforeSharingToClients)
        {
            //This sets the syncvar BlobPosition to the server gameobject's position
            BlobPosition = this.transform.position;

        }

    }

    void _syncPosition()
    {
        if (Vector3.Distance(BlobPosition, this.transform.position) > syncPositionDistance)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, BlobPosition, Time.deltaTime * syncSmoothing);
        }
    }

    

    
    // Use this for initialization
    public void Start()
    {


        //ProximityChecker.enabled = true;
        SpawnTimeStamp = Time.time;
        ThisTeamID = playerFunctions.ThisTeamID.TeamIDNumber;

        //if (!BlobSpawnController.commands.BlobList.Contains(this.gameObject))
        //BlobSpawnController.commands.BlobList.Add(this.gameObject);

        _addAllTheBasesAsObjectives();

        //randomDestination(wanderRadius);
        Invoke("PickABaseToAttack", 15);

        //PickABaseToAttack();
        if (!ListOfMovementAIs.Contains(this))
        {
            ListOfMovementAIs.Add(this);
        }
        
    }
    

    void _addAllTheBasesAsObjectives()
    {
        for (int i = 0; i < playerFunctions.ListOfSpawnPoints.Count; i++)
        {
            if (i == playerFunctions.ThisTeamID.TeamIDNumber)
            {
                Objective objective = new Objective();
                objective.TargetObject = playerFunctions.ListOfSpawnPoints[i];
                HomeBase = objective.TargetObject;
                objective.Name = "Patrol";
                ObjectivesList.Add(objective);

            }
            else
            {
                Objective objective = new Objective();
                objective.TargetObject = playerFunctions.ListOfSpawnPoints[i];
                objective.Name = "CaptureFlag";
                ObjectivesList.Add(objective);
            }
        }
    }

    public void PickABaseToAttack()
    {
        List<GameObject> BasesToAttack = new List<GameObject>();
        for (int i = 0; i < ObjectivesList.Count; i++)
        {
            if (ObjectivesList[i].Name == "CaptureFlag")
            {
                if (ObjectivesList[i].TargetObject.GetComponent<FlagHolder>().FlagsCount > 0)
                {
                    BasesToAttack.Add(ObjectivesList[i].TargetObject);
                }
            }
        }

        //Random select a base
        if (Vector3.Distance(HomeBase.transform.position, this.gameObject.transform.position) < 5)
        {
            float rand = (UnityEngine.Random.value);
            int indexNumberToSelect = Convert.ToInt32(Math.Round(((BasesToAttack.Count - 1) * rand)));
            if (BasesToAttack.Count != 0)
            {
                if (indexNumberToSelect > BasesToAttack.Count)
                {
                    indexNumberToSelect = BasesToAttack.Count;
                }
            }
            else if (BasesToAttack.Count == 0)
            {
                Debug.Log("Not a base in da woild");
                return;
            }

            CaptureTarget = BasesToAttack[indexNumberToSelect];
        }
        else
        {
            for (int i = 0; i < BasesToAttack.Count; i++)
            {
                if (CaptureTarget != null && Vector3.Distance(CaptureTarget.transform.position,this.transform.position) > Vector3.Distance(BasesToAttack[i].transform.position,this.transform.position))
                {
                    CaptureTarget = BasesToAttack[i];
                }
                else if (CaptureTarget == null)
                {
                    CaptureTarget = BasesToAttack[i];
                }
            }
        }
        
        
        Debug.Log("Selected base at " + CaptureTarget.transform.position);
        agent.SetDestination(CaptureTarget.transform.position);
        AIMode = 0;
    }

    public void ReturnToBaseWithFlag()
    {
        CaptureTarget = ObjectivesList[playerFunctions.ThisTeamID.TeamIDNumber].TargetObject;
        agent.SetDestination(CaptureTarget.transform.position);
        AIMode = 0;
    }

    public void ResumeCaptureMode()
    {
        agent.SetDestination(CaptureTarget.transform.position);
        AIMode = 0;
    }

    Vector3 PreviousPosition;

    public void FixIdleness()
    {
        
        if (CaptureTarget != null && PreviousPosition == this.transform.position)
        {
            ResumeCaptureMode();
        }
        else if (CaptureTarget == null && PreviousPosition == this.transform.position)
        {
            PickABaseToAttack();
        }
        PreviousPosition = this.transform.position;
    }

    public void SetTargetEnemyToChase(GameObject gameObject)
    {
        TargetToChase = gameObject;
        gameObject.GetComponent<Player>().AIToNotifyWhenYouDie.Add(this.gameObject);
        AIMode = 1;
    }

    public void HoldPositionButTurnAsNeeded()
    {
        agent.isStopped = true;
        TurnInPlaceBool = true;
    }

    void TurnInPlace()
    {
        Vector3 dir = TargetToChase.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(this.gameObject.transform.rotation, lookRotation, Time.deltaTime * 2).eulerAngles;
        this.gameObject.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    [SerializeField] float AttackResponseCooldown = 20;
    float LastCheckTime = 0;
    bool RespondingToAttack = false;
    public Vector3 SpotWhereBulletHit;
    public void TurnToCheckASpot(Vector3 SpotToCheck)
    {
        if (!RespondingToAttack && Time.time-LastCheckTime > AttackResponseCooldown)
        {
            float rand = UnityEngine.Random.value;
            if (rand > .6)
            {
                RespondingToAttack = true;
                SpotWhereBulletHit = SpotToCheck;
                agent.isStopped = true;
            }
        }

        if (RespondingToAttack)
        {
            Vector3 dir = SpotToCheck - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotation = Quaternion.Lerp(this.gameObject.transform.rotation, lookRotation, Time.deltaTime * 2).eulerAngles;
            this.gameObject.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
            if (Vector3.Angle(dir, this.gameObject.transform.forward) <= 30)
            {
                RespondingToAttack = false;
                agent.isStopped = false;
            }
        }
    }

    public void ResumeNavAgentMovement()
    {
        agent.isStopped = false;
        TurnInPlaceBool = false;
    }

    [Server]
    void randomDestination(float radiusToChooseFrom)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radiusToChooseFrom;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, radiusToChooseFrom, 1);
        Vector3 finalPosition = hit.position;
        SetAIDestination(finalPosition);
        //agent.destination = finalPosition;
        Debug.Log("Blob chose random spot:" + finalPosition);
        hasADestination = true;
    }



    /*
    void ShowHealth(int amount)
    {
        //healthDisplayText.text = blobHealth.ToString();
        if (amount < 4)
        {
            //INVOKE to make it fade after 5 secs?
            healthDisplay.SetActive(true);
        }
    }
    */

    //////void SetBlobHealth(int amount)
    /////{
    ////// healthDisplayText.text = amount.ToString();
    /////}

    [Server]
    GameObject GetClosestGameObject(List<GameObject> targets)
    {
        GameObject closestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (GameObject potentialTarget in targets)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            float dSqrForDetection = new Vector3(DistanceForDetection, DistanceForDetection, DistanceForDetection).sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr && dSqrToTarget < dSqrForDetection)
            {
                closestDistanceSqr = dSqrToTarget;
                closestTarget = potentialTarget;
            }
        }

        return closestTarget;
    }

    [Server]
    public void SetAIDestination(Vector3 assignedPosition)
    //public void RpcSetRemoteAIDestination(NavMeshAgent blobAgent)
    {
        agent.destination = assignedPosition;
        BlobDestination = assignedPosition;
        //agent.path = blobAgent.path;
        

    }

    //remember the last place you saw someone, where they might be headed

    //pick objective (maybe have a list of objectives that have points to help priotize them)
    [System.Serializable] public class Objective
    {
        public string Name;
        public GameObject TargetObject;
        public double PriorityScore;
        public int AttemptsWithoutProgress; //erase this after a while?, erase it after enough changes?
        public int NumberOfFlagsAtObjective;

        public void CalculatePriorityScore(GameObject thisGameObject)
        {
            //NavMeshPath path = new NavMeshPath();
            NavMeshAgent agent = new NavMeshAgent();
            NavMesh.CalculatePath(thisGameObject.transform.position, TargetObject.transform.position, NavMesh.AllAreas, agent.path);
            this.PriorityScore = agent.remainingDistance; //divided by max distance for this map
            
            if(this.Name == "Patrol")
            {
                //ally/defenses SUBTRACTS from score
            }
            else if (this.Name == "CaptureFlag")
            {
                //ally/defenses ADDS to score
            }
            //distance
            //do I have an ally OR defenses?
            //how many times hae I tried this and failed?
            //Did I just see the base lose defenses? (has to be recent because of respawn time)
            //enemy patrolling base? (saw him there, saw him elsewhere
        }
    }

    [SerializeField] public List<Objective> ObjectivesList = new List<Objective>();
    

    [System.Serializable] public class ItemOfInterest
    {
        public string Name;
        public GameObject TargetObject;
        public float DistanceWillingToDepartFromObjective;
    }
    
    //patrol
    //capture flag

    //item of interest passes 
    //information about the first objective determines whether they will return to it after distraction (time, distance, objective
    //still there, saw an enemy with the flag they were after[maybe just cheat to know where flags are, and if someone JUST took it,
    //try to get them if they are close... how can you get an AI to realize that they already know that no one was found on the way they came?])

    //distance willing to chase the interest
    //is something close to taking it first?

    //enemy
    //has flag?
    //headed to my base?
    //low health? (or I have a ton of fire power? (maybe a higher AI tries to bluff people with heavy initial barrage))
    
    //barrier

    //weapon

    //easy base or dropped flags

    //my base under attack OR ally under attack
    //is it close enough? are there any flags to defend?

    //gets shot


    [Server]
    public void Movement()
    {

        elapsedTimeforDetection += Time.deltaTime;
        elapsedTimeforJump += Time.deltaTime;
        elapsedTimeForNotMoving += Time.deltaTime;
        //elapsedTimetooFar += Time.deltaTime;

        //remember the last place you saw someone, where they might be headed

        //pick objective (maybe have a list of objectives that have points to help priotize them)
        //patrol
        //capture flag

        //item of interest passes 
            //information about the first objective determines whether they will return to it after distraction (time, distance, objective
            //still there, saw an enemy with the flag they were after[maybe just cheat to know where flags are, and if someone JUST took it,
            //try to get them if they are close... how can you get an AI to realize that they already know that no one was found on the way they came?])

            //enemy
                //has flag?
                //headed to my base?
                //low health? (or I have a ton of fire power? (maybe a higher AI tries to bluff people with heavy initial barrage))

            //weapon

            //easy base or dropped flags

            //my base under attack OR ally under attack
                //is it close enough? are there any flags to defend?


        if (arrived)
        {
            elapsedTimetoNextWander += Time.deltaTime;
            if (elapsedTimetoNextWander >= startWanderAgainTime)
            {
                readyToWander = true;

            }
        }

        if (elapsedTimeForNotMoving > 5)
        {
            agent.destination = BlobDestination;

            //I think this only is needed during wander AI, when it picks something off the map. Navmesh does a god job finding a way around otherwise
            if (previousLocationAtLastCheck == this.transform.position)
            {
                readyToWander = true;

                arrived = true;
                elapsedTimeForNotMoving = 0;
            }

            previousLocationAtLastCheck = this.transform.position;
        }




        //Set the player position as their target
        if (elapsedTimeforDetection >= detectionTime)
        {
            randomDestination(wanderRadius);


            if (GetClosestGameObject(Player.players) != null)
            {
                Target = GetClosestGameObject(Player.players);
            }

            if (Vector3.Distance(transform.position, agent.destination) <= DistanceBeforeArrived)
            {

                arrived = true;

                if (BaseTarget != null && Vector3.Distance(agent.destination, BaseTarget.transform.position) <= DistanceBeforeArrived)
                {
                    BaseTarget = null;
                }

                //wander in small area or to interesting objects
                hasADestination = false;
            }



            if (Target != null)
            {
                //agent.destination = Target.transform.position;
                SetAIDestination(Target.transform.position);

                if (Vector3.Distance(transform.position, Target.transform.position) > 200)
                {
                    elapsedTimetooFar += detectionTime;

                    if (elapsedTimetooFar > 5)
                    {
                        elapsedTimetooFar = 0;
                        Target = null;
                    }


                }

            }
            else
            {
                if (BaseTarget != null)
                {
                    readyToWander = false;
                    //agent.destination = BaseTarget.transform.position;
                    SetAIDestination(BaseTarget.transform.position);
                }

                if (agent.destination == null || readyToWander)
                {

                    randomDestination(wanderRadius);

                    readyToWander = false;
                    arrived = false;
                    elapsedTimetoNextWander = 0;
                    hasADestination = true;

                }

            }

            elapsedTimeforDetection = 0f;

        }
    }

    public void NotifyOfDeath(GameObject DeadObject)
    {
        if (TargetToChase == DeadObject)
        {

            TargetToChase = null;
            AIMode = 0;
        }
    }

    void Update()
    {

        if (elapsedTimeForSync > syncPositionTime)
        {
            //ServerPositionShare();
            //_syncPosition();
            //elapsedTimeForSync = 0;
        }
        elapsedTimeForSync += Time.deltaTime;


        //Movement();

        



    }

    //int TargetID = 0;

    //void SwitchTurnTarget()
    //{
    //    TargetToChase = TurnTestList[TargetID];
    //    TargetID++;
    //    if (TargetID >= TurnTestList.Count)
    //    {
    //        TargetID = 0;
    //    }
    //    HoldPositionButTurnAsNeeded();
    //    
    //}

    private void FixedUpdate()
    {
        if (AIMode == 0 && CaptureTarget != null)
        {
            
        }
        else if (AIMode == 1)
        {
            if (TurnInPlaceBool)
            {
                TurnInPlace();
            }

            if (Time.time % 2 == 0)
            {
                agent.destination = TargetToChase.transform.position;
            }
        }

        if (RespondingToAttack)
        {
            TurnToCheckASpot(SpotWhereBulletHit);
        }

    }




    void OnBlobDestinationChanged(Vector3 vector)
    {
        BlobDestination = vector;
        agent.destination = vector;
    }

    void OnBlobPositionChanged(Vector3 vector)
    {
        BlobPosition = vector;
        agent.destination = vector;
    }
}
