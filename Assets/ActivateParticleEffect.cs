using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateParticleEffect : MonoBehaviour {

    [SerializeField] public ParticleSystem BulletSmoke;

	// Use this for initialization
	void Start () {
        BulletSmoke.Play();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
