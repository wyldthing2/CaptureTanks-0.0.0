using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;





public class CaptureBase : MonoBehaviour {

    [SerializeField] string BaseName = "Base";
    [SerializeField] float captureTimeRequired = 60f;
    [SerializeField] float uncaptureFactor = .5f;
    [SerializeField] float captureFactor = 1f;
    [SerializeField] GameObject BlobCallingObject;
    [SerializeField] GameObject CanvasLabel;
    [SerializeField] GameObject CanvasTime;
    [SerializeField] public GameObject BaseBuilder;

    [SerializeField] ToggleEvent OnToggleBase;

    BlobSpawner _blobSpawner;
    private Text labelText;
    private Text timeText;
    float elapsedTime = 0f;
    [SerializeField] float captureTime = 0f;
    float timeToCapture;
    public bool Captured = false;

    



    void Start ()
    {
        labelText = CanvasLabel.GetComponent<Text>();
        timeText = CanvasTime.GetComponent<Text>();

        _blobSpawner = GetComponentInChildren<BlobSpawner>();


    }
	
	void Update ()
    {

        elapsedTime += Time.deltaTime;
        

        if (captureTime < captureTimeRequired && captureTime > 0)
        {
            captureTime -= Time.deltaTime * uncaptureFactor;
        }
	}

    
    void OnTriggerStay(Collider Base)
    {
        

        if (Base.gameObject.tag == "Player" && Captured == false)
        {
            labelText.text = "Capturing " + BaseName + ":";
            timeToCapture = captureTimeRequired - captureTime;
            timeText.text = timeToCapture.ToString();

            if (captureTime < captureTimeRequired && captureTime > -10 && Captured == false)
            {
                captureTime += Time.deltaTime * captureFactor;
                if (captureTime > captureTimeRequired)
                {
                    Captured = true;
                    BlobSpawnController.commands.SpawnerList.Remove(_blobSpawner);
                    captureTime = 0;
                    //BaseBuilder.SetActive(true);
                    OnToggleBase.Invoke(true);
                    //still need to add the spawner base when base is lost
                }

            }
        }

        if (Base.gameObject.tag == "Blob" && Captured == true)
        {
            if (captureTime < captureTimeRequired && captureTime > -1)
            {
                captureTime += Time.deltaTime * captureFactor;
                if (captureTime > captureTimeRequired)
                {
                    Captured = false;
                    BlobSpawnController.commands.SpawnerList.Add(_blobSpawner);
                    captureTime = 0;
                    //still need to add the spawner base when base is lost
                    BaseBuilder.GetComponent<BaseBuilder>().CmdResetWalls();
                    //BaseBuilder.SetActive(false);
                    OnToggleBase.Invoke(false);

                }

            }
        }
    }

    void CallBlobsToBase(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        int i = 0;
        while (i < hitColliders.Length)
        {
            AIJumper AIJumperScript = hitColliders[i].transform.GetComponent<AIJumper>();
            AIJumperScript.agent.destination = this.transform.position;
            i++;
        }
    }

}
