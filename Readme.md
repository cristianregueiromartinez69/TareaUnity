### TAREA UNITY. ROLL A BALL 😄

**Índice**

1. Creación del proyecto, plano del juego y personalización de obstáculos
2. Creación del jugador y movimiento
3. Configuración de camara y coleccionables
4. Enemigo del juego
5. Planteamiento de más niveles y funcionalidades añadidas 

### 1. Creación del proyecto y plano del juego 😄
Lo primero de todo sería crear el proyecto en Unity, para eso tenemos que irnos a Unity Hub y descargar la versión que vamos a utilizar, que es la **2022.3.50f1**.
Una vez descargada, pasamos a crear el proyecto.

![Introduccion](https://github.com/user-attachments/assets/94ac99c2-065d-4359-8b4e-cd229ebe3f75)


```bash
#le damos a create new proyect, escogemos un nombre y ubicación de donde estará el proyecto
```

El juego se divide en escenas, al crear el proyecto se crea una escena por defecto: cambiamos el nombre a la escena y empezamos a crear el plano donde se va a desarrollar nuestro roll a ball.

**Creación del plano**😎
```bash
En nuestra escena vamos a hacer el suelo mediante un objeto 3D llamado Plane, el cual va a recibir una scale determinado para hacerlo más grande
```

![plano](https://github.com/user-attachments/assets/11e7d303-9cdf-4a73-9da7-7ecc30d31a15)


**Personalización de obstáculos** 😃
Ya tenemos hecho el suelo, ahora ponemos obstáculos para que sea algo complicado coger los coleccionables

```bash
Para ganar, hay que coger 12 items y nos persigue un enemigo, el cual hablaremos más adelante
```

Para crear los obstáculos, simplemente hacemos lo siguiente:
- Creamos un objeto vacío llamado obstaculos.
- Dentro de este vamos a crear muchos cubos que es el objeto 3D usado para los coleccionables.
- Los cubos van a ser personalizados para formar unas paredes y laberintos.
- El jugador no puede atravesarlos y debe de encontrar los coleccionables.



```bash
Estos obstáculos son suficientes para el primer nivel del juego
```


### 2. Creación del jugador y movimiento 😄
Tenemos que tener a nuestro jugador el cúal es una bola que va rodando cogiendo coleccionables. Para eso hacemos lo siguiente:

- Creamos un objeto 3D que es una sphere
- le damos las dimensiones que queremos y la ponemos en el plano

![sphere1](https://github.com/user-attachments/assets/a2c480ec-ecf2-4bb8-99de-d386335c79b8)

 
- Añadimos un Rigidbody a la sphere para que se comporte como un objeto esfera de la vida real

![sphere1](https://github.com/user-attachments/assets/e201338b-14bf-4681-b3af-93a20bd5fd10)

  
- A través de script, manejamos el movimiento del personaje 🤗

```bash
#script del personaje
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.AI;

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


    private float limitY = -200f;

    //variable para actualizar el nivel del jugador en la UI
    private int countLevelInt = 1;

    //variables para acceder a los textos de la UI
    public Text countObjectsText;
    public Text countLifeText;
    public Text countLevelsText;

    //Items totales ha coger en los niveles
    public const int totalItems = 12;

    //color de los items de la UI
    private Color greenColor = Color.green;
    private Color redColor = Color.red;




    //empujon del enemigo
    public float knockbackForce = 0.001f;

    //referencia al objeto del enemigo
    public GameObject enemy;






    //metodo que sirve para iniciar estados de objetos
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        countObjectsInt = 0;
        countLevelInt = 1;
        countLifeInt = 3;
        setCountText();
        setLevelText();
        setLifeText();
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
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);

        rb.AddForce(movement * speed);
        setCountText();
        setLevelText();
        setLifeText();
        setColorText(countObjectsInt);
        resetFallPlayer();
        changeLevel();
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
        countLevelsText.text = "Level: " + countLevelInt.ToString() + "/5";

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
        Vector3 knockback = new Vector3(0f, knockbackForce, 0f);
        rb.AddForce(knockback, ForceMode.Impulse);
    }

    //metodo para cambiar de niveles
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
        if (countObjectsInt == totalItems)
        {
            countLevelInt += 1;
            if (countLevelInt == 2)
            {
                transform.position = new Vector3(77.02f, 3.79f, -0.889570f);

                NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.enabled = false;
                    enemy.transform.position = new Vector3(68.23f, 3.676f, 0.768f);
                    agent.enabled = true;
                    countObjectsInt = 0;
                    countObjectsText.color = redColor;
                }
            }
            if (countLevelInt == 3)
            {
                transform.position = new Vector3(38.99f, 3.79f, -151.89f);

                 NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.enabled = false;
                    enemy.transform.position = new Vector3(54.18f, 3.69f, -136.6f);
                    agent.enabled = true;
                    countObjectsInt = 0;
                    countObjectsText.color = redColor;
                }

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
    }

}
```
 
- Para moverlo tenemos que ponerle unos Input settings





```bash
Hecho todo lo anterior, el personaje se podrá mover sin problemas
```
![obstaculos](https://github.com/user-attachments/assets/0d4382d9-9f5e-4f2d-be9d-a10654446fbc)

### 3. Configuración de camara y coleccionables 😄
Vamos a hacer que la camara siga al jugador y los coleccionables

```bash
#para que la cámara siga al jugador, debemos de copiar y pegar este script
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
//El objeto que será el jugador
 public GameObject player;

//varibale de tipo Vector3 para medir la distancia entre la cámara y el jugador
 private Vector3 offset;

 void Start()
    {
        offset = transform.position - player.transform.position; 
    }

 void LateUpdate()
    {
        transform.position = player.transform.position + offset;  
    }
}
```

Luego debemos de hacer lo siguiente: :hushed:
- Nos vamos a la camara
- Hemos establecido una variable pública en el script, así que es accesible desde fuera del script
- Esa variable la vamos a hacer igual al objeto 3D del player

![camera](https://github.com/user-attachments/assets/fb6daa8a-8e0d-481c-b3f4-1faaa583afc5)



```bash
Ahora la cámara va a seguir al jugador sin problemas
```

![camera2](https://github.com/user-attachments/assets/562a3bac-67ae-4ce6-a2f5-c19a2fe5b5a4)

**Coleccionables** 🤗
Para hacer coleccionables sencillos tenemos que hacer lo siguiente:

- Creamos un objeto que será el coleccionable, un objeto 3D que es un cubo
- creamos una carpeta prefab y añadimos el objeto coleccionable a ella
- Duplicamos el objeto las veces que queramos por numero de coleccionables a meter en el juego

**Parte más importante** 😱
dejar comfirmada la opción IsTrigger para detectar si el jugador entra en el area del coleccionable

![coleccionable1](https://github.com/user-attachments/assets/e5be5be1-9509-4205-9194-98cade9463dc)

- Luego en el script que pusimos arriba, el del jugador, en la funcion OnTriggerEnter, detectamos el objeto y lo eliminamos

```bash
#pequeña demo, segun cogemos coleccionables, la UI se actualiza, fijaros arriba a la izquierda
```


![coleccionables2](https://github.com/user-attachments/assets/aee07758-1312-4141-aea4-5b7fb482958f)

### 4. Enemigo del juego 😱

Para hacer el enemigo, podéis ir a los apuntes de damian que explica bien lo que hay que hacer para manejar el agente y los navMeshAgents, yo diré como lo he creado y un par de ajustes

- Diseño del enemigo
![enemigo1](https://github.com/user-attachments/assets/c3775034-f7a2-45cd-882b-f95f70036610)

```bash
Como se puede observar, son muchos objetos 3D metidos en un objeto que es una capsula.
Cuando la capsula la movamos a traves del script, todos los objetos hijos del cilindro se van a mover con el
```

- Script del enemigo

```bash
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyController : MonoBehaviour
{
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

}
```

```bash
#demo para demostrar que el enemigo nos sigue y que cuando colisionamos con él, le añadí la funcionalidad de que me levante hacia arriba
```

![enemigo2](https://github.com/user-attachments/assets/89710b96-e3cb-4475-9287-a8b20b2012ea)




### 5. Planteamiento de más niveles y funcionalidades añadidas 🤗

El roll a ball está hecho, pero añadí como ya se mostró anteriormente, obstáculos para hacer el recorrido más interactivo. Además de eso, añadimos lo siguiente:
- respawn

```bash
#como ya tenéis en el script del jugador, declaramos una variable para detectar cual es el límite del personaje para caer al vacío y si la sobrepasa, el jugador vuelve a la posición inicial

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
    }
```

![respawn1](https://github.com/user-attachments/assets/a52d076f-9425-4ee6-8492-e4a70467f277)

- Cambio de niveles (de momento hay 3, pero es ampliable a más)

Para hacer el cambio de niveles, simplemente en el script del personaje, lo que hacemos es comprobar si los items recogidos son iguales al total de items del tablero, en este caso 12. Si es así, cambiamos de posición tanto al jugador, como al enemigo.

```bash
#metodo para cambiar de nivel
 //metodo para cambiar de niveles
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
        if (countObjectsInt == totalItems)
        {
            countLevelInt += 1;
            if (countLevelInt == 2)
            {
                transform.position = new Vector3(77.02f, 3.79f, -0.889570f);

                NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.enabled = false;
                    enemy.transform.position = new Vector3(68.23f, 3.676f, 0.768f);
                    agent.enabled = true;
                    countObjectsInt = 0;
                    countObjectsText.color = redColor;
                }
            }
            if (countLevelInt == 3)
            {
                transform.position = new Vector3(38.99f, 3.79f, -151.89f);

                 NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.enabled = false;
                    enemy.transform.position = new Vector3(54.18f, 3.69f, -136.6f);
                    agent.enabled = true;
                    countObjectsInt = 0;
                    countObjectsText.color = redColor;
                }

            }




        }


    }
```

```bash
#demo cambio de nivel
```

![cambioNivel2](https://github.com/user-attachments/assets/ffec527c-98ad-4f74-84e0-f8bfd3cd03c9)


#### nivel 2

```bash
mapa con obstáculos del nivel 2
```
![nivel2P1](https://github.com/user-attachments/assets/adb98cad-c741-485f-9cc9-b6f6347ffe31)

![nivel2P2](https://github.com/user-attachments/assets/f368c18a-2e99-4bf3-95fa-b215e269c28f)

#### nivel 3

```bash
mapa con obstáculos del nivel 3
```

![nivel3P1](https://github.com/user-attachments/assets/655df566-942d-4305-9617-38fda7c6874c)


![nivel3P2](https://github.com/user-attachments/assets/8a23652e-e835-43b5-a95d-618987e5e675)






