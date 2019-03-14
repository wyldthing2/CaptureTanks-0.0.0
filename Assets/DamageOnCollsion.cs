using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DamageOnCollsion : NetworkBehaviour {

	[SerializeField] public int DamageAmount;

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != this.gameObject)
        {
            //Debug.Log("Hit a thing!");
            PlayerHealth healthScriptToDamage = other.gameObject.GetComponent<PlayerHealth>();
            MovementAI AIToAlert = other.gameObject.GetComponent<MovementAI>();

            //Debug.Log(other.gameObject.name);

            if (healthScriptToDamage != null)
            {
                //Debug.Log("Got a script!");
                healthScriptToDamage.TakeDamage();
                if (AIToAlert != null)
                {
                    AIToAlert.TurnToCheckASpot(this.gameObject.transform.position - this.gameObject.transform.forward);
                }
            }
            else if (other.gameObject.tag == "Destructable")
            {
                Health enemyHealth = other.transform.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    //Debug.Log("It will be hurt.");
                    enemyHealth.TakeDamage();
                }
            }
            RpcDestroyBullet();
            NetworkServer.UnSpawn(this.gameObject);
            NetworkServer.Destroy(this.gameObject);
        }

    }

    [ClientRpc]
    void RpcDestroyBullet()
    {
        Debug.Log("Client gets the command.");
        Destroy(this.gameObject);
    }

    // Careful when setting this to true - it might cause double
    // events to be fired - but it won't pass through the trigger
    public bool sendTriggerMessage = false;

    public LayerMask layerMask = -1; //make sure we aren't in this layer 
    public float skinWidth = 0.1f; //probably doesn't need to be changed 

    private float minimumExtent;
    private float partialExtent;
    private float sqrMinimumExtent;
    private Vector3 previousPosition;
    //private Rigidbody myRigidbody;
    private Collider myCollider;

    //initialize values 
    void Start()
    {
        //myRigidbody = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
        previousPosition = this.gameObject.transform.position;
        minimumExtent = Mathf.Min(Mathf.Min(myCollider.bounds.extents.x, myCollider.bounds.extents.y), myCollider.bounds.extents.z);
        partialExtent = minimumExtent * (1.0f - skinWidth);
        sqrMinimumExtent = minimumExtent * minimumExtent;
    }

    void FixedUpdate()
    {
        //have we moved more than our minimum extent? 
        Vector3 movementThisStep = this.gameObject.transform.position - previousPosition;
        float movementSqrMagnitude = movementThisStep.sqrMagnitude;

        if (movementSqrMagnitude > sqrMinimumExtent)
        {
            float movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);
            RaycastHit hitInfo;

            //check for obstructions we might have missed 
            if (Physics.Raycast(previousPosition, movementThisStep, out hitInfo, movementMagnitude, layerMask.value))
            {
                if (!hitInfo.collider)
                    return;

                if (hitInfo.collider.isTrigger)
                    hitInfo.collider.SendMessage("OnTriggerEnter", myCollider);

                if (!hitInfo.collider.isTrigger)
                    this.gameObject.transform.position = hitInfo.point - (movementThisStep / movementMagnitude) * partialExtent;

            }
        }

        previousPosition = this.gameObject.transform.position;
    }



}
