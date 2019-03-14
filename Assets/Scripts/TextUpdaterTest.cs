using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextUpdaterTest : MonoBehaviour
{

    private GameObject playerObj = null;
    private GameObject errorLog;
    private Text errorText;
    float elapsedTime = 0f;



    // Use this for initialization
    void Start()
    {
        //Assign the player to the array
        //NavMeshAgent agent = GetComponent<NavMeshAgent>();

        errorText = GetComponent<Text>();
        errorText.text = "Poopy";

        if (playerObj == null)
            playerObj = GameObject.Find("Jumper1");

    }

    void Update()
    {

        elapsedTime += Time.deltaTime;

        //Set the player position as their target
        if (elapsedTime >= 2f)
        {
            //errorText.text = playerObj.AIJumper.Grounded.ToString();
            errorText.text = playerObj.transform.position.ToString();

            elapsedTime = 0f;





        }
    }
}