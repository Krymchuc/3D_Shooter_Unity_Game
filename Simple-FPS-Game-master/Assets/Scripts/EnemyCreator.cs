﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyCreator : MonoBehaviour
{
    public GameObject prefab;
    float time = 4f;
    float currentMaxTime = 4f;
    // Update is called once per frame
    void Update()
    {
        //час простою
        time -= Time.deltaTime;
        //Що робити, якщо час опускається нижче 0?
        if (time <= 0)
        {
            // Створення зомбі в координатах від -90 до 90
            System.Random rand = new System.Random();
            float x, y, z;
            x = rand.Next(-90, 90);
            z = rand.Next(-90, 90);
            y = Terrain.activeTerrain.SampleHeight(new Vector3(x, 0, z));
            GameObject Enemy = Instantiate(prefab, new Vector3(x, y, z), new Quaternion(0, 0, 0, 0));
            EnemyFSM enemyState = Enemy.GetComponent<EnemyFSM>();
            Transform scale = Enemy.GetComponentInChildren<Transform>();
            if (Score.time > 60)
            {
                currentMaxTime = 3.5f;
                scale.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                enemyState.maxHp = 20;
                enemyState.hp = enemyState.maxHp;
                enemyState.moveSpeed = 7;
            } else if(Score.time > 120)
            {
                currentMaxTime = 3f;
                scale.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                enemyState.maxHp = 25;
                enemyState.hp = enemyState.maxHp;
                enemyState.moveSpeed = 9;
            } else if(Score.time > 180)
            {
                currentMaxTime = 2.5f;
                scale.transform.localScale = new Vector3(2f, 2f, 2f);
                enemyState.maxHp = 30;
                enemyState.hp = enemyState.maxHp;
                enemyState.moveSpeed = 11;
            } else if(Score.time > 240)
            {
                currentMaxTime = 2f;
                scale.transform.localScale = new Vector3(4f, 2.4f, 4f);
                enemyState.maxHp = 50;
                enemyState.hp = enemyState.maxHp;
                enemyState.moveSpeed = 13;
            }
            time = currentMaxTime;
        }
    }
}
