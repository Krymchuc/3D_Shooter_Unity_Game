using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;


public class EnemyFSM : MonoBehaviour
{
    //Константа стану противника
    enum EnemyState
    {
        Idle, Move, Attack, Return, Damaged, Die
    }
    EnemyState m_State;
    public float findDistance = 30f; // діапазон виявлення гравця
    Transform player;
    public float attackDistance = 2f;
    public float moveSpeed = 5f;
    CharacterController cc;
    float currentTime = 0; 
    float attackDelay = 2f; 
    public int attackPower = 3;
    Vector3 originPos; // змінна для збереження початкової позиції
    Quaternion originRot; 
    public float moveDistance = 500f; //максимальна дальність руху (переслідування до смерті)
    public int hp = 15;
    public int maxHp = 15;// початкова витривалість
    public Slider hpSlider;
    Animator anim;
    NavMeshAgent smith;
    public GameObject bloodFX;
    public AudioClip dead;
    AudioSource audioSource;
    public GameObject magazine;
    // Start is called before the first frame update
    void Start()
    {
        m_State = EnemyState.Idle;
        player = GameObject.Find("Player").transform; // Отримати компонент трансформації гравця
        cc = GetComponent<CharacterController>();
        originPos = transform.position;
        originRot = transform.rotation;
        anim = transform.GetComponentInChildren<Animator>();
        smith = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_State)
        {
           case EnemyState.Idle: Idle(); break;
           case EnemyState.Move: Move(); break;
           case EnemyState.Attack: Attack(); break;
           case EnemyState.Return: Return(); break;
           case EnemyState.Damaged: //Damaged();
                break;
           case EnemyState.Die:// Die();
                break;
                 
        }
        hpSlider.value = (float)hp / (float)maxHp;
    }

    void Idle()
    {
        // Якщо відстань до гравця в межах діапазону, перейде у стан Move
        if (Vector3.Distance(transform.position, player.position) < findDistance)
        {
            m_State = EnemyState.Move;
            print("state transition: Idle->Move");
            //Перехід до рухомої анімації
            anim.SetTrigger("IdleToMove");
        }
    }
    void Move()
    {
        if (Vector3.Distance(transform.position, originPos) > moveDistance)
        {
            m_State = EnemyState.Return;
            print("state transition: Move -> Return");
        }
        //Якщо відстань від гравця виходить за межі атаки, рухайтеся до гравця
        else if (Vector3.Distance(transform.position, player.position) > attackDistance)
        {

            smith.isStopped = true; //Ініціалізація маршруту шляхом зупинки руху навігаційного агента.
            smith.ResetPath();
            smith.stoppingDistance = attackDistance; // Встановіть мінімальну відстань для наближення за допомогою навігації як відстань, яку можна атакувати.
            try
            {
                smith.destination = player.position; // Встановити пункт призначення навігації на місцезнаходження гравця.
            } catch (UnityException e)
            {
                smith.transform.position = originPos;
            }
        }
        else // Якщо в радіусі 2 м... атакувати
        {
            m_State = EnemyState.Attack;
            print("state transition:Move->Attack");
            currentTime = attackDelay;
            anim.SetTrigger("MoveToAttackDelay");
        }
    }
    void Attack()
    {
        //Якщо в межах 2м
        if (Vector3.Distance(transform.position, player.position) < attackDistance)
        {
            currentTime += Time.deltaTime;
            if (currentTime > attackDelay) // атакувати кожні 2 секунди
            {
            //    player.GetComponent<PlayerMove>().DamageAction(attackPower);
                print("attack");
                currentTime = 0;
                anim.SetTrigger("StartAttack");
            }
        }
        else// якщо він знаходиться на відстані 2 м
        {
            m_State = EnemyState.Move;
            print("state transition: Attack->Move");
            currentTime = 0;
            anim.SetTrigger("AttackToMove");
        }
    }
    public void AttackAction()
    {
        player.GetComponent<PlayerMove>().DamageAction(attackPower);
    }
    void Return()
    {
        //Рух до початкової позиції, якщо відстань від початкової позиції перевищує 0,1f.
        if (Vector3.Distance(transform.position, originPos) > 0.1f)
        {
            /*
            Vector3 dir = (originPos - transform.position).normalized;
            cc.Move(dir * moveSpeed * Time.deltaTime);
            transform.forward = dir;
            */
            smith.destination = originPos; // Встановити пункт призначення навігації на початково збережене місце
            smith.stoppingDistance = 0; // Встановити мінімальну відстань, на яку наближається навігація, на «0»
        }
        //Якщо ні, установиться початкове положення та переведеться поточний стан у режим очікування
        else
        {
            smith.isStopped = true; // Зупинити рух навігаційного агента та ініціалізувати маршрут.
            smith.ResetPath();
            //Перетворення значення позиції та значення обертання в початковий стан.
            transform.position = originPos;
            transform.rotation = originRot;
            hp = maxHp;
            m_State = EnemyState.Idle;
            print("state transition: Return -> Idle");
            anim.SetTrigger("MoveToIdle");
        }
    }
    void Damaged()
    {
        StartCoroutine(DamageProcess());
    }
    IEnumerator DamageProcess()
    {
        yield return new WaitForSeconds(1.0f);

        m_State = EnemyState.Move;
        print("state transition : Damaged -> Move");
    }
    public void HitEnemy(int hitPower)
    {
        if (m_State == EnemyState.Damaged || m_State == EnemyState.Die || 
            m_State == EnemyState.Return)
        {
            return;
        }


        hp -= hitPower; // Ініціалізація маршруту шляхом зупинки руху навігаційного агента.
        smith.isStopped = true;
        smith.ResetPath();
        if (hp > 0)
        {
            m_State = EnemyState.Damaged;
            print("state transition: Any State -> Damaged");
            anim.SetTrigger("Damaged");
            Damaged();
        }
        else {
            m_State = EnemyState.Die;
            print("state transition : Any State -> Die");
            anim.SetTrigger("Die");
            Die();
        }
    }
    void Die()
    {
        StopAllCoroutines();
        StartCoroutine(DieProcess());
    }
    IEnumerator DieProcess()
    {
        cc.enabled = false;
        //кров
        GameObject Blood = Instantiate(bloodFX, new Vector3(transform.position.x, Terrain.activeTerrain.SampleHeight(transform.position), transform.position.z - 1f), Quaternion.Euler(-90, 0, 0));
        audioSource.clip = dead;
        audioSource.Play();
        Score.score += 5;
        yield return new WaitForSeconds(2f);
        print("extinction");
        Destroy(Blood);
        Destroy(gameObject);
        System.Random random = new System.Random();

        // скинути патрони
        int ran = random.Next(1, 3);
        print(ran);
        if (ran == 2) { GameObject Magazine = Instantiate(magazine, new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z), new Quaternion(0, 0, 0, 0)); }
    }
}
