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
            Color yellow = new Color(201 / 255f, 175 / 255f, 141 / 255f, 1f);
            Body.color = Body.color == c ? yellow : c;
        }
        if (act == "TurnPink")
        {
            Color c = Body.color;
            Color pink = new Color(195 / 255f, 111 / 255f, 97/ 255f, 1f);
            Body.color = Body.color == c ? pink : c;
        }
        if (act == "TurnBlue")
        {
            Color c = Body.color;
            Color blue = new Color(66 / 255f, 70 / 255f, 83 / 255f, 1f);
            Body.color = Body.color == c ? blue : c;
        }
        if (act == "TurnWhite")
        {
            Color c = Body.color;
            Color white = new Color(200/255f,182/255f,177/255f, 1f);
            Body.color = Body.color == c ? white : c;
        }

        if (act == "TurnTan")
        {
            Color c = Body.color;
            Color tan = new Color(179/255f, 128/255f, 94/255f, 1f);
            Body.color = Body.color == c ? tan : c;
        }

        if (act == "TurnPurple")
        {
            Color c = Body.color;
            Color purple = new Color (66/255f,46/255f,58/255f, 1f);
            Body.color = Body.color == c ? purple : c;
        }
        
        if (act == "TurnGrey")
        {
            Color c = Body.color;
            Color grey = new Color (120/255f,127/255f,142/255f, 1f);
            Body.color = Body.color == c ? grey : c;
        }
        if (act == "TurnDark")
        {
            Color c = Body.color;
            Color dark = new Color (12/255f,9/255f,11/255f, 1f);
            Body.color = Body.color == c ? dark : c;
        }
    }
    public IEnumerator Flash2(float amt)
    {
        Color newColor = new Color(120/255f, 117/255f, 125/255f, 1f);
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
        Color newColor = new Color(66/255f,46/255f,58/255f, 1f);
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
