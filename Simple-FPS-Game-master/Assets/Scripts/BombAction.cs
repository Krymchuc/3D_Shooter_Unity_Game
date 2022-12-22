using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAction : MonoBehaviour
{
    public GameObject bombEffect;
    public int attackPower = 10; // пошкодження гранатами
    public float explosionRadius = 5f; // радіус дії вибуху
    private void OnCollisionEnter(Collision collision)
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, explosionRadius, 1 << 10);

        for (int i = 0; i < cols.Length; i++)
        {
            cols[i].GetComponent<EnemyFSM>().HitEnemy(attackPower);
        }

        GameObject eff = Instantiate(bombEffect); // створити префаб вибуху
        eff.transform.position = transform.position;  
        Destroy(gameObject); // Знищити об'єкт Bomb
    }
}
