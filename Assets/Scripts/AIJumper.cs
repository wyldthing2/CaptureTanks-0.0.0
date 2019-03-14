using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections.Generic;
using Prototype.NetworkLobby;
using System;
using System.Linq;

public class AIJumper : NetworkBehaviour
{

    //Array of players (added to whenever people join[still need loop for this, right now it's just finding the player])
    private GameObject playerObj = null;
    private GameObject errorLog;
    private Text errorText;
    [SerializeField] public GameObject Target = null;
    [SerializeField] public GameObject BaseTarget = null;
    [SerializeField] public float SpawnTimeStamp;


    //[SyncVar (hook = "OnBlobHealthChanged")] int blobHealth;
    [SyncVar(hook = "OnBlobHealthChanged")] int blobHealth;
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

    [SerializeField] private Player playerFunctions;

    [SerializeField] public NetworkProximityChecker ProximityChecker;

    public float blobSize;




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
        
        if (Vector3.Distance(BlobPosition,this.transform.position) > distanceBeforeSharingToClients)
        {
            //This sets the syncvar BlobPosition to the server gameobject's position
            BlobPosition = this.transform.position;

        }
        
    }

    void _syncPosition()
    {
        if (Vector3.Distance(BlobPosition,this.transform.position) > syncPositionDistance)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, BlobPosition, Time.deltaTime * syncSmoothing);
        }
    }

    [Server]
    void setHealth()
    {
        float randsize = (2 * UnityEngine.Random.value * .5f);
        blobHealth = Convert.ToInt32(Math.Round(1 + (10 * randsize * .5f)));
        Debug.Log("Regular Size");
        RpcSetSize(randsize);

    }

    [ClientRpc]
    void RpcSetSize(float sizeToSet)
    {
        transform.localScale *= (1 + sizeToSet);
        Debug.Log("Rpc Size");
    }

    // Use this for initialization
    public void Start()
    {
        

        setHealth();
        ProximityChecker.enabled = true;
        SpawnTimeStamp = Time.time;

        if (!BlobSpawnController.commands.BlobList.Contains(this.gameObject))
            BlobSpawnController.commands.BlobList.Add(this.gameObject);



        randomDestination(wanderRadius);

        //healthDisplayText = healthDisplay.GetComponent<Text>();
        healthDisplay.text = blobHealth.ToString();
        //healthDisplay.SetActive(false);


        //if (blobHealth < 100)
        //{
        //    healthDisplay.SetActive(true);
        //}
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
        //Debug.Log("Blob chose random spot:" + finalPosition);
        hasADestination = true;
    }

    
    public bool BlobTakeDamage()
    {
        bool blobDied = false;

        if (blobHealth <= 0)
            return blobDied;
        
        blobHealth--;
        blobDied = blobHealth <= 0;



        return blobDied;

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
        BlobDestination = assignedPosition;
        //agent.path = blobAgent.path;

    }


    [Server]
    public void MovementAI()
    {

        elapsedTimeforDetection += Time.deltaTime;
        elapsedTimeforJump += Time.deltaTime;
        elapsedTimeForNotMoving += Time.deltaTime;
        //elapsedTimetooFar += Time.deltaTime;


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

            //I think this only is needed during wander AI, when it picks something off the map. Navmesh does a god job finding a way around otherwise
            if (previousLocationAtLastCheck == this.transform.position)
            {
                readyToWander = true;

                arrived = true;
            }

            previousLocationAtLastCheck = this.transform.position;
        }




        //Set the player position as their target
        if (elapsedTimeforDetection >= detectionTime)
        {


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

    void Update()
    {
        elapsedTimeForSync += Time.deltaTime;

        if (elapsedTimeForSync > syncPositionTime)
        {
            ServerPositionShare();
            _syncPosition();
            elapsedTimeForSync = 0;
        }

        MovementAI();


    }


    
    void OnBlobHealthChanged(int value)
    {
        blobHealth = value;
        healthDisplay.text = value.ToString();
        ///  SetBlobHealth(value);
        //ShowHealth(value);
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
