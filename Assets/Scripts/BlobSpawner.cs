using UnityEngine;
using UnityEngine.Networking;

public class BlobSpawner : NetworkBehaviour
{

    //[SerializeField] private BlobSpawnController commands;
    //private BlobSpawnController commands;

    [SerializeField] GameObject blobPrefab;

    float elapsedTime;
    [SerializeField] public float numberPerSpawnAtGameStart;
    [SerializeField] int numberOfSpawnsAtGameStart;
    float i;
    float adjusti;
    float spawnCount = 0;



    PlayerHealth playerDied;

    [ServerCallback]
    void Start()
    {

        if (!BlobSpawnController.commands.SpawnerList.Contains(this))
            BlobSpawnController.commands.SpawnerList.Add(this);

        GameObject obj = Instantiate(blobPrefab, transform.position, transform.rotation);
        NetworkServer.Spawn(obj);
        spawnCount = numberOfSpawnsAtGameStart;
    }

    [Server]
    public void BlobSpawn(float numberToSpawn)
    {
        bool leftRight = true;

        for (i = 0; i <= numberToSpawn; i++)
        {
            if (leftRight)
            {
                Vector3 rowX = new Vector3(transform.position.x, transform.position.y, transform.position.z + i);
                GameObject obj = Instantiate(blobPrefab, rowX, transform.rotation);
                //float randsize = 2 * Random.value;
                //obj.transform.localScale += new Vector3(randsize, randsize, randsize);
                //obj.GetComponent<NetworkIdentity>().localPlayerAuthority = false;
                NetworkServer.Spawn(obj);
                BlobSpawnController.commands.BlobCount++;
                
            }
            else
            {
                adjusti = (i - 1) * -1;
                Vector3 rowX = new Vector3(transform.position.x, transform.position.y, transform.position.z + adjusti);
                GameObject obj = Instantiate(blobPrefab, rowX, transform.rotation);
                //obj.GetComponent<NetworkIdentity>().localPlayerAuthority = false;
                NetworkServer.Spawn(obj);
                BlobSpawnController.commands.BlobCount++;

            }
            leftRight = !leftRight;
        }
    }

    [ClientRpc]
    public void RpcSingleBlobSpawn()
    {
        GameObject obj = Instantiate(blobPrefab, transform.position, transform.rotation);
        //float randsize = 2 * Random.value;
        //obj.transform.localScale += new Vector3(randsize, randsize, randsize);
        //obj.GetComponent<NetworkIdentity>().localPlayerAuthority = false;
        NetworkServer.Spawn(obj);
    }
    

    void Update()
    {

        elapsedTime += Time.deltaTime;

        

        if (elapsedTime >= .5f && spawnCount >= 0)
        {
            spawnCount -= 1;
            elapsedTime = 0f;

            BlobSpawn(numberPerSpawnAtGameStart);

            
           

        }


        if (playerDied)
        {

        }

        
    }
}
