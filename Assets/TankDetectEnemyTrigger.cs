using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankDetectEnemyTrigger : MonoBehaviour {

    public List<GameObject> EnemyGameObjectList = new List<GameObject>();
    [SerializeField] public MovementAI ThisMovementAI;
    [SerializeField] public TeamID ThisTeamID;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != ThisMovementAI.gameObject)
        {
            if (other.gameObject.GetComponent<TeamID>() != null)
            {

                if (other.gameObject.tag == "Player" || other.gameObject.tag == "Destructable")
                {
                    if (other.gameObject.GetComponent<TeamID>().TeamIDNumber != ThisTeamID.TeamIDNumber)
                    {

                        if (!EnemyGameObjectList.Contains(other.gameObject))
                        {

                            EnemyGameObjectList.Add(other.gameObject);


                        }
                        if (other.gameObject.tag == "Player")
                        {
                            float chanceToSetTarget = UnityEngine.Random.value;
                            if (chanceToSetTarget > .1)
                            {
                                ThisMovementAI.SetTargetEnemyToChase(other.gameObject);
                            }
                        }
                    }
                }
            }
        }
    }

}
