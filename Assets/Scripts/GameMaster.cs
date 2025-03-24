using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//This class has some useful random little functions
//You can ignore it, but maybe something within it will be useful to you
public static class GameMaster
{
    /// Loads the next scene in the Build Settings queue.
    public static void NextStage()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /// Returns either true or false. The higher the odds param is, the more likely true.
    public static bool CoinFlip(float odds=0.5f)
    {
        return UnityEngine.Random.Range(0,1f) < odds;
    }
    
    /// Rounds a float to an int. If not a round number, randomly rounds (odds based on how close to each pole). 
    public static int RoundRand(float n)
    {
        int r = (int) n;
        float rem = n - r;
        if (UnityEngine.Random.Range(0f, 1f) <= rem)
            r++;
        return r;
    }
    
    ///Returns a random entry of the list
    public static T Random<T>(this List<T> l) where T:class
    {
        if (l.Count == 0)
            return null;
        return l[UnityEngine.Random.Range(0, l.Count)];
    }

    ///Shuffles the list
    public static List<T> Shuffle<T>(this List<T> l) where T:class
    {
        if (l.Count == 0)
            return l;
        List<T> temp = new List<T>();
        temp.AddRange(l);
        List<T> r = new List<T>();
        while (temp.Count > 0)
        {
            T chosen = temp.Random();
            temp.Remove(chosen);
            r.Add(chosen);
        }
        return r;
    }
    
    ///Turns the contents of a list into a string. Meant for Debug.Log
    public static string Summary<T>(this List<T> l) 
    {
        string r = "";
        foreach (T t in l)
            r += (r == "" ? "" : " / ") + t.ToString();
        return "List: {" + r + "}";
    }
    
    ///Tells you where your mouse is in the game world
    public static Vector2 MouseLoc(Camera cam=null)
    {
        if (cam == null)
            cam = UnityEngine.Camera.main;
        return cam.ScreenToWorldPoint(Input.mousePosition);
    }
    
    ///Multiple easing equations. Feed it a number between 0-1 and it'll give you a curve. See https://easings.net/ for more.
    public static float Ease(float t, Eases e=Eases.Out)
    {
        switch (e)
        {
            case Eases.Out: return (--t) * t * t + 1;
            case Eases.InOut: return (--t) * t * t + 1;
            case Eases.BounceOut:
            {
                float c1 = 1.70158f;
                float c3 = c1 + 1;
                return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
            }
            case Eases.Elastic:
            {
                float c4 = (2 * Mathf.PI) / 3;
                return t == 0 ? 0 : t == 1 ? 1 : Mathf.Pow(2, -10 * t) * Mathf.Sin((t * 10 - 0.75f) * c4) + 1;
            }
        }

        return t;
    }
    
    ///Rolls and sums dice. Rolls [rolls]d[size]+[bonus]
    public static int Roll(int rolls, int size, int bonus=0){
        int r = 0;
        if (size > 0) {
            for (int n = 0; n < Mathf.Abs(rolls); n++)
                r += UnityEngine.Random.Range (1, size+1);
        }
        r += bonus;
        if (r < 0)
            r = 0;
        if (rolls < 0)
            return -r;
        return r;
    }

    public static IEnumerator Fade(SpriteRenderer sr, bool fadeOut = true, float time = 0.5f, bool turnOff = true)
    {
        sr.gameObject.SetActive(true);
        float start = fadeOut ? 0 : 1;
        float end = fadeOut ? 1 : 0;
        float t = 0;
        while (t < 1)
        {
            float tt = GameMaster.Ease (t, Eases.Out);
            sr.color = new Color(0,0,0,Mathf.Lerp(start,end,tt));
            yield return null;
            t += Time.deltaTime / time;
        }
        sr.color = new Color(0,0,0,end);
        if(!fadeOut && turnOff)
            sr.gameObject.SetActive(false);
    }
    
    ///A coroutine that fades a sprite renderer out or in.
    public static IEnumerator Fade(SpriteRenderer sr, Color targColor, float time=0.5f)
    {
        sr.gameObject.SetActive(true);
        Color start = sr.color;
        float t = 0;
        while (t < 1)
        {
            float tt = GameMaster.Ease (t, Eases.Out);
            sr.color = Color.Lerp(start,targColor,tt);
            yield return null;
            t += Time.deltaTime / time;
        }
        sr.color = targColor;
    }
}

//For use with GameMaster.Ease()
public enum Eases
{
    None=0,
    Out=1,
    InOut=2,
    Elastic=3,
    BounceOut=4
}