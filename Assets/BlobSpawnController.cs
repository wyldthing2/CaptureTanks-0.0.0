using UnityEngine.Networking;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class BlobSpawnController : NetworkBehaviour {

    
    public static BlobSpawnController commands;
    
    
    [SerializeField] [SyncVar (hook = "OnBlobCountChange")] public float BlobCount;
    [SerializeField] int mutationLimit = 1000;
    [SerializeField] [SyncVar(hook = "OnMutationCountChange")] public int mutationCount = 0;
    [SerializeField] int spawnLimit = 500;
    //[SerializeField] [SyncVar (hook = "OnMutationPercentChange")] float mutationPercent;
    [SerializeField] public PlayerCanvas theCanvas;
    [SerializeField] public float spawnQueue = 0;
    [SerializeField] float spawnInterval = 10f;
    private float _numberPerSpawn = 4;
    [SerializeField] private float elapsedTime = 0;
    [SerializeField] float swarmInterval;
    [SerializeField] float elapsedTimeToSwarm = 0;
    [SerializeField] GameObject swarmBase;

    [SerializeField] public List<BlobSpawner> SpawnerList = new List<BlobSpawner>();
    public List<GameObject> BaseList = new List<GameObject>();
    public List<GameObject> BlobList = new List<GameObject>();

    //public LiveBlobs[]
    //public DeadBlobs[]


    void Awake()
    {
        if (commands == null)
            commands = this;

        else if (commands != this)
            Destroy(gameObject);

        BaseList.AddRange(GameObject.FindGameObjectsWithTag("CaptureBase"));
    }



    // Use this for initialization
    void Start ()
    {
        BlobCount = 0;
        elapsedTime = 0;
        swarmInterval = 60 + Random.value * 240;
    }

    int counter;

    // Update is called once per frame
    void Update ()
    {
        elapsedTime += Time.deltaTime;
        elapsedTimeToSwarm += Time.deltaTime;

        if (elapsedTimeToSwarm >= swarmInterval)
        {
            elapsedTimeToSwarm = 0;

            PickRandomBaseToSwarm(BaseList);

            //PickRandomBaseToSwarm(BaseList);

            swarmInterval = 30 + Random.value * 120;

        }

        
        
        //if (Time.frameCount % 600 == 0)
        //{
            
          //  counter++;
            //Debug.Log(counter);
            
        //}
        

        if (elapsedTime > 2 && elapsedTime < 32)
        {
            elapsedTime = 0;

            //make mutation go up faster when more bases are captured
            mutationCount += 2;

            if (spawnQueue < spawnLimit)
            {
                spawnQueue += 1;
            }
            else
            {
                mutationCount += 2;
            }

            if (mutationLimit > mutationCount)
            {
                //mutate the blob at the top of the list, maybe bigger mutation if the gap is too big. And big ones just for fun
            }

            if (spawnQueue > 0)
            if (BlobCount < spawnLimit)
            {
                    BaseSpawns(spawnQueue/32);
                CommandSpawnRandomPlace(100, Mathf.RoundToInt(spawnQueue / 32));
            }


        }

        

        //at each spawn, increase the count


        //if (blobCount > mutationLimit)
        

		
	}

    
    
    
    public void AddToSpawnQueue(float numberToAddToSpawnQueue)
    {
        spawnQueue += numberToAddToSpawnQueue;
    }

    public void CommandSpawn(float _numberToSpawnAtEachBase) //make it so you specify the number and it divides them among all spawners //compare to BlobList.Count and save to a log if they ever disagree
    {
        

        if (spawnQueue > 0 /*&& (spawnQueue -= _numberPerSpawn) >= 0*/)
        {
            //for (int i = 0; i < SpawnerList.Count; i++)
            //{
                //spawnerList[i].numberPerSpawn = _numberPerSpawn;
                spawnQueue--;
                SpawnerList[2].BlobSpawn(_numberToSpawnAtEachBase);
            //}

        }
        //start() all spawners add themselves to the spawn list
        //Then this tells the members to spawn (check to see if they are still blob-owned)

    }

    public void BaseSpawns(float _numberToSpawnAtEachBase) //make it so you specify the number and it divides them among all spawners //compare to BlobList.Count and save to a log if they ever disagree
    {


        if (spawnQueue > 0 /*&& (spawnQueue -= _numberPerSpawn) >= 0*/)
        {
            for (int i = 0; i < BaseList.Count; i++)
            {
                CommandSpawnHere(BaseList[i].transform,2);
                spawnQueue -= 2;
            }

        }
        //start() all spawners add themselves to the spawn list
        //Then this tells the members to spawn (check to see if they are still blob-owned)

    }

    private int _nextSpawnerToPick = 0;

   
    [Server]
    public void CommandSpawnHere(Transform here, int numberToSpawn)
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(here.position, out hit, 50, 1);
        SpawnerList[0].transform.position = hit.position;
        SpawnerList[0].BlobSpawn(numberToSpawn);
    }

    [Server]
    public void CommandSpawnRandomPlace(float radiusToChooseFrom, int numberToSpawn)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radiusToChooseFrom;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, radiusToChooseFrom, 1);
        SpawnerList[_nextSpawnerToPick].transform.position = hit.position;
        SpawnerList[_nextSpawnerToPick].BlobSpawn(numberToSpawn);
        Debug.Log("RandomSpawned");
        _nextSpawnerToPick++;
        if (_nextSpawnerToPick == SpawnerList.Count)
        {
            _nextSpawnerToPick = 1;
        }
    }

void PickRandomBaseToSwarm(List<GameObject> bases)
    {
        GameObject chosenBase = null;
        int i = 0;
        int numberOfBlobsForAttack = 0;
        //List<GameObject> copiedBlobList = bases; //for more random blobs later


        //After random time period


        //Pick a base at random
        chosenBase = bases[UnityEngine.Random.Range(0, bases.Count)];

        //Tell random portion of blobs to go (have big shaky notification for Rictor scale), split portion in half for blob bases
        

        numberOfBlobsForAttack = Mathf.FloorToInt(Mathf.Round(BlobSpawnController.commands.BlobList.Count*((float)UnityEngine.Random.Range(1, 8)/16)));

        
        Debug.Log(numberOfBlobsForAttack);

        for (i = 0; i < 100; i++)
        {
            BlobSpawnController.commands.BlobList[i].GetComponent<AIJumper>().BaseTarget = chosenBase;
            Debug.Log(BlobSpawnController.commands.BlobList[i].GetComponent<AIJumper>().agent.destination);
        }


    }

    void PickBaseToSwarm(GameObject baseToAttack)
    {
        GameObject chosenBase = null;
        int i = 0;
        int numberOfBlobsForAttack = 0;
        //List<GameObject> copiedBlobList = bases; //for more random blobs later


        //After random time period


        //Pick a base at random
        chosenBase = baseToAttack;

        //Tell random portion of blobs to go (have big shaky notification for Rictor scale), split portion in half for blob bases


        numberOfBlobsForAttack = Mathf.FloorToInt(Mathf.Round(BlobSpawnController.commands.BlobList.Count * ((float)UnityEngine.Random.Range(1, 8) / 16)));


        Debug.Log(numberOfBlobsForAttack);

        for (i = 0; i < 100; i++)
        {
            BlobSpawnController.commands.BlobList[i].GetComponent<AIJumper>().BaseTarget = chosenBase;
            //Debug.Log(BlobSpawnController.commands.BlobList[i].GetComponent<AIJumper>().agent.destination);
        }


    }

    
    void OnMutationCountChange(int value)
    {
        mutationCount = value;
        theCanvas.SetMutationMeter(value/ mutationLimit);
    }
    public void OnBlobCountChange(float value)
    {
        BlobCount = value;
        theCanvas.SetMutationMeter(value / 500);
    }

}
