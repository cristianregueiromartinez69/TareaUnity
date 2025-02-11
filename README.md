# PROYECTO DE RALL A BALL. 🎱 HECHO CON UNITY 😄

**Índice**

- Creación y script del jugador.
- Creación y script de las cámaras.
- Creación y script de los coleccionables.
- Creación y script del enemigo.
- Creacion y script de la máquina de estados.
- Diseño de los niveles.


### 1. Creación y script del jugador.

Nuestro jugador es una pelota, así que para eso necesitamos ir a nuestro Unity y hacer lo siguiente:

```bash
#Nos vamos al unity y agregamos una esfera, que es un objeto 3D a nuestra escena
```

![esfera1](https://github.com/user-attachments/assets/2e59ec40-7e18-4836-8514-5e3d61a32503)

```bash
#Ahora tenemos que agregarle 2 componentes para que se mueva, un rigidbody y un player imput
```

**RigidBody**

![esfera2](https://github.com/user-attachments/assets/26597f40-a1ea-449d-b7b2-30c863b27916)

**PlayerInput**

![esfera3](https://github.com/user-attachments/assets/78b6f82d-c394-4dc5-b7a3-80f571c89749)

***RigidBody***

Sirve para que nuestra esfera, adquiera las propiedades de un objeto de la vida real, con sus físicas y reacciones a fuerzas ejercidas sobre este.

***PlayerInput***

Sirve para poder mover a nuestro jugador a través de las teclas del ordenador

```bash
Script del jugador y su explicación de las partes importantes
```
<details>
  <summary>Script PlayerController.cs</summary>

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
</details>

**Cómo movemos al jugador** 🤔

Con esta línea.
```bash
 private Rigidbody rb;
```

y esta, la cual declaramos en el método start.

```bash
rb = GetComponent<Rigidbody>();
```
Ya tenemos un objeto declarado y listo para poder usar los métodos.

Con este método.

```bash
 //metodo para mover al jugador
    void OnMove(InputValue movementValue)
    {

        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;


    }
```

Lo que hacemos es obtener los valores de la izquierda, derecha, arriba y abajo cuando pulsamos las teclas de dirección del ordenador.
Luego lo usamos aquí.

```bash
 Vector3 movement = new Vector3(movementX, 0.0f, movementY);
rb.AddForce(movement * speed);
```
Creamos un vector3 y con ese vector, le aplicamos la fuerza al rigidBody con el metodo addForce. 

**Importante** 😱
Si nos fijamos en el script, tenemos una variable pública que es speed. Esa variables nos va a servir para, desde el unity, poder modificar la velocidad a nuestro gusto sin tener que ir al código expresamente.

### 2. Creación y script de las cámaras. 😄

Ahora vamos con las cámaras. En nuestro juego tenemos un total de 3 cámaras.

<details>
  <summary>camara principal.cs</summary>
   public GameObject player;
    private Vector3 offset;
    void Start()
    {
        offset = transform.position - player.transform.position;

    }

    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
</details>

[camaraNormal.webm](https://github.com/user-attachments/assets/6744fc38-eaca-40b3-ad1e-5a283f0e8c98)


<details>
  <summary>camara primera persona.cs</summary>
   public GameObject player;
    private Vector3 offset;
    void Start()
    {
        offset = transform.position - player.transform.position;

    }

    void LateUpdate()
    {
        float movimientoHorizontal = Input.GetAxis("Horizontal");

        if (movimientoHorizontal != 0)
        {
            transform.Rotate(0, movimientoHorizontal, 0);

        }
        transform.position = player.transform.position + offset;


    }
</details>

[primeraPersona.webm](https://github.com/user-attachments/assets/0b639b82-fa5d-4283-aa28-327b2cb3ca98)


<details>
  <summary>Camara Cenital.cs</summary>
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
</details>

[camaraCenital.webm](https://github.com/user-attachments/assets/9b7e0537-5328-4d23-8684-624bd6932170)

Por último tenemos la camaraManager, que se encarga de según pulses la ***Q*** sucesivamente, aparezca una cámara u otra.

<details>
  <summary>Script camara manager.cs</summary>
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
</details>

[camaraManager.webm](https://github.com/user-attachments/assets/19776306-b430-451b-9cff-aa7147dae500)

**Como funciona el script del camara manager** 🤔

```bash
 if (Input.GetKeyDown(KeyCode.Q))
        {
            cameras[index].SetActive(false);
            index++;
            if(index >= cameras.Length){
                index = 0;
            }
            cameras[index].SetActive(true);
        }
```
1. Establecemos la condicion de que, si el usuario pulsa la letra ***Q***, la cámara actual deja de funcionar
2. Se suma en +1 el índice de cámaras
3. Si el índice es mayor que la longitud del array de cámaras, el índice es 0
4. Se activa la cámara en el índice correspondiente

**Como funciona la cámara de primera persona** 🤔

```bash
 float movimientoHorizontal = Input.GetAxis("Horizontal");

        if (movimientoHorizontal != 0)
        {
            transform.Rotate(0, movimientoHorizontal, 0);

        }
        transform.position = player.transform.position + offset;
```
Con el script anterior, hacemos que la cámara de primera persona, gire solo de izquierda a derecha. Después en el script del jugador hacemos esto.

```bash
 Quaternion cameraRotation = firstPersonCamera.transform.rotation; // esto obtiene la rotacíon de la cámara
Vector3 cameraForward = cameraRotation * Vector3.forward; // esto calcula la dirección hacia adelante de la cámara
Vector3 movementDirection = Vector3.ProjectOnPlane(cameraForward, Vector3.up).normalized; // esto  Proyecta la dirección hacia adelante en el plano horizontal (ignora inclinaciones verticales)
Vector3 movement = movementDirection * movementY; // Crea el vector de movimiento basado en el input y la dirección de la cámara
```
1. Con el Quaternion, obtenemos la rotación de la cámara
2. Cremos un vector3 con la dirección de rotación de la cámara y lo multiplicamos por el Vector3.forward para ir solo hacia adelante
3. Proyectamos la dirección hacia adelante en el plano horizontal
4. Por último, multiplicamos el movementDirection por el movementY, que es cuando el jugador le da a las flechas adelante y atrás y listo, ya tenemos cámara en primera persona.


### 3. Creación y script de los coleccionables. 😄

Para crear los coleccionables, primero tenemos que tenerlos metidos en una carpeta llamada prefabs. Esto porqué 🤔

1. No vamos a hacer 1 solo coleccionable, sino varios
2. Al hacer un prefab de 1 coleccionable y luego duplicarlo, los cambios en el primero, afectan al resto
3. Esto se aplica al script y colores de los mismos

![prefab1](https://github.com/user-attachments/assets/a0a740eb-1af7-4c87-ac54-9de151270f7e)

Para que cuando el jugador toque el pick Up, este desaparezca, tenemos que asignar la ipción isTrigger.

![prefab2](https://github.com/user-attachments/assets/e6134c68-34e8-48f5-8027-d5da543411fb)

Esto hará que aquí.

```bash
 if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            countObjectsInt += 1;
            setCountText();

        }
```

Cuando el jugador toque un objeto cuya etiqueta sea ***Pick Up***, este objeto desaparezca a través del método SetActive(false).

[prefab1.webm](https://github.com/user-attachments/assets/99f0ef3c-699b-4c89-bd45-b860ef916669)

Como vemos en el vídeo anterior, el jugador al tocar un coleccionable, este desaparece.

### 4.  Creación y script del enemigo. 😜

Como vimos en los vídeos, en el juego tenemos 2 enemigos que nos persiguen y si nos tocan, salimos volando a la luna. Para eso tenemos que hacer lo siguiente.

1. Asigarle un agente de navegación al enemigo

![enemy1](https://github.com/user-attachments/assets/9c616e42-bf1d-470e-9442-c6e42e5cd029)

2. Indicar porque áreas tiene que circular

![enemy2](https://github.com/user-attachments/assets/643df985-d551-40cd-9df0-719cb8ccc243)

```bash
script del enemigo
```

<details>
  <summary>Enemy Controller.cs</summary>
  private NavMeshAgent pathFinder;
    private Transform target;



    void Start()
    {
        pathFinder = GetComponent<NavMeshAgent>();
        target = GameObject.Find("Player").transform;
    }
    void Update()
    {
        pathFinder.SetDestination(target.position);
        Debug.Log(target.position);

               
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerTag"))
        {
            // Llamamos al método que aplica el empuje al jugador
            collision.gameObject.GetComponent<PlayerController>().ApplyKnockback();
        }
    }
</details>

En el script anterior, no nos hizo falta poner el transform público, simplemente a través de esta línea.

```bash
target = GameObject.Find("Player").transform;
```
Ya obtenemos el vector de donde está el jugador para que lo persiga.

[enemy1.webm](https://github.com/user-attachments/assets/d75c2683-5d98-4a51-860c-1e8457d61e47)

***Como hacemos la colisión para que el enemigo nos mande a volar***🤔

Eso lo hacemos a través de esta línea en el script del enemigo.

```bash
collision.gameObject.GetComponent<PlayerController>().ApplyKnockback();
```

Cuando el enemigo entra en contacto con un objeto 3D cuya etiqueta sea ***PlayerTag***, llama al método que está en el jugador que es ApplyKnockBack();

```bash
 public void ApplyKnockback()
    {
        if (!soyInmortal)
        {
            Vector3 knockback = new Vector3(0f, knockbackForce, 0f);
            rb.AddForce(knockback, ForceMode.Impulse);
        }

    }
```

Básicamente aplicamos una fuerza sobre el ejeY y nos manda a volar.

### 5. Creacion y script de la máquina de estados. 😄
Ahora tenemos que ponerle una máquina de estados a nuestro jugador. Vamos a definir **5 estados** para nuestro jugador:
- idle -> El jugador está parado
- run -> El jugador está en movimiento
- superSpeed -> El jugador aumenta su velocidad al recoger un número determinado de coleccionables
- invenbile -> El jugador se vuelve invencible y no es afectado por la colisión del enemigo al recoger un coleccioble
- dead -> El jugador muere al perder todas las vidas
  
![estados1](https://github.com/user-attachments/assets/298cf3c7-9c37-43ac-b134-ffb781027b9f)

```bash
#manejo de estado del superSpeed
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
```

En el script lo que hacemos es que si el jugador coge un número de items, en este caso 9 o más, se aumenta la velocidad a 50.
Si no es así, el speed será de 10.

```bash
#manejo de la invencibilidad
else if (other.gameObject.CompareTag("invencible"))
        {
            soyInmortal = true;
            other.gameObject.SetActive(false);
            anim.SetBool("itemInvencible", true);
        }
```

Con el script anterior lo que hacemos es, si el jugador coge el coleccionable con el tag ***invencible*** hacemos 3 cosas:
1. cambiamos un booleano a true
2. quitamos el coleccionable de invencibilidad
3. cambiamos el estado

Como el booleano ***soyInmortal*** ahora es true, se ejecuta esto.

```bash
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
```
Esto lo que hará es un timer de unos segundos, que es lo que durará la invencibilidad. también hacemos lo siguiente:
1. cambiamos los colres de la pelota
2. la colisión del enemigo no nos afecta
3. Tenemos algo más de velocidas


### 6. Diseño de los niveles. 😄

En nuestro juego tenemos un total de **4 niveles**.

***reglas del juego*** 🤗

1. Cada nivel tiene 12 coleccionables
2. El jugador tiene 3 vidas
3. El jugador tiene que coger lso 12 coleccionables de los 4 niveles sin que sus vidan lleguen a 0
4. Si las vidas llegan a 0, el jugador pierde
5. Tenemos 2 enemigos en el juego


#### Nivel 1.

![nivel1](https://github.com/user-attachments/assets/7bc7d456-81e8-4f97-be13-8ff156b0dd99)


#### Nivel 2.

![nivel2](https://github.com/user-attachments/assets/6cc280be-bd71-486b-9c63-091b4f6dcae6)


#### Nivel 3.

![nivel3](https://github.com/user-attachments/assets/efe204ed-6774-48d7-ab5d-30c591b9e723)

#### Nivel 4.

![nivel4](https://github.com/user-attachments/assets/3e262ba9-44c9-449d-89af-15c1da145887)
![nivel4parte2](https://github.com/user-attachments/assets/fd7315c2-8bbb-4cd5-8c58-8b026889e87c)
![nivel4parte3](https://github.com/user-attachments/assets/ca1ddc6e-d7a0-4b28-b608-cd249a622c50)





