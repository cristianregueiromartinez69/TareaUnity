using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotatorLevel3 : MonoBehaviour
{
    public float speedRotate = 190.0f;
 

    void Update()
    {
        transform.Rotate(new Vector3(0, speedRotate * 3, 0)* Time.deltaTime);
    }
}
