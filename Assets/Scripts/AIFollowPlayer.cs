using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AIFollowPlayer : NetworkBehaviour {

    //Array of players (added to whenever people join[still need loop for this, right now it's just finding the player])
    private GameObject playerObj = null;
    private GameObject errorLog;
    private Text errorText;

    //Later, for making AI check position every x seconds
    float elapsedTime = 0f;

    // Use this for initialization
    void Start()
    {
        //Assign the player to the array

        

        errorLog = GameObject.Find("LogText");
        errorText = errorLog.GetComponent<Text>();

        if (playerObj == null)
            playerObj = GameObject.FindGameObjectWithTag("Player");

        errorText.text = playerObj.transform.position.ToString();

    }

    void Update()
    {
        

        elapsedTime += Time.deltaTime;

        //Set the player position as their target
        if (elapsedTime >= 2f)
        {

            errorText.text = playerObj.transform.position.ToString();


            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            agent.destination = playerObj.transform.position;



            elapsedTime = 0f;

        }



    }
}