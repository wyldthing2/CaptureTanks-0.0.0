/*
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
    private GameObject target = null;
    bool wandering = false;
    bool chasing = false;

    //[SyncVar (hook = "OnBlobHealthChanged")] int blobHealth;
    int blobHealth;
    [SerializeField] GameObject healthDisplay;
    Text healthDisplayText;
    [SerializeField] float giveUpTime = 40f;
    [SerializeField] float startWanderAgainTime = 40f;
    [SerializeField] float jumpForce = 40f;
    [SerializeField] float timeNewTarget;
    [SerializeField] float wanderRadius = 5f;
    [SerializeField] float xDistanceBeforeArrived = 5;
    [SerializeField] float yDistanceBeforeArrived = 5;
    [SerializeField] float zDistanceBeforeArrived = 5;
    bool arrived = true;
    bool readyToWander = false;
   


    LobbyPlayerList pList;
    public List<GameObject> bList = new List<GameObject>();

    float jumpSpeed;
    

    [SerializeField] NavMeshAgent agent;


    //Later, for making AI check position every x seconds
    float elapsedTimeforDetection = 0f;
    float elapsedTimeforJump = 0f;
    float elapsedTimetooFar = 0f;
    float elapsedTimetoNextWander = 0f;

    bool Grounded = false;

    // Use this for initialization
    public void Start()
    {
        //Assign the player to the array

        /*
        errorLog = GameObject.Find("LogText");
        errorText = errorLog.GetComponent<Text>();
        errorText.text = "Crap";
        */

float randsize = (2 * UnityEngine.Random.value);
transform.localScale *= (1 + randsize);
        //blobHealth = 1;
        blobHealth = Convert.ToInt32(Math.Round(10 * randsize));
        
        

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");


//players = GameObject.FindGameObjectsWithTag("Player");

GameObject[] bases = GameObject.FindGameObjectsWithTag("CaptureBase");
bList.AddRange(bases.Where((s, i) => i<bases.Length));
        //Debug.Log(bases.Length);
        //Debug.Log("Bases array: " + bases[0] + " and " + bases[1] + " and " + bases[2]);
        //Debug.Log("Bases list: " + bList[0].ToString() + " and " + bList[1].ToString() + " and " + bList[2].ToString());

        //foreach (String bname in bList.Get)

        randomDestination(wanderRadius);



healthDisplayText = healthDisplay.GetComponent<Text>();
        healthDisplayText.text = blobHealth.ToString();
        //healthDisplay.SetActive(false);

        
        //if (blobHealth < 100)
        //{
        //    healthDisplay.SetActive(true);
        //}
    }

    void randomDestination(float radiusToChooseFrom)
{
    Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radiusToChooseFrom;
    randomDirection += transform.position;
    NavMeshHit hit;
    NavMesh.SamplePosition(randomDirection, out hit, radiusToChooseFrom, 1);
    Vector3 finalPosition = hit.position;
    agent.destination = finalPosition;
    //Debug.Log("Blob chose random spot:" + finalPosition);
}

[Server]
public bool BlobTakeDamage()
{
    bool blobDied = false;

    if (blobHealth <= 0)
        return blobDied;

    blobHealth--;
    blobDied = blobHealth <= 0;

    //RpcTakeDamage(died);


    return blobDied;

}

void ShowHealth(int amount)
{
    //healthDisplayText.text = blobHealth.ToString();
    if (amount < 4)
    {
        //INVOKE to make it fade after 5 secs?
        healthDisplay.SetActive(true);
    }
}

//////void SetBlobHealth(int amount)
/////{
////// healthDisplayText.text = amount.ToString();
/////}

void Update()
{
    //errorText.text = Grounded.ToString();

    //SetBlobHealth(blobHealth);
    healthDisplayText.text = blobHealth.ToString();

    elapsedTimeforDetection += Time.deltaTime;
    elapsedTimeforJump += Time.deltaTime;
    //elapsedTimetooFar += Time.deltaTime;

    if (!wandering && !chasing)
    {
        elapsedTimetoNextWander += Time.deltaTime;
    }

    //Set the player position as their target
    if (elapsedTimeforDetection >= 2f)
    {
        /*
        playerObj = GameObject.FindGameObjectWithTag("Player");
        //agent.destination = playerObj.gameObject.transform.position;

        target = playerObj.gameObject;
        */
        if (target != null)
        {
            agent.destination = target.transform.position;
        }
        else
        {


            if (Math.Abs(agent.destination.x - transform.position.x) <= xDistanceBeforeArrived && Math.Abs(agent.destination.y - transform.position.y) <= yDistanceBeforeArrived && Math.Abs(agent.destination.z - transform.position.z) <= zDistanceBeforeArrived)
            {
                arrived = true;
                //wander in small area or to interesting objects
            }


            if (arrived)
            {
                elapsedTimetoNextWander += Time.deltaTime;
                if (elapsedTimetoNextWander >= startWanderAgainTime)
                {
                    readyToWander = true;

                }
            }


            if (agent.destination == null || readyToWander)
            {
                Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * wanderRadius;
                randomDirection += transform.position;
                NavMeshHit hit;
                NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
                Vector3 finalPosition = hit.position;
                agent.destination = finalPosition;
                //Debug.Log("Blob chose random spot:" + finalPosition);
                //Debug.Log("NavDest: " + agent.destination.x + ", " + agent.destination.y + ", " + agent.destination.z);
                readyToWander = false;
                arrived = false;
                elapsedTimetoNextWander = 0;

            }

        }
        /*foreach (LobbyPlayer playerObj in pList._players)

            {

            Debug Log(playerObj.ToString(););

                //Check for all the players for a <20 player, and check if it's closer than each of the others
                if (Vector3.Distance(this.gameObject.transform.position, playerObj.gameObject.transform.position) < 20 && Vector3.Distance(this.gameObject.transform.position, playerObj.gameObject.transform.position) < Vector3.Distance(this.gameObject.transform.position, target.transform.position) )
                {
                    //Set this Player as the new target
                    agent.destination = playerObj.gameObject.transform.position;
                    target = playerObj.gameObject;

                    elapsedTimetooFar = 0; 

                }




            }
            */

        int logCount = 0;


        if (target == null || elapsedTimetooFar > giveUpTime)
        {
            logCount++;

            Debug.Log(logCount + "th time: " + elapsedTimetooFar);

            int baseCount = 0;

            agent.destination = bList[0].transform.position;
            target = bList[0].gameObject;

            /*
            Debug.Log("Base " + baseCount + " entered");

            foreach (GameObject baseObj in bList)
            {
                baseCount++;
                Debug.Log("Checking base number " + baseCount);
                target = baseObj;
                if (Vector3.Distance(transform.position, baseObj.transform.position) < Vector3.Distance(transform.position, target.transform.position))
                {
                    agent.destination = baseObj.transform.position;
                    target = baseObj.gameObject;
                    Debug.Log("Base " + baseCount + " entered");
                }

            }
            */
            baseCount = 0;
            elapsedTimetooFar = 0;
        }



        //errorText.text = playerObj.transform.position.ToString();


        elapsedTimeforDetection = 0f;

    }

    /*
    if(elapsedTimeforJump >= 8f && elapsedTimeforJump <= 16f)
    {
        jumpSpeed = jumpForce;
        //elapsedTimeforJump = 0;
        transform.Translate(Vector3.forward * jumpSpeed * Time.deltaTime);

        if(elapsedTimeforJump >= 15)
        {
            elapsedTimeforJump = 0;
        }

    }
    */

    Grounded = false;

}


    /*
    void OnBlobHealthChanged(int value)
    {
        blobHealth = value;

      ///  SetBlobHealth(value);
     ///   ShowHealth(value);
    }
    */

    */
}








    using UnityEngine;
using UnityEngine.Networking;

public class PlayerShootingNew : NetworkBehaviour
{
    [SerializeField] float gunRange = 1000f;
    [SerializeField] float shotCooldown = .1f;
    [SerializeField] int killsToWin = 5;
    [SerializeField] Transform firePosition;
    [SerializeField] ShotEffectsManager shotEffects;
    [SerializeField] GameObject pointsSlider;
    //[SerializeField] GameObject blobSpawner;

    [SyncVar(hook = "OnScoreChanged")] int score;

    Player player;
    //BlobSpawner blobSpawnScript;
    float elapsedTime;
    bool canShoot;
    float maxBlobKills = 20;
    float percentBlobKills;

    void Start()
    {
        player = GetComponent<Player>();
        shotEffects.Initialize();

        if (isLocalPlayer) ;

        //blobSpawnScript = blobSpawner.GetComponent<BlobSpawner>();

        //Debug.Log(blobSpawner.transform.position);
    }

    [ServerCallback]
    void OnEnable()
    {
        score = 0;
    }

    void Update()
    {
        if (!canShoot)
            return;



        elapsedTime += Time.deltaTime;

        if (Input.GetButtonDown("Fire1") && elapsedTime > shotCooldown)
        {
            elapsedTime = 0f;
            CmdFireShot(firePosition.position, firePosition.forward);
            Debug.Log("Command Fire");
        }
    }

    [Command]
    void CmdFireShot(Vector3 origin, Vector3 direction)
    {
        RaycastHit hit;

        Ray ray = new Ray(origin, direction);
        Debug.DrawRay(ray.origin, ray.direction * 3f, Color.red, 1f);
        Debug.Log("Command Fire");

        bool result = Physics.Raycast(ray, out hit, gunRange);

        if (result)
        {


            PlayerHealth enemy = hit.transform.GetComponent<PlayerHealth>();
            AIJumper enemyBlob = hit.transform.GetComponent<AIJumper>();


            if (hit.transform.tag == "Blob")
            {
                bool wasKillShot = enemyBlob.BlobTakeDamage();


                if (wasKillShot)
                {

                Destroy(hit.transform.gameObject);

                //blobSpawnScript.RpcSingleBlobSpawn(); 

                if (score < maxBlobKills)
                {
                    ++score;
                }
                shotEffects.PlayDeathEffect(hit.transform.position);
                //}
            }


            if (enemy != null)
            {
                bool wasKillShot = enemy.TakeDamage();



                if (wasKillShot && ++score >= killsToWin)
                    player.Won();
            }
        }

        RpcProcessShotEffects(result, hit.point);
    }

    [ClientRpc]
    void RpcProcessShotEffects(bool playImpact, Vector3 point)
    {
        shotEffects.PlayShotEffects();

        if (playImpact)
            shotEffects.PlayImpactEffect(point);
    }

    void OnScoreChanged(int value)
    {
        score = value;
        if (isLocalPlayer)
        {
            PlayerCanvas.canvas.SetKills(value);
        }

        percentBlobKills = score / maxBlobKills;

        //if (percentBlobKills > 0)
        //{
        //pointsSlider.canvas.enabled = true;

        //}
        //else pointsSlider.enabled = false;
    }

    public void FireAsBot()
    {
        CmdFireShot(firePosition.position, firePosition.forward);
    }
}
