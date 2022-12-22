using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeCreator : MonoBehaviour
{
    public GameObject prefab;
    float time = 30f;
    // Update is called once per frame
    void Update()
    {
        // час вимкнення
        time -= Time.deltaTime;
        //Якщо час опускається нижче 0?
        if (time <= 0)
        {
            // Створення зомбі в координатах від -90 до 90
            System.Random rand = new System.Random();
            float x, y, z;
            x = rand.Next(-90, 90);
            z = rand.Next(-90, 90);
            y = Terrain.activeTerrain.SampleHeight(new Vector3(x, 0, z));
            GameObject g400 = Instantiate(prefab, new Vector3(x, y + 0.5f, z), new Quaternion(0, 0, 0, 0));
            time = 30f;
        }
    }
}
