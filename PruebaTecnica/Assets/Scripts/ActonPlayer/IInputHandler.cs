using UnityEngine;
using Oculus.Interaction.Input;

public interface IInputHandler
{
    /// <summary>Devuelve true si hay que disparar este frame.</summary>
    bool GetTriggerOrPinchDown();
    /// <summary>Devuelve true si la mano tiene tracking v�lido.</summary>
    bool IsHandTracked();
    /// <summary>Devuelve true si el controlador tiene pose v�lida.</summary>
    bool IsControllerPoseValid();
    /// <summary>Devuelve el Transform de la mano.</summary>
    Transform GetHandTransform();
    /// <summary>Devuelve el Transform del controlador.</summary>
    Transform GetControllerTransform();
}