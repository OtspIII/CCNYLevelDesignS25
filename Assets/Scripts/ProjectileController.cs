using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : HazardController
{
    
    
    //Who spawned me
    public ActorController Source;
    public bool OnlyHitPlayer = false;

    public virtual void Setup(ActorController src) 
    {
        //Who spawned you?
        Source = src;
    }
    
    private void Update()
    {
        //Just go flying in the direction I'm facing!
        //It's up to the gun to rotate me so that I'm facing the correct direction
        RB.linearVelocity = transform.right * Speed;
    }

    public override void OnHit(ActorController act)
    {
        //If you hit the person who spawned you, don't hurt them
        if (act == Source) return;
        //if the tag is projectile then don't do anything
        if (act.gameObject.CompareTag("Projectile")) return;
        //If I'm only set to hit the player, don't do anything if I hit an ally
        if (OnlyHitPlayer && !act.gameObject.CompareTag("Player")) return;

        //Do your normal payload delivery
        base.OnHit(act);
        
        //You've delivered your payload, self destruct
        Destroy(gameObject);
    }

    public override void HitWall(GameObject obj)
    {
        if(WallHit != WallHitBehavior.None)
            base.HitWall(obj);
        else
            Destroy(gameObject);
    }
}
