using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootballPlayer : MonoBehaviour
{
    // Atributos del jugador
    [SerializeField] protected float speed; // Velocidad de movimiento del jugador
    [SerializeField] protected float turnRate = 10f; // Tasa de rotación del jugador
    [SerializeField] protected Rigidbody2D rigidbody2D; // Componente Rigidbody2D para aplicar físicas
    [SerializeField] private KeyCode[] controls; // Controles para el jugador (arriba, izquierda, abajo, derecha, disparo)
    [SerializeField] protected Transform otherGoal; // Transform del gol opuesto
    [SerializeField] protected Transform ballPosition; // Posición del balón
    public Transform OtherGoal => otherGoal; // Propiedad para obtener el gol opuesto
    public bool HasBall => ballPosession != null && ballPosession.parent == transform; // Verifica si el jugador tiene el balón

    protected Vector2 movement; // Dirección de movimiento del jugador
    protected Transform ballPosession; // Referencia al balón si lo tiene el jugador
    protected bool _canPlay; // Bandera que indica si el jugador puede moverse

    // Método que se llama en cada frame físico (FixedUpdate)
    private void FixedUpdate()
    {
        if (_canPlay)
        {
            // Mover hacia arriba
            if (Input.GetKey(controls[0]))
            {
                movement = Vector2.up;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                    Quaternion.Euler(new Vector3(0f, 0, 90)), turnRate);
            }
            // Mover hacia la izquierda
            else if (Input.GetKey(controls[1]))
            {
                movement = Vector2.left;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                    Quaternion.Euler(new Vector3(0f, 0, 180)), turnRate);
            }
            // Mover hacia abajo
            else if (Input.GetKey(controls[2]))
            {
                movement = Vector2.down;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                    Quaternion.Euler(new Vector3(0f, 0, -90)), turnRate);
            }
            // Mover hacia la derecha
            else if (Input.GetKey(controls[3]))
            {
                movement = Vector2.right;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                    Quaternion.Euler(new Vector3(0f, 0, 0)), turnRate);
            }
            else
            {
                movement = Vector2.zero; // Si no se presionan teclas de movimiento, no se mueve
            }

            // Aplica la velocidad al Rigidbody2D del jugador para moverlo
            rigidbody2D.velocity = new Vector2(movement.x, movement.y) * speed;
        }
    }

    // Método Update que se llama cada frame
    protected void Update()
    {
        TranslateBall(); // Actualiza la posición del balón si el jugador lo tiene
    }

    // Método que actualiza la posición del balón cuando el jugador lo posee
    protected void TranslateBall()
    {
        if (ballPosession && ballPosession.parent == transform) 
        {
            // Actualiza la posición del balón para que siga al jugador
            ballPosession.position = ballPosition.position;
            // Detiene la rotación del balón
            ballPosession.gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0f;
        }
    }

    // Método que se llama cuando el jugador entra en contacto con otro objeto
    private void OnCollisionEnter2D(Collision2D col)
    {
        var interactable =  col.gameObject.GetComponent<IInteractable>();
        if (interactable != null)
        {
            // Si el jugador colisiona con un objeto interactuable, toma el balón
            col.transform.parent = null;
            ballPosession = col.transform;
            ballPosession.parent = transform; // Establece el balón como hijo del jugador
        }
    }

    // Método que se llama cuando el jugador sigue en contacto con un objeto
    private void OnCollisionStay2D(Collision2D col)
    {
        var interactable =  col.gameObject.GetComponent<IInteractable>();
        if (interactable != null)
        {
            // Si se presiona la tecla de disparo y el jugador tiene el balón, lo suelta
            if (Input.GetKey(controls[4]))
            {
                if (ballPosession)
                {
                    ballPosession.parent = null;
                    ballPosession = null; // El jugador suelta el balón
                }
                interactable.Interact(this); // Realiza una acción de interacción
            }
        }
    }

    // Método para deshabilitar el jugador (detiene el movimiento)
    public void DisablePlayer()
    {
        _canPlay = false; // Desactiva la capacidad de moverse
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll; // Congela el Rigidbody2D del jugador
    }
    
    // Método para habilitar el jugador (permite moverse)
    public void EnablePlayer()
    {
        _canPlay = true; // Activa la capacidad de moverse
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation; // Permite que el jugador rote
    }
}
