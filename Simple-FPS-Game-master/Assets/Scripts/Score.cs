using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public static float score = 0;
    public static float time = 0f;
    PlayerMove player;
    Text text;
    // Update is called once per frame
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMove>();
        text = GetComponent<Text>();
    }
    void Update()
    {
        if ( player.hp > 0)
        {
            //Збільшити оцінку за часом
            time += Time.deltaTime;
            score += Time.deltaTime;
            text.text = "Score : " + (int)score;
        }
    }
}
