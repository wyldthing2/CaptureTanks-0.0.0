using UnityEngine;
using System.Collections.Generic;
using RTS_Cam;
using UnityEngine.AI;

[RequireComponent(typeof(RTS_Camera))]
public class TargetSelector : MonoBehaviour 
{
    public static TargetSelector TargetSelectorCommands;

    public GameObject TargetPlayer;

    private RTS_Camera cam;
    public new Camera camera;
    public string targetsTag;
    private bool TargetingPlayer = true;

    private void Start()
    {
        cam = gameObject.GetComponent<RTS_Camera>();
        camera = gameObject.GetComponent<Camera>();
    }

    [SerializeField] NavMeshAgent agent;
    public Vector3 targetPosition;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse Clicked");
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag(targetsTag))
                {
                    cam.SetTarget(hit.transform);
                    Debug.Log("You clicked a blob");
                }
                else
                {
                    Debug.Log("Targeting Mesh");
                    //NavMeshHit nHit;
                    //Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);

                    //bool hasHit = NavMesh.Raycast(this.transform.position, hit.point, out nHit, NavMesh.AllAreas);
                    Debug.Log("Mesh Targeted");
                    Debug.Log(hit.point);
                    //randomDestination(10);

                    //Debug.Log("Area ID: " + nHit.mask);
                    //targetPosition = hit.point;
                    //agent.destination = targetPosition;



                    cam.ResetTarget();
                }
                    
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TargetingPlayer = !TargetingPlayer;
            TargetPlayer = PlayerCanvas.canvas.PlayerObject;
            if (!TargetingPlayer)
            {
                cam.SetTarget(TargetPlayer.transform);
            }
            else if (TargetingPlayer)
            {
                cam.ResetTarget();
            }
        }
    }

    void randomDestination(float radiusToChooseFrom)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radiusToChooseFrom;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, radiusToChooseFrom, 1);
        Vector3 finalPosition = hit.position;
        agent.destination = finalPosition;
        Debug.Log(finalPosition);
        //agent.destination = finalPosition;
        //Debug.Log("Blob chose random spot:" + finalPosition);
        
    }

}
