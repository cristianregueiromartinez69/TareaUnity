using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class subirBajarPlataformaNivel4 : MonoBehaviour
{


  public float speed = 1.0f;
    private float limitYAbajo = 53.58f;
    private float limitYArriba = 61.042f;
    private bool subiendo = true; 

    void Update()
    {
        Vector3 position = transform.position;

        if (subiendo)
        {
            position.y += speed * Time.deltaTime; 

            if (position.y >= limitYArriba) 
            {
                position.y = limitYArriba; 
                subiendo = false; 
            }
        }
        else
        {
            position.y -= speed * Time.deltaTime; 

            if (position.y <= limitYAbajo) 
            {
                position.y = limitYAbajo; 
                subiendo = true; 
            }
        }
        transform.position = position;
    }

}
