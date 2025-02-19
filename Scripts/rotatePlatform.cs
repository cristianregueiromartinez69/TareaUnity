using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotatePlatform : MonoBehaviour
{
    public float speedRotation = 50.0f;

    void Update()
    {
        transform.Rotate(new Vector3(speedRotation * 2 * Time.deltaTime, 0, 0));
    }
}
