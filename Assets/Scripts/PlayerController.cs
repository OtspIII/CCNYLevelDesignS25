using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : ActorController
{
    //A static variable to make the player easy to find
    public static PlayerController Player;
    


    public override void OnAwake()
    {
        base.OnAwake();
        Player = this;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        //You've seen this movement code before
        Vector2 vel = RB.linearVelocity;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            vel.x = Speed;
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            vel.x = -Speed;
        else
        {
            vel.x = 0;
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            vel.y = Speed;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            vel.y = -Speed;
        else
        {
            vel.y = 0;
        }
        RB.linearVelocity = vel;
    }

}
