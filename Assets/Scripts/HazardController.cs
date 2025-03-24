using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a type of Actor that hurts what it hits
//Useful for enemies, hazards, and projectiles
public class HazardController : ActorController
{
    //How much damage I deal on collision
    public float Damage = 1;
    //What happens when I hit a wall
    public WallHitBehavior WallHit;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Did what I hit have an actor script on it?
        ActorController act = other.gameObject.GetComponentInParent<ActorController>();
        //If so. . .
        if (act != null)
        {
            //Do your effect
            OnHit(act);
        }
        if(other.gameObject.CompareTag("Wall"))
            HitWall(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //Did what I hit have an actor script on it?
        ActorController act = other.gameObject.GetComponentInParent<ActorController>();
        //If so. . .
        if (act != null)
        {
            //Do your effect
            OnHit(act);
        }
        if(other.gameObject.CompareTag("Wall"))
            HitWall(other.gameObject);
    }

    //Gets called when it hits an outer wall
    public virtual void HitWall(GameObject obj)
    {
        //Most things don't care
        switch (WallHit)
        {
            case WallHitBehavior.Stop:
            {
                if(RB != null) RB.linearVelocity = Vector2.zero;
                break;
            }
            case WallHitBehavior.Bounce:
            {
                if (RB == null) break;
                Vector2 vel = RB.linearVelocity;
                if (obj.transform.localScale.x > obj.transform.localScale.y)
                    vel.y *= -1;
                else
                    vel.x *= -1;
                RB.linearVelocity = vel;
                break;
            }
            case WallHitBehavior.Destroy:
            {
                Destroy(gameObject);
                break;
            }
            case WallHitBehavior.Shake:
            {
                if(RB != null) RB.linearVelocity = Vector2.zero;
                StartCoroutine(Shake(0.1f));
                break;
            }
        }
    }

    //Gets called when it hits another actor
    public virtual void OnHit(ActorController act)
    {
        //If you do damage, do damage!
        if (Damage > 0)
        {
            act.TakeDamage(Damage);
        }
    }
}

public enum WallHitBehavior
{
    None=0,
    Stop=1,
    Bounce=2,
    Destroy=3,
    Shake=4,
}