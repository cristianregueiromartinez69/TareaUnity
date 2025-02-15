using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using System.Linq;
using System.Threading;

public class PlayerController : MonoBehaviour
{
    //objeto rigidBody para mover la esfera
    private Rigidbody rb;


    //variable de velocidad para el movimiento de la esfera
    public float speed = 10.0f;

    //variable de velocidad para el movimiento del eje x
    private float movementX;

    //variable de velocidad para el movimiento del eje y

    private float movementY;

    //variable para actualizar en la UI los items que ha cogido el jugador
    public int countObjectsInt;

    //variable para actualizar las vidas del jugador en la UI
    private int countLifeInt = 3;


    private float limitY = -100f;

    //variable para actualizar el nivel del jugador en la UI
    private int countLevelInt = 1;

    //variables para acceder a los textos de la UI
    public Text countObjectsText;
    public Text countLifeText;
    public Text countLevelsText;
    public Text deadText;

    //Items totales ha coger en los niveles
    public const int totalItems = 12;

    //color de los items de la UI
    private Color greenColor = Color.green;
    private Color redColor = Color.red;




    //empujon del enemigo
    public float knockbackForce = 0.001f;

    //referencia al objeto del enemigo
    public GameObject[] enemy;


    //variables públicas que son las camaras
    public GameObject cameraNormal;
    public GameObject firstPersonCamera;
    public GameObject cenitalCamera;

    private bool puedoSaltar = false;

    private bool estoyENSuelo = false;

    public float jump = 1.0f;

    private Animator anim;


    //variables invencibilad
    private Renderer coloresPelota;
    public float colorChangeInvencibleInterval = 20.0f;

    private float timerInvencible = 0.0f;

    private Color[] colors = { Color.yellow, Color.green, Color.red, Color.blue, Color.cyan, Color.magenta };
    private int currentColorIndex = 0;

    private bool soyInmortal = false;




    //metodo que sirve para iniciar estados de objetos
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        coloresPelota = GetComponent<Renderer>();
        if (coloresPelota == null)
        {
            Debug.LogError("No se encontró un Renderer en el GameObject.");
        }
        countObjectsInt = 0;
        countLevelInt = 1;
        countLifeInt = 3;

        setCountText();
        setLevelText();
        setLifeText();
        anim = GetComponent<Animator>();

    }




    //metodo para mover al jugador
    void OnMove(InputValue movementValue)
    {

        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;


    }


    //metodo que se llama en el transcurso de cada frame
    private void FixedUpdate()
    {
        if (cameraNormal.activeSelf || cenitalCamera.activeSelf)
        {
            Debug.Log("hola, entre en la camera normal");
            Vector3 movement = new Vector3(movementX, 0.0f, movementY);



            rb.AddForce(movement * speed);
        }
        Debug.Log("Estado de la camara normal" + cameraNormal.activeSelf);
        Debug.Log("Estado de primera persona: " + firstPersonCamera.activeSelf);
        if (firstPersonCamera.activeSelf)
        {
            Debug.Log("hola, entre en la primera persona");


            Quaternion cameraRotation = firstPersonCamera.transform.rotation; // esto obtiene la rotacíon de la cámara


            Vector3 cameraForward = cameraRotation * Vector3.forward; // esto calcula la dirección hacia adelante de la cámara


            Vector3 movementDirection = Vector3.ProjectOnPlane(cameraForward, Vector3.up).normalized; // esto  Proyecta la dirección hacia adelante en el plano horizontal (ignora inclinaciones verticales)


            Vector3 movement = movementDirection * movementY; // Crea el vector de movimiento basado en el input y la dirección de la cámara


            rb.AddForce(movement * speed);
        }

        setCountText();
        setLevelText();
        setLifeText();
        setColorText(countObjectsInt);
        resetFallPlayer();
        changeLevel();
        powerUpSalto(estoyENSuelo, puedoSaltar);
        mostrarTextoMuerte();
        playerMoviendose();
        superSpeed();

        if (soyInmortal)
        {
            timerInvencible += Time.deltaTime;
            changeColorPelotaInmortal();
            if (timerInvencible >= colorChangeInvencibleInterval)
            {
                speed = 30;
                anim.SetBool("itemInvencible", false);
                soyInmortal = false;
                coloresPelota.material.color = colors[2];

            }
        }


    }


    //metodo que actúa cuando entramos en el area de un objeto
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            countObjectsInt += 1;
            setCountText();

        }
        else if (other.gameObject.CompareTag("salto"))
        {
            other.gameObject.SetActive(false);
            puedoSaltar = true;
        }
        else if (other.gameObject.CompareTag("invencible"))
        {
            soyInmortal = true;
            other.gameObject.SetActive(false);
            anim.SetBool("itemInvencible", true);
        }

    }

    //metodo para sumar los items recogidos y actualizar la UI
    void setCountText()
    {
        countObjectsText.text = "Items: " + countObjectsInt.ToString() + "/12";
    }

    //metodo para restar las vidas del jugador y actualizar la UI
    void setLifeText()
    {
        countLifeText.text = "Vidas: " + countLifeInt.ToString() + "/3";
    }

    //metodo para sumar niveles y actualizar la UI
    void setLevelText()
    {
        countLevelsText.text = "Level: " + countLevelInt.ToString() + "/4";

    }

    //metodo que actualiza el color de la UI de los items al llegar a 12 de 12
    void setColorText(int numberItem)
    {
        if (numberItem >= totalItems)
        {
            countObjectsText.color = greenColor;
        }
    }

    //metodo que aplica la fuerza al player cuando es tocado por el enemigo
    public void ApplyKnockback()
    {
        if (!soyInmortal)
        {
            Vector3 knockback = new Vector3(0f, knockbackForce, 0f);
            rb.AddForce(knockback, ForceMode.Impulse);
        }

    }

    void changeLevel()
    {
        /*
            Explicacion:
            1. Para cambiar de niveles, verificamos si has cogido todos los items
            2. si es así el nivel le sumamos un +1
            3. luego vamos cambiando de niveles dependiendo de el valor del nivel
            4. en cada nivel, hacemos reseteo de items encontrados, posicion del jugador y del enemigo
            5. para el enemigo, tenemos que deshabilitar al agente ya que si no bloquea la posicion y no nos deja moverlo
        */
        if (countObjectsInt == totalItems && countLevelInt < 4)
        {
            countLevelInt += 1;

            // Se recorre cada enemigo y se le deshabilita/habilita su NavMeshAgent
            for (int i = 0; i < enemy.Length; i++)
            {
                NavMeshAgent agent = enemy[i].GetComponent<NavMeshAgent>();  // Obtener el agente para cada enemigo

                // Esto asegura que el agente de cada enemigo sea deshabilitado y habilitado correctamente
                if (agent != null)
                {
                    agent.enabled = false;
                    if (countLevelInt == 2)
                    {
                        enemy[i].transform.position = new Vector3(68.23f, 3.676f, 0.768f);
                        countObjectsInt = 0;

                    }
                    if (countLevelInt == 3)
                    {
                        enemy[i].transform.position = new Vector3(54.18f, 3.69f, -136.6f);
                        countObjectsInt = 0;

                    }
                    if (countLevelInt == 4)
                    {
                        enemy[i].transform.position = new Vector3(2.64f, 3.81f, -255.7f);
                        countObjectsInt = 0;

                    }
                    agent.enabled = true;
                }
            }

            // Cambiar la posición del jugador y las cámaras según el nivel
            if (countLevelInt == 2)
            {
                transform.position = new Vector3(77.02f, 3.79f, -0.889570f);
                firstPersonCamera.transform.position = new Vector3(77.02f, 3.79f, -0.6f);
                cenitalCamera.transform.position = new Vector3(77.02f, 10.92f, -13.29f);
            }
            if (countLevelInt == 3)
            {
                transform.position = new Vector3(38.99f, 3.79f, -151.89f);
                firstPersonCamera.transform.position = new Vector3(38.99f, 3.79f, -151.89f);
                cenitalCamera.transform.position = new Vector3(45.08f, 16.01f, -157.08f);
            }
            if (countLevelInt == 4)
            {
                transform.position = new Vector3(-5.47f, 3.79f, -262.81f);
                firstPersonCamera.transform.position = new Vector3(-5.47f, 3.79f, -262.81f);
                cenitalCamera.transform.position = new Vector3(1.3f, 14.9f, -268.4f);
            }


        }
    }


    void resetFallPlayer()
    {
        if (transform.position.y < limitY && countLevelInt == 1)
        {
            countLifeInt -= 1;
            transform.position = new Vector3(-1.723833f, 3.79f, -0.8895702f);
        }
        else if (transform.position.y < limitY && countLevelInt == 2)
        {
            countLifeInt -= 1;
            transform.position = new Vector3(77.02f, 3.79f, -0.889570f);
        }
        else if (transform.position.y < limitY && countLevelInt == 3)
        {
            countLifeInt -= 1;
            transform.position = new Vector3(38.99f, 3.79f, -151.89f);
        }
        else if (transform.position.y < limitY && countLevelInt == 4)
        {
            countLifeInt -= 1;
            transform.position = new Vector3(-5.47f, 3.79f, -262.81f);
        }
    }

    void powerUpSalto(bool tocarSuelo, bool itemCogido)
    {
        if (tocarSuelo && itemCogido)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 vectorSalto = new Vector3(0f, jump * 0.5f, 0f);
                rb.AddForce(vectorSalto, ForceMode.VelocityChange);
            }


        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            estoyENSuelo = true;
        }

    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            estoyENSuelo = false;
        }
    }

    void mostrarTextoMuerte()
    {
        if (countLifeInt <= 0)
        {   
            anim.SetBool("muerto", true);
            textoMuerteVisible(true);
            gameObject.SetActive(false);

        }
        else
        {
            textoMuerteVisible(false);
        }
    }

    void textoMuerteVisible(bool visible)
    {
        deadText.gameObject.SetActive(visible);
    }

    //Comprobar la velocidad del player

    void playerMoviendose()
    {
        if (rb.velocity.magnitude == 0)
        {
            anim.SetBool("corriendo", false);
        }
        else
        {
            anim.SetBool("corriendo", true);
        }
    }

    void superSpeed()
    {
        if (countObjectsInt >= 9)
        {
            anim.SetBool("items6", true);
            speed = 50.0f;
        }
        else
        {
            anim.SetBool("items6", false);
            speed = 10.0f;
        }
    }

    

    void changeColorPelotaInmortal()
    {
        if (coloresPelota != null)
        {
            coloresPelota.material.color = colors[currentColorIndex];
            currentColorIndex = (currentColorIndex + 1) % colors.Length;
        }

    }




}
