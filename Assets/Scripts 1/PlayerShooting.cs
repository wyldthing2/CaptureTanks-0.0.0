using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour {

	public ParticleSystem muzzleFlash;
	Animator anim;
	public GameObject impactPrefab;

    //[SerializeField] Text echoText;

    GameObject[] impacts;
	int currentImpact = 0;
	int maxImpacts = 5; 

	bool shooting = false;
    //bool RTSview = false;

	// Use this for initialization
	void Start () {

		impacts = new GameObject[maxImpacts];
		for (int i = 0; i < maxImpacts; i++)
			impacts [i] = (GameObject)Instantiate (impactPrefab);

		anim = GetComponentInChildren<Animator> ();
		
	}
	
	 //Update is called once per frame
	void Update () {

		if (Input.GetButtonDown ("Fire1")) {
			muzzleFlash.Play ();
			anim.SetTrigger ("Fire");
			shooting = true;
		}

        /*if (Input.GetButton("Fire2")) {
           echoText.text = "E Click detected.";

        }*/
		
	}

	void FixedUpdate()
	{
		if (shooting) 
		{
			shooting = false;

			RaycastHit hit;
			if(Physics.Raycast(transform.position, transform.forward, out hit, 1000f))
				{

				if (hit.transform.tag == "Blob")
					Destroy (hit.transform.gameObject);

				impacts[currentImpact].transform.position = hit.point;
				impacts[currentImpact].GetComponent<ParticleSystem>().Play();

				if(++currentImpact >= maxImpacts)
					{
						currentImpact = 0;

					}

				}
		}

	}

}
