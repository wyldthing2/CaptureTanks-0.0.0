using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DetectTargetTrigger : NetworkBehaviour {

    [SerializeField] TurretAI AIToAlert;
    private int ThisTeamID;

    private void Start()
    {
        ThisTeamID = this.transform.GetComponentInParent<TeamID>().TeamIDNumber;

    }

    //check if it's behind a wall
    //prioritizing targets, a ratio of health left to damage output to whether they have a flag. It can check every 5 seconds
    //whether they need to cover an ally, whether they are closer to the flags, whether they are fast

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (other.gameObject.GetComponent<TeamID>().TeamIDNumber != ThisTeamID)
            {
                Debug.Log("Detected an enemy Player");
                AIToAlert.targetObject = other.gameObject;
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (other.gameObject.GetComponent<TeamID>().TeamIDNumber != ThisTeamID)
            {
                Debug.Log("Enemy player left");
                AIToAlert.targetObject = null;
            }

        }
    }
}
