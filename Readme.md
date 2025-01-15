### TAREA UNITY. ROLL A BALL 😄

**Índice**

1. Creación del proyecto, plano del juego y personalización de obstáculos
2. Creación del jugador y movimiento
3. Configuración de camara y coleccionables
4. enemigo y más personalizaciones del tablero
5. Planteamiento de más niveles

### 1. Creación del proyecto y plano del juego 😄
Lo primero de todo sería crear el proyecto en Unity, para eso tenemos que irnos a Unity Hub y descargar la versión que vamos a utilizar, que es la **2022.3.50f1**.
Una vez descargada, pasamos a crear el proyecto.

```bash
#le damos a create new proyect, escogemos un nombre y ubicación de donde estará el proyecto
```

El juego se divide en escenas, al crear el proyecto se crea una escena por defecto: cambiamos el nombre a la escena y empezamos a crear el plano donde se va a desarrollar nuestro roll a ball.

**Creación del plano**😎
```bash
En nuestra escena vamos a hacer el suelo mediante un objeto 3D llamado Plane, el cual va a recibir una scale determinado para hacerlo más grande
```

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
- Añadimos un Rigidbody a la sphere para que se comporte como un objeto esfera de la vida real
- A través de script, manejamos el movimiento del personaje
- Para moverlo tenemos que ponerle unos Input settings

```bash
Hecho todo lo anterior, el personaje se podrá mover sin problemas
```

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

Luego debemos de hacer lo siguiente:
- Nos vamos a la camara
- Hemos establecido una variable pública en el script, así que es accesible desde fuera del script
- Esa variable la vamos a hacer igual al objeto 3D del player

```bash
Ahora la cámara va a seguir al jugador sin problemas
```


