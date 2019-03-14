using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FlagPuller : NetworkBehaviour {

    [SerializeField] public GameObject ObjectToPullFrom;
    [SerializeField] private FlagHolder FlagHolderToGiveTo;

    public void PullFlag(GameObject givingObject)
    {
        
        
    }

    public void GiveFlag(GameObject recievingObject)
    {

    }

}
