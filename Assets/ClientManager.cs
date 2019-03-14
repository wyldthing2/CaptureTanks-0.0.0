using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ClientManager : NetworkBehaviour {

    [SerializeField] public GameObject TopDownObject;
    [SerializeField] public Camera TopDownCamera;
    [SerializeField] public TargetSelector targetSelector;
    [SerializeField] bool TopDown = false;

    [SerializeField] public GameObject TopDownObjectProp
    {
        get
        {
            return TopDownObject;
        }
    }

    public static ClientManager ClientManagerCommands;

    private void Start()
    {
        Debug.Log(TopDownObject.transform.position);
        //TopDownCamera.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            
            
                TopDown = !TopDown;
                if (TopDown)
                {
                    TopDownObject.SetActive(true);
                    
                }
                else if (!TopDown)
                {
                    TopDownObject.SetActive(false);
                }
                
            
        }
    }

    public void SetTopDownObject(GameObject gameObject)
    {
        TopDownObject = gameObject;
    }


}
