using UnityEngine;
using Oculus.Interaction.Input;

public class InputHandler : MonoBehaviour, IInputHandler
{
    [SerializeField] private Controller controller;
    [SerializeField] private Hand hand;
    [SerializeField] private Transform handTransform;
    [SerializeField] private Transform controllerTransform;
    [Tooltip("Cooldown entre disparos con el puño (segundos)")]
  

    public bool GetTriggerOrPinchDown()
    {
       
        if (IsHandTracked())
        {
            return IsPinch(hand);
        }else if (IsControllerPoseValid())
        {
            return controller.ControllerInput.TriggerButton;
        }
        else
        {
            return false;
        }


    }

    public bool IsHandTracked()
    {
        return hand != null && hand.IsTrackedDataValid;
    }

    public bool IsControllerPoseValid()
    {
        return controller != null && controller.IsPoseValid;
    }

    public Transform GetHandTransform()
    {
        return handTransform;
    }

    public Transform GetControllerTransform()
    {
        return controllerTransform;
    }

    private bool IsPinch(Hand hand)
    {
        float threshold = 0.2f;

        bool thumb = hand.GetFingerPinchStrength(HandFinger.Thumb) > threshold;
        bool index = hand.GetFingerPinchStrength(HandFinger.Index) > threshold;
        return index && thumb; 
    }
}