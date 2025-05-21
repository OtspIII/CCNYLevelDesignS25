using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Miles : MonoBehaviour
{
    PlayerController player;
    float miles = 0;
    TextMeshPro text;
    void Start()
    {
        player = PlayerController.Player;
        text = GetComponent<TextMeshPro>();
    }

    void Update()
    {
        if (player.RB.linearVelocity != Vector2.zero)
        {
            miles += Time.deltaTime * 4.0f;
        }

        text.text = "Miles Walked: " + Mathf.RoundToInt(miles).ToString();
    }
}
