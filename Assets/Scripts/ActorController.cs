using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ActorController : MonoBehaviour
{
    //My rigidbody
    public Rigidbody2D RB;
    //My main sprite
    public SpriteRenderer Body;
    //My Collider
    public Collider2D Coll;
    //My animator
    public Animator Anim;
    //My health. MaxHealth records your health at the start of the game
    public float Health;
    protected float MaxHealth;
    public float Speed = 5;
    public float SpinSpeed = 5;
    public float SizeSpeed = 5;
    // public float LerpSpeed = 0.1f;
    
    //Your default projectile. Leave null if this object doesn't shoot
    public ProjectileController DefaultProjectile;

    public List<ProjectileController> AltProjectiles;
    //Spawns this gnome (particle and audio emitter) when you shoot 
    public GnomeScript ShootGnome;
    //Spawns this gnome (particle and audio emitter) when you take damage 
    public GnomeScript HurtGnome;
    //Spawns this gnome (particle and audio emitter) when you die
    public GnomeScript DeathGnome;

    protected  bool HasIdle = false;
    protected  string CurrentAnim = "";

    protected Vector3 DesiredPos;
    protected  MoveStyle ChasingDesiredPos = MoveStyle.None;
    protected  float DesiredRot;
    protected  MoveStyle ChasingDesiredRot = MoveStyle.None;
    protected Vector3 DesiredSize;
    protected  MoveStyle ChasingDesiredSize = MoveStyle.None;

    protected Color FadeColor;
    protected float IFrames = 0;

    void Awake()
    {
        //We do this because you can't make Awake virtual
        OnAwake();
    }

    void Start()
    {
        //We do this because you can't make Start virtual
        OnStart();
        if (Coll == null) Coll = GetComponent<Collider2D>();
        if (Body) FadeColor = Body.color;
        //The GameManager tracks all the actors that exist in the game
        //Add us to it when the scene begins
        GameManager.Singleton.AddActor(this);
        
        //If we have an animator and an animation named "Idle", note that for later
        if (Anim != null)
        {
            foreach (AnimationClip c in Anim.runtimeAnimatorController.animationClips)
            {
                //Debug.Log(gameObject.name + ": " + c.name);
                if (c.name == "Idle")
                    HasIdle = true;
            }
        }
    }

    void Update()
    {
        //If we're currently invincible, count down until we aren't
        if (IFrames > 0)
        {
            IFrames -= Time.deltaTime;
        }
        //We do this because you can't make Update virtual 
        OnUpdate();
    }

    public virtual void OnAwake()
    {
        //We do this because you can't make Awake virtual
        MaxHealth = Health;
    }
    
    public virtual void OnStart()
    {
        
    }
    
    public virtual void OnUpdate()
    {
        //If we're moving towards a point, do so
        if (ChasingDesiredPos == MoveStyle.Linear)
        {
            transform.position = Vector3.MoveTowards(transform.position, DesiredPos, Speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, DesiredPos) < 0.1f)
            {
                ChasingDesiredPos = MoveStyle.None;
            }
        }
        else if (ChasingDesiredPos == MoveStyle.Lerp)
        {
            transform.position = Vector3.Lerp(transform.position, DesiredPos, Speed * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, DesiredPos, Speed * 0.001f);
            if (Vector2.Distance(transform.position, DesiredPos) < 0.01f)
            {
                ChasingDesiredPos = MoveStyle.None;
            }
        }
        //If we're rotating towards a rotation, do so
        if (ChasingDesiredRot == MoveStyle.Linear)
        {
            // Debug.Log("ROT1: " + transform.rotation.eulerAngles.z);
            transform.rotation = Quaternion.Euler(0,0,Mathf.MoveTowards(transform.rotation.eulerAngles.z, DesiredRot, SpinSpeed * 90 * Time.deltaTime));
            // Debug.Log("ROT2: " + transform.rotation.eulerAngles.z);
            if (Mathf.Abs(DesiredRot - transform.rotation.eulerAngles.z) < 0.01f)
            {
                ChasingDesiredRot = MoveStyle.None;
            }
        }
        else if (ChasingDesiredRot == MoveStyle.Lerp)
        {
            // Debug.Log("ROT3: " + transform.rotation.eulerAngles.z);
            transform.rotation = Quaternion.Euler(0,0,Mathf.LerpAngle(transform.rotation.eulerAngles.z, DesiredRot, SpinSpeed * Time.deltaTime));
            transform.rotation = Quaternion.Euler(0,0,Mathf.MoveTowards(transform.rotation.eulerAngles.z, DesiredRot, SpinSpeed * 0.01f));
            // Debug.Log("ROT4: " + transform.rotation.eulerAngles.z);
            if (Mathf.Abs(DesiredRot - transform.rotation.eulerAngles.z) < 0.01f)
            {
                ChasingDesiredRot = MoveStyle.None;
            }
        }
        if (ChasingDesiredSize == MoveStyle.Linear)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, DesiredSize, SizeSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.localScale, DesiredSize) < 0.1f)
            {
                ChasingDesiredSize = MoveStyle.None;
            }
        }
        else if (ChasingDesiredSize == MoveStyle.Lerp)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, DesiredSize, SizeSpeed * Time.deltaTime);
            transform.localScale = Vector3.MoveTowards(transform.localScale, DesiredSize, SizeSpeed * 0.001f);
            if (Vector2.Distance(transform.localScale, DesiredSize) < 0.01f)
            {
                ChasingDesiredSize = MoveStyle.None;
            }
        }
    }

    ///Sets where the character wants to move and how
    public void SetDesiredPos(Vector3 where, MoveStyle how)
    {
        where.z = transform.position.z;
        DesiredPos = where;
        ChasingDesiredPos = how;
    }

    ///Sets where the character wants to be rotated and how
    public void SetDesiredRot(float rot, MoveStyle how)
    {
        DesiredRot = rot;
        ChasingDesiredRot = how;
    }

    ///Sets where the character wants to be sized and how
    public void SetDesiredSize(Vector3 size, MoveStyle how)
    {
        DesiredSize = size;
        ChasingDesiredSize = how;
    }
    
    /// Deals damage to an actor, killing them if they hit 0 health
    public virtual void TakeDamage(float amt, ActorController source=null)
    {
        //If we don't have health, we don't take damage
        if (MaxHealth <= 0 || IFrames > 0) return;
        //Lower health by amount and die if it hits 0
        Health -= amt;
        IFrames = 0.5f;
        if(HurtGnome != null)
            Instantiate(HurtGnome, transform.position, transform.rotation);
        if(Health <= 0)
            Die(source);
    }

    /// Called when an actor is reduced to 0HP
    public virtual void Die(ActorController source=null)
    {
        //Default activity on death is to just get deleted
        //You may want to override this with something fancier
        if(DeathGnome != null)
            Instantiate(DeathGnome, transform.position, transform.rotation);
        Destroy(gameObject);
    }
    
    ///Rotate to look at a target. If turnTime is over 0, it takes you turnTime to do a full turn around
    public float LookAt(ActorController targ,float turnTime=0)
    {
        if (targ == null) return 0;
        return LookAt(targ.transform.position,turnTime);
    }
    public float LookAt(GameObject targ,float turnTime=0)
    {
        if (targ == null) return 0;
        return LookAt(targ.transform.position,turnTime);
    }
    public float LookAt(Vector3 targ,float turnTime=0)
    {
        //Calculate what z-rotation you'd need to face the target. This is just trig
        Vector3 diff = targ - transform.position;
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        //If you turn slowly, only rotate part of the way there
        float z = turnTime > 0 ? Mathf.MoveTowardsAngle(transform.rotation.eulerAngles.z, rot_z, (180/turnTime) * Time.deltaTime) : rot_z;
        //Plug the rotation you calculated into your actual transform.rotation
        transform.rotation = Quaternion.Euler(0,0,z);
        //Returns how much rotation you still need to do to look at your target 
        return Mathf.Abs(Mathf.DeltaAngle(z, rot_z));
    }

    public void ShootAtPlayer()
    {
        ShootAtPlayer(DefaultProjectile,transform.position);
    }

    public void ShootAtPlayer(ProjectileController prefab)
    {
        ShootAtPlayer(prefab,transform.position);
    }

    public void ShootAtPlayer(ProjectileController prefab, Vector3 pos)
    {
        ShootAt(PlayerController.Player.transform.position,prefab,pos);
    }

    //Shoot at a target
    public void ShootAt(GameObject targ)
    {
        ShootAt(targ.transform.position,DefaultProjectile,transform.position);
    }
    public void ShootAt(GameObject targ, ProjectileController prefab)
    {
        ShootAt(targ.transform.position,prefab,transform.position);
    }
    public void ShootAt(GameObject targ, ProjectileController prefab, Vector3 pos)
    {
        ShootAt(targ.transform.position,prefab,pos);
    }
    public void ShootAt(Vector3 targ)
    {
        ShootAt(targ,DefaultProjectile,transform.position);
    }
    public void ShootAt(Vector3 targ, ProjectileController prefab)
    {
        ShootAt(targ,prefab,transform.position);
    }
    public void ShootAt(Vector3 targ, ProjectileController prefab, Vector3 pos)
    {
        Vector3 diff = targ - pos;
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        Shoot(prefab,pos,new Vector3(0,0,rot_z));
    }
    
    //Spawns a bullet and calls setup on it
    //Three versions, so you can decide if you want to provide all info or default on it
    public void Shoot(ProjectileController prefab=null)
    {
        Shoot(prefab,transform.position,transform.rotation.eulerAngles);
    }
    public void Shoot(ProjectileController prefab, Vector3 pos)
    {
        Shoot(prefab,pos,transform.rotation.eulerAngles);
    }
    public void Shoot(ProjectileController prefab,Vector3 pos, Vector3 rot)
    {
        //If I don't list a prefab, use my default projectile
        if (prefab == null) prefab = DefaultProjectile;
        if (prefab == null) return;
        //Spawn a bullet
        ProjectileController p = Instantiate(prefab, pos, Quaternion.Euler(rot));
        //Then call setup on it
        p.Setup(this);
        //If I have a shoot gnome set up, spawn it
        if(ShootGnome != null)
            Instantiate(ShootGnome, pos, Quaternion.Euler(rot));
    }

    private void OnDestroy()
    {
        //The GameManager tracks all the actors that exist in the game
        //Remove us from it when we leave the scene
        GameManager.Singleton.RemoveActor(this);
    }

    //This gets called by JSON
    public void TakeEvent(EventJSON e)
    {
        //If it calls for an animation, call it!
        if (Anim != null && !string.IsNullOrEmpty(e.Anim))
        {
            Anim.Play(e.Anim);
            if(HasIdle) StartCoroutine(TrackAnim(e.Anim));
        }
        //If it calls for an action, run it!
        if (!string.IsNullOrEmpty(e.Action))
        {
            DoAction(e.Action,e.Amt);
        }
    }

    public IEnumerator TrackAnim(string animName)
    {
        CurrentAnim = animName;
        yield return new WaitForSeconds(0.1f);
        float Duration = 0;
        foreach (AnimationClip c in Anim.runtimeAnimatorController.animationClips)
        {
            if (c.name == animName)
                Duration = c.length / Anim.speed;
        }
            
        if (Duration > 0)
        {
            yield return new WaitForSeconds(Duration-0.1f);
            yield return null;
            if (CurrentAnim == animName)
            {
                //Debug.Log("PLAYIDLE");
                Anim.Play("Idle");
                CurrentAnim = "";
            }
        }

    }

    public virtual ProjectileController FindProjectile(float n)
    {
        if (n < AltProjectiles.Count) return AltProjectiles[(int)n];
        return DefaultProjectile;
    }

    //A virtual function meant to be overridden. Gets called whenever you have an event
    //act is equal to the event's "Action" json value
    //Each script you make should have its own override of this
    public virtual void DoAction(string act, float amt = 0)
    {
        //Reads the 'act' that's provided and runs different code depending on the message
        //This is usually a coroutine
        if (act == "Flash")
        {
            StartCoroutine(Flash(amt));
        }
        else if (act == "Shake")
        {
            StartCoroutine(Shake(amt));
        }
        else if (act == "Shoot")
        {
            Shoot(FindProjectile(amt));
        }
        else if (act == "ShootAtPlayer")
        {
            ShootAtPlayer(FindProjectile(amt));
        }
        else if (act == "Rotate")
        {
            transform.rotation = Quaternion.Euler(0,0,amt);
        }
        else if (act == "MoveRight")
        {
            transform.position += new Vector3(amt,0,0);
        }
        else if (act == "MoveUp")
        {
            transform.position += new Vector3(0,amt,0);
        }
        else if (act == "TurnOn")
        {
            gameObject.SetActive(true);
        }
        else if (act == "TurnOff")
        {
            gameObject.SetActive(false);
        }
        else if (act == "SetScale")
        {
            transform.localScale = new Vector3(amt, amt, 1);
        }
        else if (act == "SetAnimSpeed")
        {
            if (Anim != null) Anim.speed = amt;
        }
        else if (act == "SetX")
        {
            Vector3 pos = transform.position;
            pos.x = amt;
            transform.position = pos;
        }
        else if (act == "SetY")
        {
            Vector3 pos = transform.position;
            pos.y = amt;
            transform.position = pos;
        }
        else if (act == "SetToPlayerX")
        {
            Vector3 pos = transform.position;
            pos.x =  PlayerController.Player.transform.position.x;
            transform.position = pos;
        }
        else if (act == "SetToPlayerY")
        {
            Vector3 pos = transform.position;
            pos.y =  PlayerController.Player.transform.position.y;
            transform.position = pos;
        }
        else if (act == "MoveToX")
        {
            Vector3 pos = transform.position;
            if (ChasingDesiredPos != MoveStyle.None) pos = DesiredPos;
            pos.x = amt;
            SetDesiredPos(pos,MoveStyle.Linear);
        }
        else if (act == "MoveToY")
        {
            Vector3 pos = transform.position;
            if (ChasingDesiredPos != MoveStyle.None) pos = DesiredPos;
            pos.y = amt;
            SetDesiredPos(pos,MoveStyle.Linear);
        }
        else if (act == "LerpToX")
        {
            Vector3 pos = transform.position;
            if (ChasingDesiredPos != MoveStyle.None) pos = DesiredPos;
            pos.x = amt;
            SetDesiredPos(pos,MoveStyle.Lerp);
        }
        else if (act == "LerpToY")
        {
            Vector3 pos = transform.position;
            if (ChasingDesiredPos != MoveStyle.None) pos = DesiredPos;
            pos.y = amt;
            SetDesiredPos(pos,MoveStyle.Lerp);
        }
        else if (act == "MoveToRotation")
        {
            SetDesiredRot(amt,MoveStyle.Linear);
        }
        else if (act == "LerpToRotation")
        {
            SetDesiredRot(amt,MoveStyle.Lerp);
        }
        else if (act == "RotateToPlayer")
        {
            Vector3 diff = PlayerController.Player.transform.position - transform.position;
            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            Debug.Log("R2P: " + diff + " / " + rot_z);
            SetDesiredRot(rot_z,MoveStyle.Linear);
        }
        else if (act == "RotateToPlayerLerp")
        {
            Vector3 diff = PlayerController.Player.transform.position - transform.position;
            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            Debug.Log("R2PL: " + diff + " / " + rot_z);
            SetDesiredRot(rot_z,MoveStyle.Lerp);
        }
        else if (act == "LookAtPlayer")
        {
            Vector3 diff = PlayerController.Player.transform.position - transform.position;
            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0,0,rot_z);
        }
        else if (act == "RotateRelative")
        {
            float rot_z = transform.rotation.eulerAngles.z + amt;
            transform.rotation = Quaternion.Euler(0,0,rot_z);
        }
        else if (act == "ToSize")
        {
            Vector3 size = transform.localScale;
            if (ChasingDesiredSize != MoveStyle.None) size = DesiredSize;
            size.x = amt;
            size.y = amt;
            SetDesiredSize(size,MoveStyle.Linear);
        }
        else if (act == "SizeToX")
        {
            Vector3 size = transform.localScale;
            if (ChasingDesiredSize != MoveStyle.None) size = DesiredSize;
            size.x = amt;
            SetDesiredSize(size,MoveStyle.Linear);
        }
        else if (act == "SizeToY")
        {
            Vector3 size = transform.localScale;
            if (ChasingDesiredSize != MoveStyle.None) size = DesiredSize;
            size.y = amt;
            SetDesiredSize(size,MoveStyle.Linear);
        }
        else if (act == "LerpToSize")
        {
            Vector3 size = transform.localScale;
            if (ChasingDesiredSize != MoveStyle.None) size = DesiredSize;
            size.x = amt;
            size.y = amt;
            SetDesiredSize(size,MoveStyle.Lerp);
        }
        else if (act == "SizeLerpToX")
        {
            Vector3 size = transform.localScale;
            if (ChasingDesiredSize != MoveStyle.None) size = DesiredSize;
            size.x = amt;
            SetDesiredSize(size,MoveStyle.Lerp);
        }
        else if (act == "SizeLerpToY")
        {
            Vector3 size = transform.localScale;
            if (ChasingDesiredSize != MoveStyle.None) size = DesiredSize;
            size.y = amt;
            SetDesiredSize(size,MoveStyle.Lerp);
        }
        else if (act == "SetSpeed")
        {
            Speed = amt;
        }
        else if (act == "SetSpinSpeed")
        {
            SpinSpeed = amt;
        }
        else if (act == "SetSizeSpeed")
        {
            SizeSpeed = amt;
        }
        else if (act == "Stop")
        {
            ChasingDesiredPos = MoveStyle.None;
            ChasingDesiredRot = MoveStyle.None;
            ChasingDesiredSize = MoveStyle.None;
        }
        else if (act == "StopMove")
        {
            ChasingDesiredPos = MoveStyle.None;
        }
        else if (act == "StopRot")
        {
            ChasingDesiredRot = MoveStyle.None;
        }
        else if (act == "StopSize")
        {
            ChasingDesiredSize = MoveStyle.None;
        }
        else if (act == "RandomWalk")
        {
            Vector3 endPos = new Vector3(Random.Range(-5.5f,5.5f),Random.Range(-2.5f,2.5f));
            SetDesiredPos(endPos,MoveStyle.Lerp);
        }
        else if (act == "MoveToPlayer")
        {
            Vector3 pos = PlayerController.Player.transform.position;
            SetDesiredPos(pos,MoveStyle.Linear);
        }
        else if (act == "LerpToPlayer")
        {
            Vector3 pos = PlayerController.Player.transform.position;
            SetDesiredPos(pos,MoveStyle.Lerp);
        }
        else if (act == "MoveToPlayerX")
        {
            Vector3 pos = transform.position;
            pos.x =  PlayerController.Player.transform.position.x;
            SetDesiredPos(pos,MoveStyle.Linear);
        }
        else if (act == "MoveToPlayerY")
        {
            Vector3 pos = transform.position;
            pos.y =  PlayerController.Player.transform.position.y;
            SetDesiredPos(pos,MoveStyle.Linear);
        }
        else if (act == "LerpToPlayerX")
        {
            Vector3 pos = transform.position;
            pos.x =  PlayerController.Player.transform.position.x;
            SetDesiredPos(pos,MoveStyle.Lerp);
        }
        else if (act == "LerpToPlayerY")
        {
            Vector3 pos = transform.position;
            pos.y =  PlayerController.Player.transform.position.y;
            SetDesiredPos(pos,MoveStyle.Lerp);
        }
        else if (act == "FadeOut")
        {
            Color c = Body.color;
            c.a = 0;
            StartCoroutine(GameMaster.Fade(Body, c, amt > 0 ? amt : 0.5f));
        }
        else if (act == "FadeIn")
        {
            StartCoroutine(GameMaster.Fade(Body,FadeColor,amt > 0 ? amt : 0.5f));
        }
        else if (act == "HitboxOn")
        {
            if (Coll != null) Coll.enabled = true;
        }
        else if (act == "HitboxOff")
        {
            if (Coll != null) Coll.enabled = false;
        }
        else if (act == "VelX")
        {
            if (RB != null)
            {
                Vector2 vel = RB.linearVelocity;
                vel.x = amt;
                RB.linearVelocity = vel;
            }
        }
        else if (act == "VelY")
        {
            if (RB != null)
            {
                Vector2 vel = RB.linearVelocity;
                vel.y = amt;
                RB.linearVelocity = vel;
            }
        }
        else if (act == "VelPlayer")
        {
            if (RB != null)
            {
                Vector2 vel = PlayerController.Player.transform.position-transform.position;
                vel = vel.normalized * (amt > 0 ? amt : Speed);
                RB.linearVelocity = vel;
            }
        }
        
    }

    //Makes the actor flash red
    public IEnumerator Flash(float amt)
    {
        if (amt <= 0) amt = 0.5f;
        float bigTime = amt;
        float smTime = 0.1f;
        Color c = Body.color;
        while (bigTime > 0)
        {
            bigTime -= Time.deltaTime;
            smTime -= Time.deltaTime;
            if (smTime <= 0)
            {
                smTime = 0.1f;
                Body.color = Body.color == c ? Color.red : c;
            }
            yield return null;
        }
        Body.color = c;
    }
    
    //Makes the actor screenshake
    public IEnumerator Shake(float amt)
    {
        float time = amt > 0 ? amt : 0.5f;
        Vector3 startPos = Body.transform.localPosition;
        HazardController h = (this is HazardController) ? (HazardController)this : null;
        bool wh = false;
        if (h != null && h.WallHit == WallHitBehavior.Shake)
        {
            h.WallHit = WallHitBehavior.None;
            wh = true;
        }
        while (time > 0)
        {
            time -= Time.deltaTime;
            Body.transform.localPosition = startPos + 
                new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
            yield return null;
        }

        if (wh)
        {
            h.WallHit = WallHitBehavior.Shake;
        }
        Body.transform.localPosition = startPos;
    }

    public void ResetRotation()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}

public enum MoveStyle
{
    None=0,
    Linear=1,
    Lerp=2
}