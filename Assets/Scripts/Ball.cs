using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour, IInteractable
{
    [SerializeField] private Rigidbody2D rigidbody2D;  // Referencia al Rigidbody2D de la pelota
    [SerializeField] private Collider2D collider2D;    // Referencia al Collider2D de la pelota
    [SerializeField] private AudioSource source;       // Fuente de audio para los efectos de sonido

    [SerializeField] private bool isIAGame;            // Bandera para indicar si es un juego de IA
    private PhysicsMaterial2D _material;               // Material físico de la pelota, para restaurarlo después

    private void Awake()
    {
        // Inicializa el material físico cuando el objeto se crea
        _material = collider2D.sharedMaterial;
    }

    // Método que maneja la interacción de un jugador con la pelota
    public void Interact(FootballPlayer player)
    {
        // Reproduce el sonido de la pelota
        source.Play();

        // Genera valores aleatorios para la dirección y la fuerza del disparo
        var randY = Random.Range(-0.01f, 0.011f);
        var randShootForce = Random.Range(10, 50);
        var rand = new Vector2(1, randY);

        // Si el jugador está mirando hacia la otra portería, dispara en esa dirección
        if (IsFacingOther(player.transform, player.OtherGoal))
        {
            Vector2 direction = (player.OtherGoal.position - transform.position).normalized;
            rigidbody2D.AddForce((rand * direction) / randShootForce, ForceMode2D.Impulse);
            return;
        }

        // Si no está mirando hacia la otra portería, dispara hacia la derecha
        rigidbody2D.AddForce((rand * Vector2.right) / randShootForce, ForceMode2D.Impulse);
    }

    // Método que verifica si un jugador está mirando hacia la otra portería
    private bool IsFacingOther(Transform player, Transform other)
    {
        // Calcula la dirección hacia la otra portería
        Vector2 toOther = (other.position - player.position).normalized;

        // Compara la dirección del jugador con la dirección hacia la portería usando el producto punto
        var result = Vector2.Dot(toOther, player.right);

        // Si el jugador está mirando hacia la portería (producto punto mayor que 0.75), devuelve verdadero
        if (result > 0.75f)
        {
            return true;
        }
        return false;
    }

    // Método que elimina la interacción de la pelota (cuando se detiene el juego o se termina la interacción)
    public void RemoveInteraction()
    {
        // Restaura el material físico original de la pelota
        collider2D.sharedMaterial = null;

        // Detiene el movimiento de la pelota
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.angularVelocity = 0f;

        // Si la pelota tiene un padre, congela las restricciones de su Rigidbody2D
        if (transform.parent)
        {
            transform.parent.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    // Método que habilita la interacción de nuevo (al reanudar el juego o al permitir interacción)
    public void EnableInteraction()
    {
        // Restaura el material físico original de la pelota
        collider2D.sharedMaterial = _material;
    }
}
