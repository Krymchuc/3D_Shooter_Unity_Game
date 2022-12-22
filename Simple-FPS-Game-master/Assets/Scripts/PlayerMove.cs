using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 7f;
    CharacterController cc;
    float gravity = -20f;
    float yVelocity = 0;
    public float jumpPower = 10f;
    public bool isJumping = false;
    public int hp = 20;
    int maxHp = 20;
    public Slider hpSlider;
    public GameObject hitEffect;
    Animator anim; 

    // Update is called once per frame
    private void Start()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
    }
    void Update()
    {
        // Дозволяє маніпулювати лише тоді, коли стан гри «в грі».
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(h, 0, v);
        dir = dir.normalized; 
        anim.SetFloat("MoveMotion", dir.magnitude);

        // Конкретний вектор руху на основі ігрового об’єкта, до якого прикріплено компонент Transform
        // TransformDirection() перетворює на вектор відносного напрямку
        dir = Camera.main.transform.TransformDirection(dir);
        if (cc.collisionFlags == CollisionFlags.Below)// Чи торкається землі?
        {
            if (isJumping) 
            {
                isJumping = false;
            }
        }

        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            yVelocity = jumpPower;
            isJumping = true;
        }
        //Застосувати значення сили тяжіння до вертикальної швидкості персонажа.
        yVelocity += gravity * Time.deltaTime;
        dir.y = yVelocity;
        // Рухатися відповідно до швидкості руху
        cc.Move(dir * moveSpeed * Time.deltaTime);
        //    transform.position += dir * moveSpeed * Time.deltaTime;

        //Відображення здоров'я поточного гравця (%) до значення повзунка 
        hpSlider.value = (float)hp / (float)maxHp;
    }
    public void DamageAction(int damage)
    {
        hp -= damage;
        if (hp > 0)
        {
            StartCoroutine(PlayerHitEffect());
                
        }
    }
    IEnumerator PlayerHitEffect()
    {
        hitEffect.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        hitEffect.SetActive(false);
    }
}
