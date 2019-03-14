using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeToFollowAt : MonoBehaviour {

    [SerializeField] MovementAI ThisMovementAI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == ThisMovementAI.TargetToChase)
        {
            ThisMovementAI.HoldPositionButTurnAsNeeded();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == ThisMovementAI.TargetToChase)
        {
            ThisMovementAI.ResumeNavAgentMovement();
        }
    }
}
