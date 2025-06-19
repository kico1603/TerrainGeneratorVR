using UnityEngine;
using Oculus.Interaction.Input;

public class InputHandler : MonoBehaviour, IInputHandler
{
    [SerializeField] private Controller controller;
    [SerializeField] private Hand hand;
    [SerializeField] private Transform handTransform;
    [SerializeField] private Transform controllerTransform;
    [Tooltip("Cooldown entre disparos con el puño (segundos)")]
    //[SerializeField] private float fistReleaseCooldown = 0.5f;

    //private bool lastFist = false;
    //private float fistReleasedTime = 0f;
    //private bool canTriggerFist = true;

    public bool GetTriggerOrFistDown()
    {
        //bool trigger = rightController != null && rightController.ControllerInput.TriggerButton;
        //bool fist;

        //if (hand != null)
        //{
        //    fist = IsFist(hand);

        //    //if (fist && !lastFist && canTriggerFist)
        //    //{
        //    //    canTriggerFist = false;
        //    //    lastFist = fist;
        //    //    return true;
        //    //}
        //    //else if (!fist && lastFist)
        //    //{
        //    //    fistReleasedTime = Time.time;
        //    //}

        //    //if (!fist && !canTriggerFist && (Time.time - fistReleasedTime >= fistReleaseCooldown))
        //    //{
        //    //    canTriggerFist = true;
        //    //}

        //    //lastFist = fist;
        //}
        //// Si disparó por gatillo
        //if (trigger)
        //{
        //    return true;
        //}
        //return false;

        if (IsHandTracked())
        {
            return IsFist(hand);
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

    private bool IsFist(Hand hand)
    {
        float threshold = 0.05f;

        bool thumb = hand.GetFingerPinchStrength(HandFinger.Thumb) > threshold;
        bool index = hand.GetFingerPinchStrength(HandFinger.Index) > threshold;
        //bool middle = hand.GetFingerIsPinching(HandFinger.Middle);// > threshold;
        //bool ring = hand.GetFingerIsPinching(HandFinger.Ring); //> threshold;
        //bool pinky = hand.GetFingerIsPinching(HandFinger.Pinky);//> threshold;
        return index && thumb; // && ring && pinky;
    }
}