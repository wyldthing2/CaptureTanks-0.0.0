using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerShootingNew : NetworkBehaviour
{
    [SerializeField] float gunRange = 1000f;
    [SerializeField] public float shotCooldown = .1f;
    [SerializeField] int killsToWin = 5;
    [SerializeField] public Transform firePosition;
    [SerializeField] ShotEffectsManager shotEffects;
    [SerializeField] int maxBlobKills = 20;
    [SerializeField] GameObject BulletPrefab;

    //[SerializeField] private BlobSpawnController commands;

    [SyncVar(hook = "OnScoreChanged")] float score;
    

    Player player;
    [SerializeField] float elapsedTime;
    bool canShoot;
    List<Collider> _hitList = new List<Collider>();

    void Start()
    {
        player = GetComponent<Player>();
        shotEffects.Initialize();

        if (isLocalPlayer)
            canShoot = true;
    }

    [ServerCallback]
    void OnEnable()
    {
        score = 0;


    }

    void Update()
    {
        if (!canShoot)
            return;

        elapsedTime += Time.deltaTime;

        if (Input.GetButton("Fire1") && elapsedTime > shotCooldown)
        {
            if (elapsedTime > shotCooldown)
            {
                FireShot();
                elapsedTime = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            
            CmdActivateObject(firePosition.position, firePosition.forward);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            DeactivateObject(firePosition.position,firePosition.forward);
        }
    }

    [Command]
    void CmdActivateObject(Vector3 origin, Vector3 direction)
    {
        RaycastHit hit;

        Ray ray = new Ray(origin, direction);

        bool result = Physics.Raycast(ray, out hit, gunRange);

        if (result)
        {
            Debug.Log("dumdum?");

            if (hit.transform.name == "BaseBuilder")
            {
                Debug.Log("dumdum");
                BaseBuilder thisBaseBuilder = hit.transform.GetComponent<BaseBuilder>();
                if (score >= (thisBaseBuilder.NextWallToBuild +1 )*20)
                {
                    score -= (thisBaseBuilder.NextWallToBuild + 1) * 20;
                    thisBaseBuilder.CmdActivateWalls();
                }
                else
                {
                    Debug.Log("Not enough power.");
                }
                
                //GameObject baseBuilderObject = thisBaseBuilder.gameObject;
                Debug.Log("dumdum!");
                //ActivateBaseBuilder(baseBuilderObject);
            }
        }
    }

    
    void DeactivateObject(Vector3 origin, Vector3 direction)
    {
        Debug.Log("The count is " + BlobSpawnController.commands.BlobCount);

        RaycastHit hit;

        Ray ray = new Ray(origin, direction);
        Debug.DrawRay(ray.origin, ray.direction * 3f, Color.red, 1f);

        bool result = Physics.Raycast(ray, out hit, gunRange);

        if (result)
        {
            if (hit.transform.name == "BaseBuilder")
            {
                GameObject hitObject =  hit.transform.gameObject;
            }
        }

    }

    
    //void ActivateBaseBuilder(GameObject clientBaseBuilder)
    //{
        //clientBaseBuilder.GetComponent<BaseBuilder>().CmdActivateWalls();
    //}

    void RpcDeactivate(GameObject clientObject)
    {
        if (!isLocalPlayer)
        {
            clientObject.SetActive(false);
        }
    }

    [Command]
    void CmdProcessHit(Vector3 origin, Quaternion rotation)
    {
        RpcSpawnBullet(origin, rotation);
    }

    //[Server]
    //void SpawnBullet(Vector3 origin, Quaternion rotation)
    //{
        //RpcSpawnBullet(origin, rotation);
    //}

    [ClientRpc]
    void RpcSpawnBullet(Vector3 origin, Quaternion rotation)
    {
        GameObject BulletFired = Instantiate(BulletPrefab, origin, rotation);
        NetworkServer.Spawn(BulletFired);
    }

    public void FireShot()
    {


            RaycastHit hit;


            Ray ray = new Ray(firePosition.position, firePosition.forward);
            Debug.DrawRay(ray.origin, ray.direction * 3f, Color.red, 1f);

            bool result = Physics.Raycast(ray, out hit, gunRange);

            CmdProcessHit(firePosition.position, firePosition.rotation);


        //RpcProcessShotEffects(result, hit.point);
    }

    [ClientRpc]
    void RpcProcessShotEffects(bool playImpact, Vector3 point)
    {
        shotEffects.PlayShotEffects();

        if (playImpact)
            shotEffects.PlayImpactEffect(point);
    }

    void OnScoreChanged(float value)
    {
        score = value;

        //float percent = score / maxBlobKills;

        if (isLocalPlayer)
        {
            PlayerCanvas.canvas.SetKills(value,maxBlobKills);
            PlayerCanvas.canvas.SetBlobKillBar(value/maxBlobKills);
        }
    }

    


}
