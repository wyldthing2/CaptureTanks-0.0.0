using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.AI;

public class WallHealth : NetworkBehaviour
{
    [SerializeField] public int maxHealth = 30; //consider SC2 health

    [SerializeField] float NoTagBackTime = 5f;
    [SerializeField] float elapsedTimeNoTagBack;
    
    [SerializeField] ToggleEvent onToggleWallComponents;

    
    //[SerializeField] Collider colliderComponent;
    //[SerializeField] Collider triggerComponent;
    //[SerializeField] NavMeshObstacle NavmeshObstacleComponent;
    //[SerializeField] MeshRenderer MeshRendererComponent;
    //[SerializeField] GameObject Repeller;


    //Only server can set value of SyncVar
    [SerializeField][SyncVar(hook = "OnHealthChanged1")] public int health;


    
    [ServerCallback]
    void OnEnable()
    {
        health = maxHealth;
    }

    [ServerCallback]
    void Start()
    {
        health = maxHealth;
    }

    

    private void OnTriggerStay (Collider collis)
    {
        if (collis.gameObject.tag == "Blob")
        {
            Debug.Log("Blobbed");
            

            this.gameObject.GetComponent<Collider>().enabled = false;
            Debug.Log("Collider turned off");
        

            elapsedTimeNoTagBack = 0;
            CmdTakeDamage1(1);
            Debug.Log("Finished collision");
        }
    }

    float countDown;

    private void Update()
    {
        if (elapsedTimeNoTagBack <= NoTagBackTime)
        {
            countDown = NoTagBackTime - elapsedTimeNoTagBack;

        }
        else
        {
            this.gameObject.GetComponent<Collider>().enabled = true;
        }

        elapsedTimeNoTagBack += Time.deltaTime;

        
    }

    bool died = false;

    [Server]
    public void CmdTakeDamage1(int damageAmount)
    {
        
        health -= damageAmount;

        if (health <= 0 && died == false)
        {
            BlobSpawnController.commands.CommandSpawnHere(this.transform, maxHealth/8);
            RpcDeactivateWalls();
            died = true;
        }
    }


    

    [ClientRpc]
    void RpcDeactivateWalls()
    {
        Debug.Log("Rpc take damage");
        Debug.Log("Wall Died");
        onToggleWallComponents.Invoke(false);
    }
    

    void OnHealthChanged1(int value)
    {
        health = value;
        Debug.Log(health);
        
    }

    
    public void ResetWall()
    {
        RpcResetWall();
    }

    [ClientRpc]
    public void RpcResetWall()
    {
        onToggleWallComponents.Invoke(true);
        health = maxHealth;
        died = false;
    }
}
