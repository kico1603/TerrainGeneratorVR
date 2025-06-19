using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;

public class ActionPlayerOLD : MonoBehaviour
{
    [SerializeField] private Controller rightController;
    [SerializeField] private Hand hand;

    [SerializeField] private Transform handTransform;
    [SerializeField] private Transform controllerTransform;

    private bool lastFist = false;
    private float fistReleasedTime = 0f;
    private bool canTriggerFist = true;
    public float fistReleaseCooldown = 0.5f;

    [SerializeField] private LineRenderer lineRenderer;

    public int points = 30;
    public float timeStep = 0.1f;
    public float initialSpeed = 8f;
    public float gravity = -9.81f;


    [SerializeField] private GameObject cubeGhost;

    private ObjectsPoolTypes objectsPool;

    //public float gridSize = 1f;

    [Range(0, 89)] public float elevationAngle = 30f; // �ngulo de elevaci�n en grados

    private List<GameObject> cubesInstaced = new List<GameObject>();


    private void Start()
    {

        cubeGhost.SetActive(false);
        objectsPool = ObjectsPoolTypes.Instance;

    }

    private void Update()
    {

        if (rightController != null && rightController.ControllerInput.TriggerButton)
        {
            OnTriggerOrFist();
        }



        if (hand != null)
        {
            bool currentFist = IsFist(hand);

            if (currentFist && !lastFist && canTriggerFist)
            {

                OnTriggerOrFist();
                canTriggerFist = false;
            }
            else if (!currentFist && lastFist)
            {

                fistReleasedTime = Time.time;
            }


            if (!currentFist && !canTriggerFist && (Time.time - fistReleasedTime >= fistReleaseCooldown))
            {
                canTriggerFist = true;
            }

            lastFist = currentFist;
        }

        DrawArc();
    }

    bool IsFist(Hand hand)
    {
        float threshold = 0.05f;
        bool index = hand.GetFingerPinchStrength(HandFinger.Index) > threshold;
        bool middle = hand.GetFingerPinchStrength(HandFinger.Middle) > threshold;
        bool ring = hand.GetFingerPinchStrength(HandFinger.Ring) > threshold;
        bool pinky = hand.GetFingerPinchStrength(HandFinger.Pinky) > threshold;
        return index && middle && ring && pinky;
    }

    // Implement your logic here
    private void OnTriggerOrFist()
    {
        //Debug.Log("�Disparado!");

        if (cubeGhost.activeSelf)
        {
            cubesInstaced.Add(objectsPool.InstanciteObjectPool(ObjectsPoolTypes.TypeGameObjectIDKey.CubeMetal, cubeGhost.transform.position, cubeGhost.transform.rotation));
        }

    }


    void DrawArc()
    {

        //Debug.Log(" controller " + rightController.IsPoseValid + " mano " + hand.IsTrackedDataValid);

        Vector3[] arcPoints = new Vector3[points];


        Vector3 startPosition = Vector3.zero;
        Vector3 startVelocity = Vector3.zero;

        if (hand.IsTrackedDataValid)
        {
            startPosition = handTransform.position;
            startVelocity = handTransform.right * initialSpeed;
        }
        else if (rightController.IsPoseValid)
        {
            startPosition = controllerTransform.position;
            startVelocity = controllerTransform.forward * initialSpeed;
        }
        else
        {
            lineRenderer.positionCount = 0;
        }


        // Aumenta el initialSpeed si subes la mano 
        // initialSpeed = Mathf.Lerp(6f, 15f, Mathf.Clamp01(handTransform.up.y));

        for (int i = 0; i < points; i++)
        {
            float t = i * timeStep;
            Vector3 gravityVector = Vector3.up * gravity;
            Vector3 point = startPosition + startVelocity * t + 0.5f * gravityVector * t * t;
            arcPoints[i] = point;


            if (point.y <= 0.01f)
            {

                System.Array.Resize(ref arcPoints, i + 1);
                break;
            }
        }

        lineRenderer.positionCount = arcPoints.Length;
        lineRenderer.SetPositions(arcPoints);



        if (cubeGhost != null && arcPoints.Length > 0)
        {
            Vector3 hitPoint = arcPoints[arcPoints.Length - 1];

            // Raycast hacia abajo para encontrar el cubo real debajo del punto de impacto
            RaycastHit hit;
            Vector3 rayOrigin = hitPoint + Vector3.up * 2f; // Un poco por encima por si el punto es justo en el borde
            float rayDistance = 3f;

            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, rayDistance))
            {
                // Encontramos un objeto, coloca el marcador justo encima de ese objeto
                Vector3 above = hit.collider.transform.position + Vector3.up * hit.collider.bounds.size.y;
                cubeGhost.transform.position = above;
                cubeGhost.SetActive(true);
            }
            else
            {
                // No se encontr� ning�n objeto debajo, oculta el marcador (o ponlo en el punto de impacto +1 en Y como fallback)
                cubeGhost.SetActive(false);
                // placementInstance.transform.position = hitPoint + Vector3.up; // O esta opci�n
            }
        }
        else if (cubeGhost != null)
        {
            cubeGhost.SetActive(false);
        }


    }

    public void RemoveAllCubes()
    {
        foreach (GameObject cube in cubesInstaced)
        {
            objectsPool.ReturnObjectToObjectPool(ObjectsPoolTypes.TypeGameObjectIDKey.CubeRock, cube);
        }
    }
}
