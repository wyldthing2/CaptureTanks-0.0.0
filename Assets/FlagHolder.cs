using System.Collections;
using System;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FlagHolder : NetworkBehaviour {

    [SerializeField] public List<List<GameObject>> ListOfLists = new List<List<GameObject>>();
    [SerializeField] [SyncVar(hook = "OnFlagsCountChange")] public int FlagsCount = 0;
    [SerializeField] public MovementAI ThisMovementAI;
    

    [SerializeField] public bool HolderIsAPlayer;
    [SerializeField] public int NumberOfTeams = 4;
    [SerializeField] public static int NumberOfFlagsPerTeam = 2;
    [SerializeField] public static int FlagsFromEachTeamToWin = 2;
    [SerializeField] public int ThisHolderTeamID;
    [SerializeField] public List<Material> MaterialsList = new List<Material>();

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collided with Pad");
        if (other.tag == "Player")
        {
            if (!HolderIsAPlayer)
            {

                PullFlag(other.gameObject);

                GiveFlag(other.gameObject);

            }
        }
    }



    void OnFlagsCountChange(int value)
    {
        FlagsCount = value;
    }

    private void Start()
    {
        ThisHolderTeamID = this.gameObject.GetComponent<TeamID>().TeamIDNumber;

        for (int i = 0; i < NumberOfTeams; i++)
        {
            ListOfLists.Add(new List<GameObject>());
        }

        if (!HolderIsAPlayer && ThisHolderTeamID < 5)
        {
            SpawnFlags(ThisHolderTeamID);
        }
    }

    [SerializeField] GameObject FlagPosition;
    [SerializeField] GameObject FlagPrefab;

    void SpawnFlags(int TeamID)
    {
        if (TeamID < 5)
        {
            if (TeamID % 2 != 0)
            {
                for (int i = 0; i < NumberOfFlagsPerTeam; i++)
                {
                    ListOfLists[TeamID].Add(Instantiate(FlagPrefab, transform.TransformPoint(new Vector3((float)(FlagPosition.transform.localPosition.x - (((float)TeamID - 1) / 16 + .0625)), FlagPosition.transform.localPosition.y, FlagPosition.transform.localPosition.z + ((float)i / 8))), FlagPrefab.transform.rotation));
                    TeamID FlagScript = ListOfLists[TeamID][i].GetComponent<TeamID>();
                    ListOfLists[TeamID][i].GetComponent<MeshRenderer>().material = MaterialsList[TeamID];
                    FlagScript.CurrentFlagBase = this;
                    FlagScript.TeamIDNumber = TeamID;
                    ListOfLists[TeamID][i].transform.SetParent(this.gameObject.transform);
                    FlagsCount++;
                }
            }
            else
            {
                for (int i = 0; i < NumberOfFlagsPerTeam; i++)
                {
                    ListOfLists[TeamID].Add(Instantiate(FlagPrefab, transform.TransformPoint(new Vector3((float)(FlagPosition.transform.localPosition.x + ((float)TeamID / 16 + .0625)), FlagPosition.transform.localPosition.y, FlagPosition.transform.localPosition.z + ((float)i / 8))), FlagPrefab.transform.rotation));
                    TeamID FlagScript = ListOfLists[TeamID][i].GetComponent<TeamID>();
                    ListOfLists[TeamID][i].GetComponent<MeshRenderer>().material = MaterialsList[TeamID];
                    FlagScript.CurrentFlagBase = this;
                    FlagScript.TeamIDNumber = TeamID;
                    ListOfLists[TeamID][i].transform.SetParent(this.gameObject.transform);
                    FlagsCount++;
                }
            }
        }
        

    }

    [SerializeField] GameObject DroppedFlagHolderPrefab;


    public void DropFlags()
    {
        if (HolderIsAPlayer)
        {
            //drop on ground
            //GameObject DroppedFlags = Instantiate((GameObject)Resources.Load("DroppedFlagHolder"), this.transform);
            GameObject DroppedFlags = Instantiate(DroppedFlagHolderPrefab, this.transform.position, this.transform.rotation);
            DroppedFlags.GetComponent<FlagHolder>().PullFlagForDrop(this.gameObject);
            //invoke return flag to home base
            DroppedFlags.GetComponent<FlagHolder>().ReturnFlagsToTheirHomes();
            //DroppedFlags.GetComponent<FlagHolder>().Invoke("ReturnFlagsToTheirHomes", 20f);
        }
    }

    public void ReturnFlagsToTheirHomes()
    {
        for (int i = 0; i < NumberOfTeams; i++)
        {

                if (ListOfLists[i] != null && ListOfLists[i].Count > 0)
                {
                    FlagHolder flagHomeHolder = ListOfLists[i][ListOfLists[i].Count - 1].GetComponent<TeamID>().CurrentFlagBase;
                    flagHomeHolder.ListOfLists[i].Add(ListOfLists[i][ListOfLists[i].Count - 1]);
                    ListOfLists[i][ListOfLists[i].Count - 1].SetActive(false);
                    ListOfLists[i].Remove(ListOfLists[i][ListOfLists[i].Count - 1]);
                    flagHomeHolder.ListOfLists[i][flagHomeHolder.ListOfLists[i].Count - 1].transform.SetParent(flagHomeHolder.transform);
                    if (i % 2 != 0)
                    {
                        flagHomeHolder.ListOfLists[i][flagHomeHolder.ListOfLists[i].Count - 1].transform.localPosition = new Vector3((float)(flagHomeHolder.FlagPosition.transform.localPosition.x - (((float)i - 1) / 16 + .0625)), flagHomeHolder.FlagPosition.transform.localPosition.y, flagHomeHolder.FlagPosition.transform.localPosition.z + (((float)flagHomeHolder.ListOfLists[i].Count - 1) / 8));
                    }
                    else
                    {
                    flagHomeHolder.ListOfLists[i][flagHomeHolder.ListOfLists[i].Count - 1].transform.localPosition = new Vector3((float)(flagHomeHolder.FlagPosition.transform.localPosition.x + ((float)i / 16 + .0625)), flagHomeHolder.FlagPosition.transform.localPosition.y, flagHomeHolder.FlagPosition.transform.localPosition.z + (((float)flagHomeHolder.ListOfLists[i].Count - 1) / 8));
                    }
                flagHomeHolder.ListOfLists[i][flagHomeHolder.ListOfLists[i].Count - 1].transform.rotation = flagHomeHolder.transform.rotation;
                flagHomeHolder.ListOfLists[i][flagHomeHolder.ListOfLists[i].Count - 1].SetActive(true);
                
                    
                }
                //Debug.Log("Given");
            Destroy(this.gameObject);
            
        }
    }

    [SerializeField] public GameObject ObjectToPullFrom;
    [SerializeField] private FlagHolder FlagHolderToGiveTo;

    
    public void PullFlag(GameObject givingObject)
    {
        if (givingObject != null)
        {
            if (!HolderIsAPlayer && givingObject.GetComponent<TeamID>().TeamIDNumber == ThisHolderTeamID)
            {
                FlagHolder givingFlagHolder = givingObject.GetComponent<FlagHolder>();
                //Debug.Log("We're the same team");
                if (givingFlagHolder != null)
                {
                    //Debug.Log("Got the holder to pull");
                    int VictoryCheck = 0;
                    for (int i = 0; i < NumberOfTeams; i++)
                    {

                        if (givingFlagHolder.ListOfLists[i] != null && givingFlagHolder.ListOfLists[i].Count > 0)
                        {
                            //Debug.Log("There's a flag here");
                            if (ListOfLists[i].Count < NumberOfFlagsPerTeam)
                            {

                                givingFlagHolder.ListOfLists[i][givingFlagHolder.ListOfLists[i].Count - 1].SetActive(false);
                                ListOfLists[i].Add(givingFlagHolder.ListOfLists[i][givingFlagHolder.ListOfLists[i].Count - 1]);
                                FlagsCount++;
                                givingFlagHolder.ListOfLists[i][givingFlagHolder.ListOfLists[i].Count - 1].GetComponent<TeamID>().CurrentFlagBase = this;
                                givingFlagHolder.ListOfLists[i].Remove(givingFlagHolder.ListOfLists[i][givingFlagHolder.ListOfLists[i].Count - 1]);
                                givingFlagHolder.FlagsCount--;
                                if (i % 2 != 0)
                                {
                                    ListOfLists[i][ListOfLists[i].Count - 1].transform.position = transform.TransformPoint(new Vector3((float)(FlagPosition.transform.localPosition.x - (((float)i - 1) / 16 + .0625)), FlagPosition.transform.localPosition.y, FlagPosition.transform.localPosition.z + (((float)ListOfLists[i].Count - 1) / 8)));
                                }
                                else
                                {
                                    ListOfLists[i][ListOfLists[i].Count - 1].transform.position = transform.TransformPoint(new Vector3((float)(FlagPosition.transform.localPosition.x + ((float)i / 16 + .0625)), FlagPosition.transform.localPosition.y, FlagPosition.transform.localPosition.z + (((float)ListOfLists[i].Count - 1) / 8)));
                                }
                                ListOfLists[i][ListOfLists[i].Count - 1].transform.SetParent(this.transform);
                                ListOfLists[i][ListOfLists[i].Count - 1].transform.rotation = this.transform.rotation;
                                ListOfLists[i][ListOfLists[i].Count - 1].SetActive(true);
                                if (givingFlagHolder.GetComponent<Player>().isAI)
                                {
                                    givingFlagHolder.ThisMovementAI.PickABaseToAttack();
                                }
                                //Debug.Log("Pulled the flag");
                            }
                        }
                        if (ListOfLists[i].Count >= FlagsFromEachTeamToWin)
                        {
                            VictoryCheck++;
                            //Debug.Log(VictoryCheck + " Victory points");
                            if (VictoryCheck == 4)
                            {
                                givingObject.GetComponent<Player>().Won();
                            }
                        }
                    }
                }
            }
        }
    }

    public void GiveFlag(GameObject recievingObject)
    {
        //if not same team

        if (!HolderIsAPlayer && recievingObject.GetComponent<TeamID>().TeamIDNumber != ThisHolderTeamID)
        {
            FlagHolder recievingFlagHolder = recievingObject.GetComponent<FlagHolder>();
            if (recievingFlagHolder != null)
            {
                //Debug.Log("Got the holder");
                for (int i = 0; i < NumberOfTeams; i++)
                {
                    if (recievingFlagHolder.ListOfLists[i].Count == 0)
                    {
                        if (ListOfLists[i].Count > 0)
                        {
                            ListOfLists[i][ListOfLists[i].Count - 1].SetActive(false);
                            recievingFlagHolder.ListOfLists[i].Add(ListOfLists[i][ListOfLists[i].Count - 1]);
                            recievingFlagHolder.FlagsCount++;
                            ListOfLists[i].Remove(ListOfLists[i][ListOfLists[i].Count - 1]);
                            FlagsCount--;
                            recievingFlagHolder.ListOfLists[i][recievingFlagHolder.ListOfLists[i].Count - 1].transform.SetParent(recievingObject.transform);
                            if (i % 2 != 0)
                            {
                                recievingFlagHolder.ListOfLists[i][recievingFlagHolder.ListOfLists[i].Count - 1].transform.localPosition = new Vector3((float)(recievingFlagHolder.FlagPosition.transform.localPosition.x - (((float)i - 1) / 8 + .125)), recievingFlagHolder.FlagPosition.transform.localPosition.y, recievingFlagHolder.FlagPosition.transform.localPosition.z);
                            }
                            else
                            {
                                recievingFlagHolder.ListOfLists[i][recievingFlagHolder.ListOfLists[i].Count - 1].transform.localPosition = new Vector3((float)(recievingFlagHolder.FlagPosition.transform.localPosition.x + ((float)i / 8 + .125)), recievingFlagHolder.FlagPosition.transform.localPosition.y, recievingFlagHolder.FlagPosition.transform.localPosition.z);
                            }
                            recievingFlagHolder.ListOfLists[i][recievingFlagHolder.ListOfLists[i].Count - 1].transform.rotation = recievingObject.transform.rotation;
                            recievingFlagHolder.ListOfLists[i][recievingFlagHolder.ListOfLists[i].Count - 1].SetActive(true);
                            if (recievingFlagHolder.GetComponent<Player>().isAI)
                            {
                                recievingFlagHolder.ThisMovementAI.ReturnToBaseWithFlag();
                            }
                        }
                        //Debug.Log("Given");
                    }
                }
            }
        }
    }

    public void PullFlagForDrop(GameObject givingObject)
    {

        for (int i = 0; i < NumberOfTeams; i++)
        {
            ListOfLists.Add(new List<GameObject>());
        }
        if (!HolderIsAPlayer)
        {
            FlagHolder givingFlagHolder = givingObject.GetComponent<FlagHolder>();
            //Debug.Log("We're the same team");
            if (givingFlagHolder != null)
            {
                //Debug.Log("Got the holder to pull");
                for (int i = 0; i < NumberOfTeams; i++)
                {

                    if (givingFlagHolder.ListOfLists[i] != null && givingFlagHolder.ListOfLists[i].Count > 0)
                    {
                        //Debug.Log("There's a flag here");
                        if (ListOfLists[i].Count < NumberOfFlagsPerTeam)
                        {

                            givingFlagHolder.ListOfLists[i][givingFlagHolder.ListOfLists[i].Count - 1].SetActive(false);
                            ListOfLists[i].Add(givingFlagHolder.ListOfLists[i][givingFlagHolder.ListOfLists[i].Count - 1]);
                            givingFlagHolder.ListOfLists[i].Remove(givingFlagHolder.ListOfLists[i][givingFlagHolder.ListOfLists[i].Count - 1]);

                            if (i % 2 != 0)
                            {
                                ListOfLists[i][ListOfLists[i].Count - 1].transform.position = transform.TransformPoint(new Vector3((float)(FlagPosition.transform.localPosition.x - (((float)i - 1) / 16 + .0625)), FlagPosition.transform.localPosition.y, FlagPosition.transform.localPosition.z + (((float)ListOfLists[i].Count - 1) / 8)));
                            }
                            else
                            {
                                ListOfLists[i][ListOfLists[i].Count - 1].transform.position = transform.TransformPoint(new Vector3((float)(FlagPosition.transform.localPosition.x + ((float)i / 16 + .0625)), FlagPosition.transform.localPosition.y, FlagPosition.transform.localPosition.z + (((float)ListOfLists[i].Count - 1) / 8)));
                            }
                            ListOfLists[i][ListOfLists[i].Count - 1].transform.SetParent(this.transform);
                            ListOfLists[i][ListOfLists[i].Count - 1].transform.rotation = this.transform.rotation;
                            ListOfLists[i][ListOfLists[i].Count - 1].SetActive(true);
                            //Debug.Log("Pulled the flag");
                        }
                    }
                }
            }
        }
    }

}
