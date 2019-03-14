using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerCanvas : NetworkBehaviour
{
    public static PlayerCanvas canvas;

    public SimpleHealthBar healthBar;

    [Header("Component References")]
    [SerializeField] Canvas reticule;
    [SerializeField] UIFader damageImage;
    [SerializeField] public Image waterImage;
    [SerializeField] Text gameStatusText;
    [SerializeField] Text healthValue;
    [SerializeField] Text killsValue;
    [SerializeField] Text logText;
    [SerializeField] AudioSource deathAudio;
    [SerializeField] Image blobKillBar;
    [SerializeField] Image mutationMeter;
    [SerializeField] public GameObject PlayerObject;

    //[Header("Links for Other Scripts")]
    //[SerializeField] BlobSpawnController CanvasLinkBlobSpawnController;

    //public static PlayerCanvas commands;



    //Ensure there is only one PlayerCanvas
    void Awake()
    {
        


        if (canvas == null)
         canvas = this; 

        

        else if (canvas != this)
            Destroy(gameObject);
    }


    void Start()
    {
        //blobKillBar.fillAmount = 0;
        //mutationMeter.fillAmount = 0;
    }

    //Find all of our resources
    /*void Reset()
    {
        reticule = GameObject.Find("Crosshair").GetComponent();
        damageImage = GameObject.Find("DamageFlash").GetComponent();
        gameStatusText = GameObject.Find("Online Text").GetComponent();
        healthValue = GameObject.Find("LifeValue").GetComponent();
        killsValue = GameObject.Find("BlobsValue").GetComponent();
        logText = GameObject.Find("LogText").GetComponent();
        deathAudio = GameObject.Find("DeathSound").GetComponent();
    }
    */

    public void Initialize()
    {
        reticule.enabled = true;
        gameStatusText.text = "";
    }

    public void HideReticule()
    {
        reticule.enabled = false;
    }

    public void AddPlayerToPlayerList()
    {

    }
      
    

    public void FlashDamageEffect()
    {
        damageImage.Flash();
    }

    public void PlayDeathAudio()
    {
        if (!deathAudio.isPlaying)
            deathAudio.Play();
    }

    public void SetKills(float amount, int max)
    {
        killsValue.text = amount.ToString() + "/" + max.ToString();
       
    }

    public void SetBlobKillBar(float amount)
    {
        blobKillBar.fillAmount =  amount;
    }

    
    public void SetMutationMeter(float amount)
    {
        mutationMeter.fillAmount = amount;
    }

    public void SetHealth(int amount, int max)
    {
        healthBar.UpdateBar(amount, max);


    }

    public void WriteGameStatusText(string text)
    {
        gameStatusText.text = text;
    }

    public void WriteLogText(string text, float duration)
    {
        CancelInvoke();
        logText.text = text;
        //Invoke("ClearLogText", duration);
    }

    void ClearLogText()
    {
        logText.text = "";
    }

    
    
}