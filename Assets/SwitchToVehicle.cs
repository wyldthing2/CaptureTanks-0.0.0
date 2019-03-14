using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SwitchToVehicle : NetworkBehaviour {

    [SerializeField] public float range = 5f;
    [SerializeField] Transform firePosition;
    [SerializeField] ToggleEvent toggleLocal;
    [SerializeField] ToggleEvent toggleRemote;


    // Use this for initialization
    void Start () {
		
        

	}


    void EnterVehicle()
    {

        //disable controller. If it's the local player, disable the local. If remote, disable their controller


        

        if (isLocalPlayer)
            toggleLocal.Invoke(false);
        
    }

    /*
    void ExitVehicle ()
    {
        if (isLocalPlayer)
        {
            PlayerCanvas.canvas.Initialize();
            mainCamera.SetActive(false);
        }

        onToggleShared.Invoke(true);

        if (isLocalPlayer)
            onToggleLocal.Invoke(true);
        else
            onToggleRemote.Invoke(true);
    }
    */

    // Update is called once per frame
    void FixedUpdate () {

        if (Input.GetButtonDown("Fire1"))
        {
            
            CmdEnterVehicle(firePosition.position, firePosition.forward);


        }

    }

    [Command]
    void CmdEnterVehicle(Vector3 origin, Vector3 direction)
    {

        

        RaycastHit hit;




        Ray ray = new Ray(origin, direction);
        Debug.DrawRay(ray.origin, ray.direction * 3f, Color.red, 1f);

        bool result = Physics.Raycast(ray, out hit, range);

        if (result)
        {
            TestCar vehicle = hit.transform.GetComponent<TestCar>();
            



            if (hit.transform.tag == "Vehicle")
            {
                //Destroy(hit.transform.gameObject);
                vehicle.enabled = true;

                EnterVehicle();
                this.gameObject.transform.parent = vehicle.gameObject.transform.Find("DriverSeat");
                //this.gameObject.transform. = new Vector3(0, 0, 0);
                this.gameObject.transform.position = new Vector3(0, 0, 0);
                this.gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);

                





            }


            
        }

        
    }
}
