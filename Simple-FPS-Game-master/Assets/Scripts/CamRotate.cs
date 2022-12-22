using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRotate : MonoBehaviour
{
    public float rotSpeed = 200f; // змінна швидкість обертання
    float mx = 0;
    float my = 0;

    // Update is called once per frame
    void Update()
    {
        // Дозволяє маніпулювати лише тоді, коли стан гри «в грі».
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }
        
        float mouse_X = Input.GetAxis("Mouse X"); 
        float mouse_Y = Input.GetAxis("Mouse Y");

        mx += mouse_X * rotSpeed * Time.deltaTime;
        my += mouse_Y * rotSpeed * Time.deltaTime;
        // Обмеження значення змінної повороту вертикального руху (my) між -90 і 90 градусами.
        my = Mathf.Clamp(my, -90f, 90f);

        transform.eulerAngles = new Vector3(-my, mx, 0);

    }
}
