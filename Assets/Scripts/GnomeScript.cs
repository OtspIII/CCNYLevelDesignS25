using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class plays a sound and bursts out some particles
//The code doesn't need to be edited, but if you want to make custom
//  effects you should make a copy of these prefabs and edit them
public class GnomeScript : MonoBehaviour
{
    [Header("How Many Particles")]
    public int ParticleAmt = 10;
    [Header("What SFX Plays")]
    public AudioClip Clip;
    [Space]
    public ParticleSystem PS;
    public AudioSource AS;
    public float Lifetime = 0;
    
    void Start()
    {
        //If I have a particle system, I shoot particles
        if (PS != null)
        {
            PS.Emit(ParticleAmt);
            Lifetime = Mathf.Max(PS.main.duration, Lifetime);
        }

        if (AS != null && Clip != null)
        {
            AS.PlayOneShot(Clip);
            Lifetime = Mathf.Max(Lifetime, Clip.length);
        }
    }

    void Update()
    {
        Lifetime -= Time.deltaTime;
        if (Lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
