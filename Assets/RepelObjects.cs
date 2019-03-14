using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RepelObjects : MonoBehaviour {

    GameObject repeller;

    private void Start()
    {
       repeller = this.gameObject;
    }


    private void OnTriggerEnter(Collider c)
    {
        Debug.Log("Repelled");
        NavMeshAgent cAgent = c.gameObject.GetComponent<NavMeshAgent>();
        //cAgent.velocity = cAgent.velocity * -1;
        //cAgent.transform.position += new Vector3(0,0,-10);
        //cAgent.transform.position = repeller.transform.position + new Vector3(10f, 0f, 0f);
        cAgent.transform.position = Vector3.Lerp(cAgent.transform.position, repeller.transform.position + new Vector3(10, 0, 0), 1);
    }

    /*private void OnCollisionEnter(Collision c)
    {
        Debug.Log("Repelling");
        //GameObject repellee = c.gameObject;
        Vector3 collisionNormal = c.contacts[0].normal;
        c.gameObject.transform.position += collisionNormal.normalized * 1;
    }
    */
}
