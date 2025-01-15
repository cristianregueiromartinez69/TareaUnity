using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyController : MonoBehaviour
{
    private NavMeshAgent pathFinder;
    private Transform target;



    void Start()
    {
        pathFinder = GetComponent<NavMeshAgent>();
        target = GameObject.Find("Player").transform;
    }
    void Update()
    {
        pathFinder.SetDestination(target.position);
        Debug.Log(target.position);

               
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerTag"))
        {
            // Llamamos al método que aplica el empuje al jugador
            collision.gameObject.GetComponent<PlayerController>().ApplyKnockback();
        }
    }

}
