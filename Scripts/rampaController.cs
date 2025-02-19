using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rampaController : MonoBehaviour
{
    public float propulsion = 200.0f;
  
     void OnCollisionEnter(Collision collision)
    {
        // Verifica si el objeto que colisiona es el jugador
        if (collision.gameObject.CompareTag("PlayerTag"))
        {
            Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();

            if (playerRigidbody != null)
            {
                Vector3 propulsionDirection = transform.right;
                playerRigidbody.AddForce(propulsionDirection * propulsion * 3, ForceMode.Impulse);
            }
        }
    }
}
