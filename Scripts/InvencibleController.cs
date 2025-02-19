using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvencibleController : MonoBehaviour
{
    public float speed = 50;
    public float colorChangeInterval = 0.3f; 
    private Renderer objRenderer;
    private Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow, Color.magenta, Color.cyan };
    private int currentColorIndex = 0;
    private float timer = 0f;

    void Start()
    {
        objRenderer = GetComponent<Renderer>();
        if (objRenderer == null)
        {
            Debug.LogError("No se encontró un Renderer en el GameObject.");
        }
    }

    void Update()
    {
        transform.Rotate(new Vector3(0.0f, 0.0f, speed * Time.deltaTime));
        timer += Time.deltaTime;
        if (timer >= colorChangeInterval)
        {
            ChangeColor();
            timer = 0f;
        }
    }
     void ChangeColor()
    {
        if (objRenderer != null)
        {
            objRenderer.material.color = colors[currentColorIndex];
            currentColorIndex = (currentColorIndex + 1) % colors.Length;
        }
    }
}
