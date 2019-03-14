using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TurretAI : NetworkBehaviour {

    public GameObject targetObject;
    [SerializeField] public float TurnSpeed = 2;
    [SerializeField] Transform firePosition;
    [SerializeField] Transform firePosition2;
    [SerializeField] TeamID ThisTeamID;
    float elapsedTime;
    [SerializeField] float fireRate = 30;
    bool left = true;

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (targetObject == null)
            return;

        Aim(targetObject.transform);

        if (elapsedTime >= fireRate)
        {
            if (left)
            {
                RaycastHit hit;

                Ray ray = new Ray(firePosition.position, firePosition.forward);
                Debug.DrawRay(firePosition.position, firePosition.forward*50, Color.yellow , 2);

                bool result = Physics.Raycast(ray, out hit, 50);
                if (result)
                {
                    if (hit.transform.gameObject.tag == "Player" || hit.transform.gameObject.tag == "Destructable")
                    {

                        if (hit.transform.gameObject.GetComponent<TeamID>().TeamIDNumber != ThisTeamID.TeamIDNumber/*hit.transform.gameObject.tag == "Player" || hit.transform.gameObject.tag == "Destructable"*/)
                        {
                            FireShot(firePosition.position, firePosition.rotation);
                            left = !left;
                        }
                    }
                }
            }
            else
            {
                RaycastHit hit;

                Ray ray = new Ray(firePosition2.position, firePosition2.forward);

                bool result = Physics.Raycast(ray, out hit, 5);
                if (result)
                {
                    if (hit.transform.gameObject.tag == "Player" || hit.transform.gameObject.tag == "Destructable")
                    {

                        if (hit.transform.gameObject.GetComponent<TeamID>().TeamIDNumber != ThisTeamID.TeamIDNumber/*hit.transform.gameObject.tag == "Player" || hit.transform.gameObject.tag == "Destructable"*/)
                        {
                            FireShot(firePosition.position, firePosition.rotation);
                            left = !left;
                        }
                    }
                }

            }

            elapsedTime = 0;
        }

    }

    private void FixedUpdate()
    {
        if (targetObject == null)
            return;
        if (Time.time % 2 == 0)
        {
            
        }
    }

    //[Server]
    public void Aim(Transform target)
    {
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(this.gameObject.transform.rotation, lookRotation, Time.deltaTime * TurnSpeed).eulerAngles;
        this.gameObject.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    public void NotifyOfDeath(GameObject DeadObject)
    {
        if (DeadObject == targetObject)
        {    
            targetObject = null;
        }
    }

    void FireShot(Vector3 origin, Quaternion rotation)
    {

        Instantiate(Resources.Load("Bullet"), origin, rotation);

    }

    //[ClientRpc]
    void turnToThisDestination()
    {
        //Gunsync code
    }

    void PrioritizeTarget()
    {

    }

    bool BlockedByWallCkeck()
    {
        return false;
    }

    void Fire()
    {

    }

    
}
