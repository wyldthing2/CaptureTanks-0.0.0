using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

public class BaseSpawn : NetworkBehaviour {

    [SerializeField] public static List<GameObject> TeamList = new List<GameObject>();
    [SerializeField] public static int TeamCount = 0;

	void Awake () {


        AddToTeamList();
		
	}

    [ServerCallback]
    public void AddToTeamList()
    {
        if (!TeamList.Contains(this.gameObject))
            TeamList.Add(this.gameObject);
    }
	
}
