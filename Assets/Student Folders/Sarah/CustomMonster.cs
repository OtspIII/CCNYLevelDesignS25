using System.Collections;
using UnityEngine;

public class CustomMonster : HazardController
{
    public override void DoAction(string act, float amt = 0)
    {
        base.DoAction(act, amt);
        if (act == "Flash2")
        {
            StartCoroutine(Flash2(amt));
        }
        if (act == "Flash3")
        {
            StartCoroutine(Flash3(amt));
        }

        if (act == "Teleport")
        {
            transform.position = new Vector2(Random.Range(-5.5f,5.5f),Random.Range(-2.5f,2.5f));
        }

        if (act == "TurnYellow")
        {
            Color c = Body.color;
            Color yellow = new Color(247 / 255f, 202 / 255f, 60 / 255f, 1f);
            Body.color = Body.color == c ? yellow : c;
        }
        if (act == "TurnPink")
        {
            Color c = Body.color;
            Color pink = new Color(242 / 255f, 119 / 255f, 176 / 255f, 1f);
            Body.color = Body.color == c ? pink : c;
        }
        if (act == "TurnBlue")
        {
            Color c = Body.color;
            Color blue = new Color(13 / 255f, 55 / 255f, 110 / 255f, 1f);
            Body.color = Body.color == c ? blue : c;
        }
        if (act == "TurnWhite")
        {
            Color c = Body.color;
            Color white = new Color(1,1,1, 1f);
            Body.color = Body.color == c ? white : c;
        }

        if (act == "TurnGreen")
        {
            Color c = Body.color;
            Color green = new Color(74f/255f, 114f/255f, 71f/255f, 1f);
            Body.color = Body.color == c ? green : c;
        }

        if (act == "TurnPurple")
        {
            Color c = Body.color;
            Color purple = new Color (127/255f,79/255f,164/255f, 1f);
            Body.color = Body.color == c ? purple : c;
        }
    }
    public IEnumerator Flash2(float amt)
    {
        Color newColor = new Color(74f/255f, 114f/255f, 71f/255f, 1f);
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
                smTime = 0.3f;
                Body.color = Body.color == c ? newColor : c;
            }
            yield return null;
        }
        Body.color = c;
    }
    
    public IEnumerator Flash3(float amt)
    {
        Color newColor = new Color(127/255f,79/255f,164/255f, 1f);
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
                smTime = 0.3f;
                Body.color = Body.color == c ? newColor : c;
            }
            yield return null;
        }
        Body.color = c;
    }
}
