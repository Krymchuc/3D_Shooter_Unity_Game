using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFire : MonoBehaviour
{
    public GameObject firePosition; 
    public GameObject bombFactory; 
    public float throwPower = 15f;
    public int weaponPower = 5;
    public GameObject bulletEffect;
    ParticleSystem ps;// система частинок ефекту попадання
    Animator anim;
    public Image wModeImage;
    public GameObject[] eff_Flash;
    AudioSource audioSource;
    public AudioClip boltUp;
    public AudioClip fire;
    public AudioClip granade;
    public AudioClip dryFire;
    public AudioClip zoom;
    public GameObject ironSite;
    public GameObject crossHair;
    public static int magazine;
    public static int g400; 
    public Text magazineText;
    public Text g400Text;
    enum WeponMode //змінна режиму зброї
    {
        Normal,
        Sniper
    }
    WeponMode wMode;
    bool ZoomMode = false;
    private void Start()
    {
        ps = bulletEffect.GetComponent<ParticleSystem>();
        anim = GetComponentInChildren<Animator>();
        wMode = WeponMode.Normal;
        audioSource = GetComponent<AudioSource>();
        magazine = 30;
        g400 = 3;
    }
    // Update is called once per frame
    void Update()
    {
        // Дозволяє маніпулювати лише тоді, коли стан гри «в грі».
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }
        //Звичайний режим: натискання правої кнопки миші кидає гранату в напрямку прямої видимості.
        //Снайперський режим: я хочу збільшити масштаб екрана, коли натискаю праву кнопку миші.
        if (Input.GetMouseButtonDown(1))//якщо клацнути правою кнопкою миші
        {
            switch (wMode)
            {
                case WeponMode.Normal:
                    if (g400 != 0)
                    {
                       g400 -= 1;
                        g400Text.text = "Leftover grenades: " + g400;
                       GameObject bomb = Instantiate(bombFactory); // створити гранату
                        bomb.transform.position = firePosition.transform.position;
                        //Отримати компонент Rigidbody об'єкта гранати
                        Rigidbody rb = bomb.GetComponent<Rigidbody>();
                        //Прикладіть фізичну силу до гранати в напрямку передньої частини камери.
                        rb.AddForce(Camera.main.transform.forward * throwPower, ForceMode.Impulse);
                     }
                        break;
                case WeponMode.Sniper:
                    //Якщо не в режимі масштабування, збільште масштаб камери та перейдіть у режим масштабування.
                    if (!ZoomMode)
                    {
                        ironSite.SetActive(true);
                        crossHair.SetActive(false);
                        audioSource.Stop();// зупинити існуючий звук
                        audioSource.clip = zoom;
                        audioSource.Play();
                        Camera.main.fieldOfView = 15f;
                        ZoomMode = true;
                    }
                    else
                    {
                        ironSite.SetActive(false);
                        crossHair.SetActive(true);
                        Camera.main.fieldOfView = 60f;
                        ZoomMode = false;
                    }
                    break;
            }
            
           
        }
        if (Input.GetMouseButtonDown(0)) //Якщо натиснуто ліву кнопку миші...
        {
            if (magazine != 0) {
                if (anim.GetFloat("MoveMotion") == 0)
                {
                    anim.SetTrigger("Attack");
                }
                // запуск звук тригера
                audioSource.Stop();
                audioSource.clip = fire;
                audioSource.Play();
                magazine -= 1;
                magazineText.text = "Bullets: " + magazine;
                //Після створення променя встановлення місця та напрямок запуску.
                Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                //Створить змінну для зберігання інформації про ціль, на яку потрапив промінь.
                RaycastHit hitInfo = new RaycastHit();
                if (Physics.Raycast(ray, out hitInfo))
                {
                    if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                    {
                        EnemyFSM eFSM = hitInfo.transform.GetComponent<EnemyFSM>();
                        eFSM.HitEnemy(weaponPower);
                    }
                    else
                    {
                        //Перемістить розташування ефекту попадання в точку, де зіткнувся промінь
                        bulletEffect.transform.position = hitInfo.point;
                        //Зіставлення напряммку ефекту попадання з вектором точки, куди влучив промінь
                        bulletEffect.transform.forward = hitInfo.normal;
                        ps.Play();
                    }
                }
                StartCoroutine(ShootEffectOn(0.05f));
            } else
            {
                audioSource.Stop();
                audioSource.clip = dryFire;
                audioSource.Play();
            }

        }
        IEnumerator ShootEffectOn(float duration)
        {
            int num = Random.RandomRange(0, eff_Flash.Length);
            eff_Flash[num].SetActive(true);
            yield return new WaitForSeconds(duration);
            eff_Flash[num].SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && wMode == WeponMode.Sniper) //Якщо натиснуто цифрову клавішу 1, режим гранати
        {
            wMode = WeponMode.Normal;
            Camera.main.fieldOfView = 60f;
            wModeImage.sprite = Resources.Load<Sprite>("Image/state_1");
            ironSite.SetActive(false);
            crossHair.SetActive(true);
            audioSource.Stop();
            audioSource.clip = boltUp;
            audioSource.Play();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && wMode == WeponMode.Normal)// Якщо натиснута цифрова клавіша 2, режим прицілювання
        {
            wMode = WeponMode.Sniper;
            wModeImage.sprite = Resources.Load<Sprite>("Image/state_2");
            audioSource.Stop();
            audioSource.clip = boltUp;
            audioSource.Play();
        }
    }
}
