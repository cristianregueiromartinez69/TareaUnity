using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cameramanager : MonoBehaviour
{
    public GameObject[] cameras;
    private int index = 0;
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            cameras[index].SetActive(false);
            index++;
            if(index >= cameras.Length){
                index = 0;
            }
            cameras[index].SetActive(true);
        }
    }
}
