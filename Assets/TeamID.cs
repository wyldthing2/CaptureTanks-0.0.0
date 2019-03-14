using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamID : MonoBehaviour {

    [SerializeField] public int TeamIDNumber;
    [SerializeField] public FlagHolder CurrentFlagBase;

	
    public void AssignTeam(int value)
    {
        TeamIDNumber = value;
    }

    public void AssignFlagBase(FlagHolder BaseToAssign)
    {
        CurrentFlagBase = BaseToAssign;
    }
}
