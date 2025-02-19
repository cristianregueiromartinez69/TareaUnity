using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraControllerCenital : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed = 10.0f;

    void Start()
    {

    }
    void Update()
    {

        transform.RotateAround(target.position, Vector3.up, rotationSpeed * Time.deltaTime);

        transform.LookAt(target);

    }

   
}

