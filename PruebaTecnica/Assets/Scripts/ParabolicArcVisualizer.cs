using UnityEngine;
using Oculus.Interaction.Input;

public class ParabolicArcVisualizer : MonoBehaviour
{
    public Transform handTransform;
    public Transform controllerTransform;

    public Controller rightController;
    public Hand hand;

    public LineRenderer lineRenderer;        

    public int points = 30;                   
    public float timeStep = 0.1f;             
    public float initialSpeed = 8f;          
    public float gravity = -9.81f;            

    private void Update()
    {
        DrawArc();
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
        }else if (rightController.IsPoseValid)
        {
            startPosition = controllerTransform.position;
            startVelocity = controllerTransform.right * initialSpeed;
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
    }
}
