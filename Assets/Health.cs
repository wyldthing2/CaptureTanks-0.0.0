using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

    [SerializeField] int maxHealth = 6; //consider SC2 health



    //Only server can set value of SyncVar
    [SyncVar(hook = "OnHealthChanged")] int health;
    //[SyncVar (hook = "PlayerDied")] bool died;

    [SerializeField] TurretAI AIToTurnOff;
    [SerializeField] List<GameObject> ObjectsToDisableOnDeath = new List<GameObject>();



    void Awake()
    {
    }

    [ServerCallback]
    void OnEnable()
    {
        health = maxHealth;
    }

    [ServerCallback]
    void Start()
    {
        health = maxHealth;
    }

    [Server]
    public bool TakeDamage()
    {
        //Assume they're alive
        bool died = false;

        //Already below zero before health--? Then, he's not dead because of you
        if (health <= 0)
            return died;

        health--;

        //Did that health-- change anything? Is it 0 now? Then he dead.
        died = health <= 0;

        //tell the player
        RpcTakeDamage(died);

        //Tell me (the server)
        return died;
    }

    [ClientRpc]
    void RpcTakeDamage(bool died)
    {
        if (died)
        {

            Die();
            Invoke("revive", 30);
        }
    }

    void Die()
    {
        AIToTurnOff.enabled = false;
        foreach (GameObject objectToDisable in ObjectsToDisableOnDeath)
        {
            objectToDisable.SetActive(false);
        }
    }

    void revive()
    {
        AIToTurnOff.enabled = true;
        foreach (GameObject objectToDisable in ObjectsToDisableOnDeath)
        {
            objectToDisable.SetActive(true);
        }
    }

    void OnHealthChanged(int value)
    {
        health = value;

        //healthBar.UpdateBar(value, maxHealth);
        if (isLocalPlayer)
            PlayerCanvas.canvas.SetHealth(value, maxHealth);
    }
}
