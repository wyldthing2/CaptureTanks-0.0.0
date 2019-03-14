using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class IdleAIFixer : NetworkBehaviour {

    MovementAI MovementAI;
	[SerializeField] float IdleCheckInterval = 10;
    float LastCheckTime = 0;

	// Update is called once per frame
    [Server]
	void Update ()
    {

        if (Time.time - LastCheckTime >= IdleCheckInterval)
            for (int i = 0; i < MovementAI.ListOfMovementAIs.Count; i++)
            {
                MovementAI.ListOfMovementAIs[i].FixIdleness();
            }
	}
}
