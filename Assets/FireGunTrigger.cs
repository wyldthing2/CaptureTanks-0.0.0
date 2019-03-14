using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FireGunTrigger : NetworkBehaviour {

    [SerializeField] PlayerShootingNew ThisPlayerShooting;
    [SerializeField] TankDetectEnemyTrigger ThisTankDetectEnemyTrigger;

    [SerializeField] float elapsedTime = 0;

    private void Update()
    {
        elapsedTime += Time.deltaTime;
    }

    [Server]
    public void OnTriggerStay(Collider other)
    {
        if (ThisTankDetectEnemyTrigger.EnemyGameObjectList.Contains(other.gameObject))
        {
            if (elapsedTime > ThisPlayerShooting.shotCooldown)
            {
                RaycastHit hit;

                Ray ray = new Ray(ThisPlayerShooting.firePosition.position, ThisPlayerShooting.firePosition.forward);

                bool result = Physics.Raycast(ray, out hit, 50);
                if (hit.transform.gameObject.tag == "Player" || hit.transform.gameObject.tag == "Destructable")
                {
                    ThisPlayerShooting.FireShot();
                    elapsedTime = 0;
                }
            }
        }
    }
}
