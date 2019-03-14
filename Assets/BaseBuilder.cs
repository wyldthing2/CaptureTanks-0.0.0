using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BaseBuilder : NetworkBehaviour {

    [SerializeField] public List<GameObject> BuilderModel = new List<GameObject>();
    
    [System.Serializable] public class wallLevels
    {
        public GameObject Level;
        public List<GameObject> walls = new List<GameObject>();
    }
    public List<wallLevels> wallLevelGroup = new List<wallLevels>();
    //[SerializeField] public List<GameObject> walls = new List<GameObject>();
    //[SyncVar(hook = "OnWallActiveChange")] public bool WallActive;
    public int NextWallToBuild = 0;
    private int _maxWallsToBuild = 3;

    private void Update()
    {
        //if (Time.time > 30)
        //{
        //RpcActivateWalls();
        //}

        
    }

    


    private void Start()
    {
        //FillWallFields();
        RpcDeactivateWallLevels();

    }

    [Command]
    public void CmdActivateWalls()
    {
        RpcActivateWalls();
    }

    [ClientRpc]
    public void RpcActivateWalls()
    {
        
        if (NextWallToBuild < _maxWallsToBuild )
        {
            wallLevelGroup[NextWallToBuild].Level.SetActive(true);
            BuilderModel[NextWallToBuild].SetActive(true);
            NextWallToBuild++;
        }
        else
        {
            Debug.Log("All Walls Already Built");
        }
    }

    [Command]
    public void CmdResetWalls()
    {
        RpcResetWalls();
        Debug.Log("reset walls");
    }

    
    public void FillWallFields()
    {
        foreach (wallLevels wallLevel in wallLevelGroup)
        {
            
                Debug.Log(wallLevel.Level.transform.childCount);
                for (int i = 0; i < wallLevel.Level.transform.childCount; i++)
                {
                    Debug.Log("Working on Wall " + i);
                    GameObject currentWall = wallLevel.Level.transform.GetChild(i).gameObject;
                    if (!wallLevel.walls.Contains(currentWall))
                    {
                        wallLevel.walls.Add(currentWall);
                    }
                    
                    //Why no blob spawn?
                    //wallLevel.Level.transform.GetChild(i).gameObject.GetComponent<WallHealth>().ResetWall();
                    //BuilderModel[i].SetActive(false);

                }
            
        }
    }


    [ClientRpc]
    public void RpcResetWalls()
    {
        Debug.Log("reset walls rpc");
        NextWallToBuild = 0;
        //foreach deactivate
        int c = 0;
        foreach (wallLevels wallLevel in wallLevelGroup)
        {
            if (wallLevel.Level.activeInHierarchy)
            {
                Debug.Log(wallLevel.Level.transform.childCount);
                for (int i = 0; i < wallLevel.walls.Count; i++ )
                {
                    Debug.Log("Working on Wall " + i);
                    WallHealth wallScript = wallLevel.walls[i].GetComponent<WallHealth>();
                    
                    wallScript.CmdTakeDamage1(wallScript.maxHealth);
                    //Why no blob spawn?
                    wallScript.ResetWall();
                }
                BuilderModel[c].SetActive(false);
                c++;
                wallLevel.Level.SetActive(false);
                Debug.Log("Walls turned off");
                
            }
        }
    }

    [ClientRpc]
    public void RpcDeactivateWallLevels()
    {
        int c = 0;
        foreach (wallLevels wallLevel in wallLevelGroup)
        {
            if (wallLevel.Level.activeInHierarchy)
            {
                Debug.Log(wallLevel.Level.transform.childCount);
                for (int i = 0; i < wallLevel.walls.Count; i++)
                {
                    Debug.Log("Working on Wall " + i);
                    WallHealth wallScript = wallLevel.walls[i].GetComponent<WallHealth>();

                    //Why no blob spawn?
                    wallScript.ResetWall();
                }
                c++;
                wallLevel.Level.SetActive(false);
                Debug.Log("Walls turned off");

            }
        }
    }

    
    
}
