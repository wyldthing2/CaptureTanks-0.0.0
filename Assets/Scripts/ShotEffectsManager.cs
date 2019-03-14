using UnityEngine;

public class ShotEffectsManager : MonoBehaviour
{

    //[SerializeField] ParticleSystem muzzleFlash;
    //[SerializeField] AudioSource gunAudio;
    [SerializeField] GameObject impactPrefab;
    [SerializeField] GameObject deathPrefab;

    public ParticleSystem impactEffect;
    public ParticleSystem deathEffect;


    public void Initialize()
    {
        impactEffect = Instantiate(impactPrefab).GetComponent<ParticleSystem>();
        deathEffect = Instantiate(deathPrefab).GetComponent<ParticleSystem>();
	}

    public void PlayShotEffects()
    {
        //muzzleFlash.Stop(true);
        //muzzleFlash.Play(true);
        //gunAudio.Stop();
        //gunAudio.Play();
    }

    public void PlayImpactEffect(Vector3 impactPosition)
    {
        //impactEffect.transform.position = impactPosition;
        //impactEffect.Stop();
        //impactEffect.Play();
    }

    
    public void PlayDeathEffect(Vector3 impactPosition)
    {
        deathEffect.transform.position = impactPosition;
        deathEffect.Stop();
        deathEffect.Play();
    }
    

}
