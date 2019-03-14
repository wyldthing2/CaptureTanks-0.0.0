using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class TopDownController : NetworkBehaviour {

    [SerializeField] private new Camera camera;
    private Vector3 targetPosition;
    [SerializeField] public NavMeshAgent agent;


    void Start()
    {
        //if (isLocalPlayer)
        //TargetSelector.TargetSelectorCommands.TargetPlayer = this.transform.gameObject;
        //camera = ClientManager.ClientManagerCommands.TopDownCamera;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //camera = TargetSelector.TargetSelectorCommands.camera;
            Debug.Log("Mouse Clicked");
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                
                Debug.Log("Targeting Mesh");
                    NavMeshHit nHit;

                    bool hasHit = NavMesh.Raycast(this.transform.position, hit.point, out nHit, NavMesh.AllAreas);
                    Debug.Log("Mesh Targeted");
                    Debug.Log(hit.point);
                    //randomDestination(10);

                    //Debug.Log("Area ID: " + nHit.mask);
                    targetPosition = hit.point;
                    agent.destination = targetPosition;
                Debug.Log("NavDestination Set");



                    //cam.ResetTarget();
                

            }
        }

        
    }

    
}
