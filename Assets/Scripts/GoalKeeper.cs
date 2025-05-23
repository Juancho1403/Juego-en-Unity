using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalKeeper : MonoBehaviour
{
    // Acción que se invoca cuando se marca un gol
    public Action OnScore;

    // Método que se llama cuando el portero entra en contacto con otro objeto
    private void OnCollisionEnter2D(Collision2D col)
    {
        // Intenta obtener el componente IInteractable del objeto con el que colisiona
        IInteractable ball = col.gameObject.GetComponent<IInteractable>();

        // Si el objeto con el que colisiona es interactuable (por ejemplo, el balón)
        if (ball != null)
        {
            // Elimina la interacción con el balón (detener su movimiento o lo que se necesite hacer al interactuar)
            ball.RemoveInteraction();

            // Invoca el evento OnScore si hay suscriptores (indica que se ha marcado un gol)
            OnScore?.Invoke();
        }
    }
}
