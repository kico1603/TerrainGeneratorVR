Prueba Técnica Unity Developer – VR Procedural Terrain

## Introducción y Presentación Personal

**Francisco J. Ponce** – *Unity Developer/Game Designer*. Puedes conocer más sobre mí en [LinkedIn ](https://www.linkedin.com/in/francisco-ponce-gamedesigner/)y en mi [portfolio](https://poncegamedesigner.wordpress.com/) personal. Este proyecto es el resultado de una prueba técnica para un puesto de Unity Developer, desarrollada en unas **~20 horas**.

Me encuentro satisfecho con lo logrado en el tiempo dado: se ha implementado un entorno VR con terreno generado proceduralmente y mecánicas básicas de interacción. A continuación detallo el gameplay, la instalación, controles, y las soluciones técnicas empleadas, así como reflexiones sobre desafíos y posibles mejoras.

## Gameplay y APK

Puedes ver una **demostración en video** del gameplay en YouTube: [Demo Gameplay (VR Quest)](https://www.youtube.com/watch?v=9ObWI_5tgxQ). En este video se aprecia la generación del terreno y la interacción básica con cubos en realidad virtual.

También he preparado un **APK ejecutable** para visores Meta Quest. Puedes descargar la aplicación (Android APK) desde este enlace de Google Drive:[ Descargar APK para Quest.](https://drive.google.com/file/d/1_AjjPGUY0g9D5x4tx3ASVpHQrPmPYTmD/view?usp=drive_link) *(Nota: Se tiene que habilitar la instalación de “orígenes desconocidos” en tu dispositivo Quest para poder instalar la APK.)*

## Instalación y Requisitos Técnicos

- **Unity:** El proyecto fue desarrollado con **Unity 2022.3.50f1** (LTS). Se recomienda usar exactamente esta versión para evitar incompatibilidades.
  
- **SDK XR:** Utiliza el **Meta XR SDK** en su versión *v76*.
  
- **Plataforma:** El proyecto está pensado para **Meta Quest** (standalone). Ha sido probado principalmente en un **Meta Quest 3** y también en el **Simulador de Meta XR** .
  
- **Hardware requerido:** Para la experiencia completa se necesita un visor **Meta Quest 3** (debería ser compatible con Quest 2 igualmente, aunque no testeado directamente). Alternativamente, puedes ejecutar la escena en Unity usando el **XR Simulator** provisto por el SDK de Meta para simular manos y mandos VR en pantalla.
  

**Instrucciones de ejecución:** Clona este repositorio y ábrelo con Unity 2022.3.50f1. Abre la escena principal del proyecto en Assents/Scenes/Main y pulsa *Play* con un Quest conectado (o con el simulador configurado) para probar la aplicación. Si exportas a un dispositivo Quest, recuerda activar el *VR* en Player Settings y seguir los pasos normales para desplegar en Android.

## Guía de Uso y Controles

El proyecto soporta interacción tanto con **manos (hand tracking)** como con **controladores**:

- **Interacción con la UI:** Para pulsar botones o interactuar con la interfaz, puedes usar **tus manos** seleccionando con los dedos, o si usas mando, tocando con el mando la propia interfaz como si de una mano se tratase.
  
- **Instanciar cubos en el mundo:** Esta es la acción principal de la demo. Para crear un cubo en el entorno, **apunta** hacia el lugar deseado (con la mano o con el controlador). Luego:
  
  - Si usas **manos**, realiza el gesto de **puño cerrado** mientras apuntas hacia adelante.
    
  - Si usas el **mando**, presiona el **gatillo** mientras apuntas.
    
    Al hacerlo, se instanciará un cubo en la posición objetivo a cierta distancia (justo al frente del rayo). Puedes generar múltiples cubos y estos utilizan un sistema de *object pooling* para reciclarse.
    

## Explicación General de Scripts Principales

A continuación describo los scripts más importantes del proyecto, que conforman la lógica central:

### TerrainGenerator.cs

Este script principal del **terreno procedural por chunks**. Su responsabilidad es generar secciones de terreno (chunks) de forma dinámica, incluyendo características como elevación variable y caminos que conectan un chunk con el siguiente.

- **Generación de terreno:** Utiliza ruido Perlin y otros algoritmos para crear variaciones en la altura del terreno de cada chunk. La generación es **modular y configurable** mediante parámetros (por ejemplo, el tamaño del chunk, amplitud de la altura, densidad de detalles, etc. están definidos en un ScriptableObject de configuración global). Esto permite ajustar fácilmente el aspecto del mundo sin modificar código.
  
- **Chunks y límites:** El mundo está dividido en **chunks** (secciones cuadradas de terreno). `TerrainGenerator` se encarga de crear los nuevos chunks a medida que el jugador (o su punto de interés) alcanza el borde de uno, implementando una suerte de “mundo infinito” por secciones. Cada chunk tiene bordes que deben coincidir con sus vecinos para evitar discontinuidades bruscas en altura.
  
- **Caminos entre chunks:** Una parte clave de la prueba era generar **caminos** que atravesaran el terreno y continuaran de un chunk al siguiente. El script calcula posiciones de entrada y salida de caminos por chunk, de forma que cuando un nuevo chunk se genera, detecta si debe haber una **entrada** de camino conectada con la **salida** del chunk anterior. Con algoritmos simples de trazado, dibuja/eleva un camino sobre la superficie del terreno. Este sistema es **flexible** en el sentido de que soporta distintos anchos de camino o curvaturas básicas configurables. Sin embargo, reconozco que es un área compleja y mi implementación básica podría no cubrir todos los casos.
  

En resumen, `TerrainGenerator.cs` provee la lógica procedural para que el mundo se construya de forma incremental y *coherente*, manteniendo continuidad en el terreno y permitiendo la conexión de vías entre segmentos.

### ActionPlayer.cs

`ActionPlayer.cs` actúa como el **controlador central de las acciones del jugador**, principalmente el sistema de *disparo/instanciación* de objetos y la interacción general. En un proyecto VR típico, este rol lo llevaría un controlador de interacción; aquí implementé uno propio para mostrar conocimientos de arquitectura de código.

- **Rol centralizado:** Este script se suscribe a los **inputs** del jugador (ya sea botón de mando o gestos de la mano) y, cuando detecta la acción de “disparo” (gatillo o puño cerrado), instancia un cubo en la escena. Para ello, hace un *raycast* desde la mano/controlador para determinar dónde colocar el cubo. También coordina el feedback visual si lo hubiera (por ejemplo, podría activar un indicador visual al apuntar, etc.).
  
- **Principios SOLID aplicados:** Diseñé `ActionPlayer` intentando seguir buenas prácticas de arquitectura. Por ejemplo, hace uso de **interfaces** para abstraer las entradas: tanto una mano como un controlador implementan una interfaz común (por ej. `IInputSource`) que define eventos como *OnShoot* o *OnUIClick*. De esta forma, `ActionPlayer` no necesita saber si el evento provino de la mano o del mando, simplemente escucha la interfaz. Esto ejemplifica la **inversión de dependencias**: las manos/mandos disparan eventos que el ActionPlayer recibe, en lugar de que éste consulte directamente a cada tipo de dispositivo. Esta modularidad facilita la **escalabilidad**, permitiendo agregar fácilmente nuevos tipos de acciones (por ejemplo, lanzar proyectiles distintos) o soportar nuevos dispositivos sin cambiar la lógica central.
  
- **Escalabilidad y mantenimiento:** Gracias a esta estructura, si quisiéramos ampliar el juego (digamos agregar una segunda arma, o permitir agarrar objetos con las manos), podríamos crear nuevos componentes que implementen las mismas interfaces de interacción y `ActionPlayer` podría manejarlos sin grandes cambios. Aunque el tiempo no permitió construir un sistema de input completamente genérico, la base sentada en `ActionPlayer` sigue principios que lo hacen mantenible y extendible.
  

*Nota:* Existen más scripts en la carpeta `Assets/Scripts` (por ejemplo, controladores de UI VR, manejadores de colisiones de cubos, etc.) que sirven como soporte al gameplay. No los detallo todos aquí para mantener el enfoque en los principales, pero en general siguen una funcionalidad específica y relativamente sencilla acorde a su nombre.

## Sistemas Adicionales Implementados

Además de los scripts principales, el proyecto incorpora varios **sistemas de apoyo** para cumplir con los requerimientos de la prueba técnica:

- **Configuración del mundo vía ScriptableObject:** Se creó un ScriptableObject que actúa como **configuración global** de parámetros del mundo (por ejemplo, tamaño de los chunks, altura máxima del terreno, longitud esperada de los caminos, prefabs de objetos a instanciar, etc.). Esto permite ajustar el comportamiento del terreno procedural y otras mecánicas desde el Editor de Unity de forma fácil, siguiendo el principio de tener “datos ajustables” sin cambiar código. La configuración centralizada facilita también experimentar con distintos valores durante el desarrollo.
  
- **Sistema de escenas aditivas:** Para manejar eficientemente el mundo y la experiencia VR, utilicé **escenas cargadas de forma aditiva**. Hay una escena base (p.ej. el entorno principal o escena de gameplay) y otras escenas secundarias que se cargan encima (additively). En este proyecto, por ejemplo, la generación de terrenos por chunks podría gestionarse con escenas aditivas (cada chunk como sub-escena), o la UI VR se mantiene en una escena separada cargada sobre la principal. Esta técnica ayuda a **organizar** el proyecto y potencialmente a manejar el streaming de contenido (cargar/unload chunks) sin pausas notables. En la implementación actual, configuré el sistema de escenas para cargar inicializaciones y luego el mundo de juego en paralelo.
  
- **Pool genérico de objetos:** Se implementó un **object pool** genérico para reutilizar instancias de objetos en vez de crearlos/destruirlos constantemente. Actualmente se usa para los cubos instanciados por el jugador: en lugar de Instantiate/Destroy cada vez, el pool mantiene un número de cubos disponibles y recicla los no usados (mejora el rendimiento y evita *garbage collection* excesivo en VR). El sistema está diseñado de forma **extensible**, de modo que fácilmente podríamos aprovecharlo para otros tipos de objetos (enemigos, proyectiles, etc.) simplemente registrando nuevos prefabs en el pool manager. Esto demuestra consideración por la eficiencia, importante especialmente en dispositivos móviles VR.
  

## Desafíos Encontrados

Durante el desarrollo de esta prueba técnica me enfrenté a varios desafíos dignos de mención:

- **Actualización del SDK de Meta XR:** Inicialmente el proyecto se planteó con una versión del SDK, pero para asegurar compatibilidad con **Quest 3** tuve que migrar a la versión *v76* del Meta XR SDK. Este cambio implicó ajustar algunas configuraciones (por ejemplo, la gestión de **input XR** y corregir algunos métodos obsoletos o cambios en el plugin). Fue un desafío menor pero necesario: las actualizaciones del SDK a veces rompían la compatibilidad con ciertas funciones, y hubo que invertir tiempo en depurar los nuevos **XR Interaction** features y asegurar que el hand tracking y los controles siguieran funcionando correctamente tras la actualización.
  
- **Sistema de caminos procedural:** Sin duda, el **trazado de caminos inter-chunk** fue el reto más grande. La idea era generar caminos continuos que cruzaran de un terreno a otro de manera coherente. Lograr que un camino generado aleatoriamente en un chunk coincidiera con la entrada esperada en el siguiente chunk implicó idear algún sistema de “anclaje” de los puntos de inicio/fin del camino. Implementé una solución básica donde cada chunk conoce si debe tener una salida de camino en su borde, y el siguiente chunk genera la continuación desde ese punto. Sin embargo, admito que **probablemente no cumple al 100% con todos los requisitos** esperados: por ejemplo, la variedad o naturalidad de los caminos es limitada y puede haber casos borde donde el camino no encaje perfectamente. Dada la complejidad, requeriría más tiempo afinar este sistema (posiblemente usando algoritmos más avanzados de generación de caminos, e.j. algoritmos de grafos o splines para suavizar).
  
- **Generación procedural de terreno:** Aunque la generación de terreno por ruido es un tema popular (existen muchas referencias y código abierto al respecto), integrarla con las demás mecánicas tuvo su complejidad. Tuve que equilibrar tamaño de los chunks, nivel de detalle vs. rendimiento en VR, y garantizar que la costura entre chunks fuera suave en términos de alturas. Afortunadamente, hay bastante documentación y ejemplos sobre *procedural terrain in Unity*, lo cual me sirvió de guía. Aún así, ajustar los parámetros y depurar la aparición de artefactos (picos indeseados, huecos entre chunks) tomó tiempo. Fue un desafío **técnico** pero también una de las partes más interesantes de desarrollar, ya que aprendí a combinar distintas técnicas conocidas de generación procedural y adaptarlas a un entorno VR móvil.
  

### Uso de Inteligencia Artificial en el Desarrollo

Durante el desarrollo del proyecto se ha hecho un uso puntual de Inteligencia Artificial (IA) para agilizar ciertas tareas. En particular, la IA ha ayudado en aspectos concretos como los cálculos y verificación de fórmulas matemáticas relacionadas con la generación procedural del terreno, soporte en la lógica del sistema de caminos, y también en la redacción, revisión y limpieza final de este documento, permitiendo así acelerar el proceso de trabajo y mantener una documentación clara y profesional.

## Críticas y Posibles Mejoras

Como desarrollador, suelo evaluar críticamente el resultado para identificar áreas de mejora. En este proyecto de prueba técnica destaco lo siguiente:

- **Arquitectura y escalabilidad:** Por cuestiones de tiempo, no apliqué una arquitectura de proyecto tan modular/escalable como me hubiese gustado. Idealmente se podría separar más claramente la lógica de generación, de interacción, de datos, quizás usando patrones de diseño como MVC o sistemas más orientados a datos (ECS). No obstante, dentro de las limitaciones, intenté mantener el código **limpio y mantenible**. El script `ActionPlayer.cs` es un ejemplo donde apliqué principios SOLID (interfaces, single responsibility, etc.) para facilitar futuras extensiones. Con más tiempo, expandiría ese enfoque al resto del proyecto, separando responsabilidades en más clases y reduciendo dependencias directas entre sistemas.
  
- **Funcionalidad faltante para ser un juego completo:** El entregable funciona como **demo técnica**, pero no es un juego completo. Varias características quedaron fuera por el alcance de la prueba, por ejemplo: un sistema de objetivos o desafío para el jugador, más tipos de objetos o interacción (actualmente solo hay cubos), una interfaz de usuario más pulida (menús, feedback en HUD), sonido y música, entre otros. También temas como guardar progreso, reinicios, o incluso optimizaciones avanzadas (oclusión, LODs en el terreno) no se implementaron. Identifico que para llevar esto a un producto final, habría que **añadir y refinar muchas cosas**. A pesar de ello, el núcleo presentado demuestra las bases técnicas solicitadas (VR interaction, terreno procedural, object pooling, etc.). Dado más tiempo y en un contexto de desarrollo real, estas bases se irían puliendo y expandiendo hasta conformar una experiencia de usuario sólida.
  

En conclusión, fue una prueba estimulante donde pude combinar mis conocimientos de Unity en VR con generación procedural. El resultado es modesto pero cumple con gran parte de lo pedido, y sobre todo, me dejó aprendizajes sobre cómo integrar sistemas complejos en un tiempo acotado. Quedo satisfecho con el resultado obtenido, consciente de lo que podría mejorarse a futuro. ¡Gracias por la oportunidad de desarrollar este proyecto!
