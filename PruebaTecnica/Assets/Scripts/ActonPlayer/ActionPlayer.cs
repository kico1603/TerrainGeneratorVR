using UnityEngine;

public class ActionPlayer : MonoBehaviour, IPlayerAction
{
    [Header("Componentes del sistema (referencias)")]
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private TrajectoryCalculator arcCalculator;
    [SerializeField] private ArcRenderer arcRenderer;
    [SerializeField] private GhostManager ghostManager;
    [SerializeField] private ObjectSpawner objectSpawner;

    // Referencias a interfaces (para mantener inversión de dependencias)
    private IInputHandler _input;
    private IArcCalculator _arc;
    private IArcRenderer _arcRend;
    private IGhostManager _ghost;
    private IObjectSpawner _spawner;

    [Header("Configuración de disparo")]
    [Tooltip("Tiempo de enfriamiento entre disparos consecutivos (segundos).")]
    [SerializeField] private float shootCooldown = 1.0f;

    private float lastShootTime = 0f;   // Momento en que se realizó el último disparo
    private bool wasFirePressed = false; // Estado previo de la entrada de disparo (para detectar cambios si es necesario)
  

    private void Awake()
    {
        PlayerActionService.PlayerAction = this;
    }

    private void Start()
    {
        // Asignar referencias de componentes a sus interfaces
        _input = inputHandler;
        _arc = arcCalculator;
        _arcRend = arcRenderer;
        _ghost = ghostManager;
        _spawner = objectSpawner;

        // Inicialmente ocultar el ghost hasta que haya una trayectoria válida
        if (_ghost != null) _ghost.HideGhost();
        // Inicializar el LineRenderer vacío
        if (_arcRend != null) _arcRend.ClearArc();

        lastShootTime = 5f; // permitir disparo inmediato al inicio
    }

    private void Update()
    {
       
        if (_input == null || _arc == null || _arcRend == null || _ghost == null || _spawner == null)
        {
            Debug.LogError("Falta alguna referencia");
            return; 
        }

        // 1. Determinar origen y dirección según la fuente activa (mano o controlador)
        if (!_input.IsHandTracked() && !_input.IsControllerPoseValid())
        {
            // Si no hay mano ni controlador activos, no hay trayectoria que mostrar
            _arcRend.ClearArc();
            _ghost.HideGhost();
            return;
        }

        Vector3 origin = Vector3.zero;
        Vector3 direction = Vector3.zero;

        if (_input.IsHandTracked())
        {
            origin = _input.GetHandTransform().position;
            direction = _input.GetHandTransform().right;
        }
        else if (_input.IsControllerPoseValid())
        {
            origin = _input.GetControllerTransform().position;
            direction = _input.GetControllerTransform().forward;
        }

        //Vector3 origin = _input.GetHandTransform();
        //Vector3 direction = _input.GetDirection();

        // 2. Calcular la trayectoria del proyectil con la física definida
        var arcPoints = _arc.CalculateArcPoints(origin, direction);

        // Dibujar la trayectoria en la escena
        _arcRend.RenderArc(arcPoints);

        // 3. Gestionar el ghost en la posición final de la trayectoria
        bool validTarget = false;
        if (arcPoints != null && arcPoints.Count > 0)
        {
            Vector3 endPoint = arcPoints[arcPoints.Count - 1];
            validTarget = _ghost.PlaceGhost(endPoint);
        }
        else
        {
            // Si por alguna razón no hay puntos, ocultar el ghost
            _ghost.HideGhost();
        }

        // 4. Comprobar entrada de disparo (puño o gatillo) y cooldown
        bool firePressed = _input.GetTriggerOrFistDown();
        if (firePressed)
        {
            // Si el disparo está activado y ha pasado el tiempo de cooldown
            if (Time.time - lastShootTime >= shootCooldown)
            {
                // Solo disparar si hay un objetivo válido (ghost colocado sobre una superficie)
                if (validTarget && _ghost.IsGhostActive())
                {
                    // Instanciar el objeto real (cubo) en la posición del ghost
                    Vector3 spawnPos = _ghost.GetGhostPosition();
                    Quaternion spawnRot = _ghost.GetGhostRotation();
                    _spawner.SpawnObject(spawnPos, spawnRot);
                }
                // Actualizar el tiempo del último disparo
                lastShootTime = Time.time;
            }
        }

        // Guardar estado de botón para lógica de una sola pulsación si se necesitara
        wasFirePressed = firePressed;
    }

    public void ClearCubes()
    {
        objectSpawner.ClearObjects();

    }

    void OnDestroy()
    {
        if (PlayerActionService.PlayerAction == this)
            PlayerActionService.PlayerAction = null;
    }

    public void DoSomething()
    {
        ClearCubes();
    }
}
