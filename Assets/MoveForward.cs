using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour {

    [SerializeField] public float ProjectileSpeed = 1f;
    //float elapsedTime = 0f;

	void Update () {
        //elapsedTime += Time.deltaTime;
        this.gameObject.transform.Translate(Vector3.forward * Time.deltaTime * ProjectileSpeed);
	}
}
