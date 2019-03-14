using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTest : MonoBehaviour {

    [SerializeField] GameObject ObjectToDestroy;


    private void OnCollisionEnter(Collision collis)
    {
        Debug.Log("Something Hits");
        if (collis.gameObject.tag == "Player")
        {
            Debug.Log("Player Hits");
            collis.gameObject.GetComponent<PlayerHealth>().TakeDamage();
        }
        else if (collis.gameObject.tag == "Destructable")
        {
            collis.transform.GetComponent<Health>().TakeDamage();
        }

        Destroy(ObjectToDestroy);
    }

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {




        

        
    }
}
