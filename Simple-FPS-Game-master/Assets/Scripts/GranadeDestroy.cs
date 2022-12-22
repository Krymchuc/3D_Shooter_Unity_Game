using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GranadeDestroy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            ////взяти патрони, щоб скинути магазин до 30
            PlayerFire.g400 = 3;
            Text g400Text = GameObject.Find("Granade").GetComponent<Text>();
            g400Text.text = "Leftover grenades: " + PlayerFire.g400;
            Destroy(gameObject);
        }
    }
}
