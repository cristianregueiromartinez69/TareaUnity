using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemSalto : MonoBehaviour
{
    public float speed = 40;

    void Update()
    {
        transform.Rotate(new Vector3(0.0f, 0.0f, speed * Time.deltaTime));
    }
}
