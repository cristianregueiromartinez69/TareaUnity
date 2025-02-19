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

    //variable limite Y para el limite del respawn del player
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

    //variables booleanas para comprobar que el jugador puede o no saltar
    private bool puedoSaltar = false;

    private bool estoyENSuelo = false;

    //variable para establecer el salto del jugador
    public float jump = 1.0f;

    //variable para acceder a los estados del jugador
    private Animator anim;


    //variables de invencible del jugador
    private Renderer coloresPelota;
    public float colorChangeInvencibleInterval = 20.0f;

    private float timerInvencible = 0.0f;

    private Color[] colors = { Color.yellow, Color.green, Color.red, Color.blue, Color.cyan, Color.magenta };
    private int currentColorIndex = 0;

    //variable para saber el estado actual del jugador
    private string currentStateName;




    /**
    Metodo que se ejecuta una sola vez
    Nos sirve para iniciar los objetos y para establecer los valores de:
    1, la vida del jugador, que tiene 3 
    2. EL numero de objetos cogidos, que de momento son 0
    3. el nivel actual del jugador que de momento es 1
    */
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
        /*
        Comprobamos que está activa la primera persona o la cámara cenital
        */

        if (cameraNormal.activeSelf || cenitalCamera.activeSelf)
        {
            Vector3 movement = new Vector3(movementX, 0.0f, movementY);
            //comprobamos que el nombre del estado actual no es dead, porque si es así, el jugador no se moverá ya que perdió
            if (currentStateName != "dead")
            {
                Vector3 dir = Vector3.zero;
                dir.x = Input.acceleration.x;
                dir.z = Input.acceleration.y;
                if (dir.sqrMagnitude > 1)
                    dir.Normalize();

                dir *= Time.deltaTime;
                transform.Translate(dir * speed, Space.World);


            }
        }

        //si está activada la primera persona, entra aquí
        if (firstPersonCamera.activeSelf)
        {
            Debug.Log("hola, entre en la primera persona");


            Quaternion cameraRotation = firstPersonCamera.transform.rotation; // esto obtiene la rotacíon de la cámara


            Vector3 cameraForward = cameraRotation * Vector3.forward; // esto calcula la dirección hacia adelante de la cámara


            Vector3 movementDirection = Vector3.ProjectOnPlane(cameraForward, Vector3.up).normalized; // esto  Proyecta la dirección hacia adelante en el plano horizontal (ignora inclinaciones verticales)


            Vector3 movement = movementDirection * movementY; // Crea el vector de movimiento basado en el input y la dirección de la cámara


            if (currentStateName != "dead")
            {
                Vector3 dir = Vector3.zero;
                dir.x = -Input.acceleration.y;
                dir.z = Input.acceleration.x;
                if (dir.sqrMagnitude > 1)
                    dir.Normalize();

                dir *= Time.deltaTime;
                transform.Translate(dir * speed, Space.World);


            }
        }

        /*
        Llamamos al metodo que obtiene la base de todos los estados del jugador
        Después llamamos a otro método que nos devuelve el estado actual del jugador
        */
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        currentStateName = returnNameState(stateInfo);

        setCountText(); //metodo que establece el numero de objetos pillados y lo pone en la UI
        setLevelText(); //metodo que establece el numero del nivel del jugador y lo pone en la UI
        setLifeText(); //metodo que establece las vidas del jugador y lo pone en la UI
        resetFallPlayer(); //metodo que resetea la posicion del jugador en caso de pasar el limiteY definido arriba
        changeLevel(); //metodo que cambia la posicion del jugador y de los enemigos en función del nivel en el que está
        powerUpSalto(estoyENSuelo, puedoSaltar); //metodo que hace que el jugador pueda o no saltar dependiendo de si está o no en el suelo
        mostrarTextoMuerte(); //método que muestra el texto de muerte cuando el jugador tiene 0 vidas
        playerMoviendose(); //metodo que cambia el estado del jugador de Idle a run
        superSpeed(); //metodo que cambia el estado del jugador de run a superSpeed
        Debug.Log("Estado actual: " + currentStateName);
        selectState(currentStateName); //metodo que, en función del estado delm jugador, hará métodos diferentes

    }


    //metodo que actúa cuando entramos en el area de un objeto
    void OnTriggerEnter(Collider other)
    {
        //si entramos en un coleccionable, este desaparece y sumamos en +1 los coleccionables
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            countObjectsInt += 1;
            setCountText();

        }

        //si pillamos el item para saltar, este desaparece y ya podemos saltar, siempre y cuando esté el jugador en el suelo
        else if (other.gameObject.CompareTag("salto"))
        {
            other.gameObject.SetActive(false);
            puedoSaltar = true;
        }

        //si pillamos el item de invencibilidad, este desaparece y durante unos segundos, somos invencibles
        else if (other.gameObject.CompareTag("invencible"))
        {
            anim.SetBool("itemInvencible", true);
            other.gameObject.SetActive(false);
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


    //metodo que aplica la fuerza al player cuando es tocado por el enemigo
    public void ApplyKnockback()
    {
        if (currentStateName != "invencible")
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

    /**
    Metodo que resetea la posición del jugador en función del nivel en el que está
    Tanbién restamos 1 vida al jugador cada vez que este reaparece.
    */
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

    /**
    Metodo para hacer la lógica del salto del jugador:
    1. Si el jugador está en el suelo y el item está pillado
    2. Tiene que pulsar la tecla de espacio y el jugador dará un salto
    3. Para sto, tenemos que definir en Unity, los objetos 3D que serán considerados planos para saltar
    */
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

    /**
    Metodo para saber su el jugador está o no en el suelo
    Si colisiona con un objeto con etiqueta (ground) el jugador estará en el suelo y podrá saltar
    */
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            estoyENSuelo = true;
        }

    }

    /**
    Metodo para saber su el jugador ha salido del suelo
    Si sale de la colisión  de un objeto con etiqueta (ground), el jugador no podrá saltar hasta pisar el suelo de nuevo
    */
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            estoyENSuelo = false;
        }
    }

    /**
    Metodo que cambia el estado a (dead) del jugador siempre y cuando las vidas del enemigo sean 0
    */
    void mostrarTextoMuerte()
    {
        if (countLifeInt <= 0)
        {
            anim.SetBool("muerto", true);

        }
        else
        {
            textoMuerteVisible(false);
        }
    }

    /**
    Metodo que jhace visible el texto de muerte del jugador al morir
    */
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

    /**
    Método que activa el estado de super velocidad al tener 9 o más coleccionables
    */
    void superSpeed()
    {
        if (countObjectsInt >= 9)
        {
            anim.SetBool("items6", true);
        }
        else
        {
            anim.SetBool("items6", false);
        }
    }



    /**
    metodo de inmortalidad del jugador:
    1. Comprobamos que el objeto que accede a los materiales del jugador no es null
    2. cambiamos los colores del jugador muy rápido
    3. comprobamos que no se sale del tamaño del array
    4. si el tiempo llega a 0, restauramos el estado a run y restauramos el color del jugador a rojo
    */
    void accionInmortal()
    {
        if (coloresPelota != null)
        {
            coloresPelota.material.color = colors[currentColorIndex];
            currentColorIndex = (currentColorIndex + 1) % colors.Length;
        }
        timerInvencible += Time.deltaTime;
        if (timerInvencible >= colorChangeInvencibleInterval)
        {
            speed = 10;
            anim.SetBool("itemInvencible", false);
            coloresPelota.material.color = colors[2];
            timerInvencible = 0;
        }

    }

    /**
    método que cambia el color del jugador a rojo y establece la velocidad a 10
    */
    void accionRun()
    {
        coloresPelota.material.color = colors[2];
        speed = 10.0f;
    }

    /**
        método que cambia el color del jugador a azul y establece la velocidad a 50
    */
    void accionSuperSpeed()
    {
        coloresPelota.material.color = colors[3];
        speed = 50.0f;
    }


    /**
    Metodo que se ejecuta cuando el jugador está en estado muerte
    1. Se muestra el texto de muerte
    2. El jugador no puede moverse
    */
    void accionMuerte()
    {
        textoMuerteVisible(true);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    /**
    metodo que, en función del estado del jugador, hace metodos diferentes
    */
    void selectState(string state)
    {
        switch (state)
        {
            case "run":
                accionRun();
                break;
            case "superSpeed":
                accionSuperSpeed();
                break;
            case "invencible":
                accionInmortal();
                break;
            case "dead":
                accionMuerte();
                break;

        }
    }

    /**
    Metodo para saber el estado actual del jugador
    El método debe de ser llamado en el Update para saber el estado en cada frame
    */
    string returnNameState(AnimatorStateInfo stateInfo)
    {
        string currentStateName = stateInfo.IsName("idle") ? "idle" :
          stateInfo.IsName("run") ? "run" :
          stateInfo.IsName("superSpeed") ? "superSpeed" :
          stateInfo.IsName("invencible") ? "invencible" :
          stateInfo.IsName("dead") ? "dead" :
          "Otro estado";

        return currentStateName;
    }




}
