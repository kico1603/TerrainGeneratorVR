using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButton : MonoBehaviour
{
    public void OnButtonPressed()
    {
        var action = PlayerActionService.PlayerAction;
        if (action != null)
        {
            action.DoSomething();
        }
        else
        {
            Debug.LogWarning("No hay jugador registrado");
        }
    }
}
