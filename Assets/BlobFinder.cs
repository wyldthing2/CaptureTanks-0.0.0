using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobFinder : MonoBehaviour {

    

    [SerializeField] public float blobCount;
    //[SerializeField] int mutationLimit;




    // Use this for initialization
    void Start()
    {
        PlayerCanvas.canvas.WriteGameStatusText("Spawn Work Please.");
    }

    // Update is called once per frame
    void Update()
    {

        //at each spawn, increase the count


        //if (blobCount > mutationLimit)



    }

    /*
   public void OnBlobCountChange(float value)
    {
        //blobCount += value;
        //Debug.Log(blobCount);

        if (isLocalPlayer)
        {
            //PlayerCanvas.canvas.SetKills(value, maxBlobKills);
            //PlayerCanvas.canvas.SetBlobKillBar(value / maxBlobKills);
        }
    }
    */

    
}
