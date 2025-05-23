using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BotPlayer : FootballPlayer
{
    [SerializeField] private Transform myGoal;  // Referencia a la portería del bot
    [SerializeField] private Transform ball;    // Referencia a la pelota
    [SerializeField] private FootballPlayer player;  // Referencia al jugador controlado por el usuario
    
    private float speedX;  // Velocidad en el eje X
    private float speedY;  // Velocidad en el eje Y
    private State currentState;  // Estado actual del bot (atacar, defender, atrapar)

    // Variables para controlar la dirección del movimiento del bot
    private bool _moveUp;
    private bool _moveLeft;
    private bool _moveRight;
    private bool _moveDown;
    private bool _goalKeeping;  // Indica si el bot está en modo portero

    private bool _hasShot;  // Indica si el bot ha disparado

    // Enumeración para los estados del bot (atacar, defender, atrapar)
    private enum State
    {
        CATCH = 0,  // Estado en el que el bot trata de atrapar la pelota
        DEFEND = 1,  // Estado en el que el bot defiende la portería
        ATTACK = 2,  // Estado en el que el bot ataca hacia la portería contraria
    }

    private void Start()
    {
        // Inicializa las velocidades y el estado inicial
        speedX = speed;
        speedY = speed;
        currentState = State.CATCH;
    }

    private void FixedUpdate()
    {
        if (_canPlay)
        {
            // Movimiento hacia la izquierda
            if (_moveLeft)
            {
                Debug.Log("MOVIENDO HACIA LA IZQUIERDA");
                movement = Vector2.left;
                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                    Quaternion.Euler(new Vector3(0f, 0, 180)), turnRate);
            }
            // Movimiento hacia la derecha
            else if (_moveRight)
            {
                Debug.Log("MOVIENDO HACIA LA DERECHA");
                movement = Vector2.right;
                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                    Quaternion.Euler(new Vector3(0f, 0, 0)), turnRate);
            }
            
            // Movimiento hacia arriba
            if (_moveUp)
            {
                Debug.Log("MOVIENDO HACIA ARRIBA");
                movement = Vector2.up;
                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                    Quaternion.Euler(new Vector3(0f, 0, 90)), turnRate);
            }
            // Movimiento hacia abajo
            else if (_moveDown)
            {
                Debug.Log("MOVIENDO HACIA ABAJO");
                movement = Vector2.down;
                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                    Quaternion.Euler(new Vector3(0f, 0, -90)), turnRate);
            }

            // Aplica la velocidad del bot al Rigidbody2D para moverlo
            rigidbody2D.velocity = new Vector2(movement.x * speedX, movement.y * speedY);
        }
    }
    
    private void OnCollisionStay2D(Collision2D col)
    {
        var interactable =  col.gameObject.GetComponent<IInteractable>();
        if (interactable != null)
        {
            // Si el bot está cerca de la portería y tiene la pelota
            if (Vector2.Distance(transform.position, otherGoal.position) < 2.5f)
            {
                if (ballPosession)
                {
                    // Si el bot tiene la pelota, la suelta
                    ballPosession.parent = null;
                    ballPosession = null;
                    transform.rotation = Quaternion.RotateTowards(transform.rotation,
                        Quaternion.Euler(new Vector3(0f, 0, 180)), turnRate);
                }
                // Interactúa con el objeto (puede disparar o defender)
                interactable.Interact(this);
                // Comienza la rutina de disparo y defensa
                StartCoroutine(ShootAndDefend());
            }
        }
    }

    // Corutina que maneja el disparo y la defensa
    IEnumerator ShootAndDefend()
    {
        _hasShot = true;  // El bot ha disparado
        yield return new WaitForSeconds(Random.Range(0.5f, 0.75f));  // Espera un tiempo aleatorio antes de volver a defender
        _hasShot = false;  // El bot deja de haber disparado
    }

    private void OnCollisionExit(Collision other)
    {
        // Cuando el bot deja de estar en contacto con la pelota, la suelta
        ballPosession = null;
    }
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        var interactable =  col.gameObject.GetComponent<IInteractable>();
        if (interactable != null)
        {
            // Si el bot toca la pelota, la toma
            col.transform.parent = null;
            ballPosession = col.transform;
            ballPosession.parent = transform;
        }
    }

    private void Update()
    {
        // Cambia el estado del bot según las condiciones
        if (!HasBall)
        {
            if (!player.HasBall && !_hasShot) currentState = State.CATCH;  // Si el jugador no tiene la pelota, intenta atraparla
            else if (player.HasBall && Vector2.Distance(player.transform.position, myGoal.position) < 3) currentState = State.DEFEND;  // Si el jugador tiene la pelota y está cerca de la portería, el bot defiende
            else if (!player.HasBall && _hasShot) currentState = State.DEFEND;  // Si el bot ha disparado, defiende
            else if (!HasBall && Vector2.Distance(player.transform.position, transform.position) < 1f) currentState = State.DEFEND;  // Si el bot no tiene la pelota y está cerca del jugador, defiende
        }
        else if (HasBall) currentState = State.ATTACK;  // Si el bot tiene la pelota, ataca
        
        // Dependiendo del estado, realiza una acción
        switch (currentState)
        {
            case State.CATCH:
                Debug.Log("Atrapando");
                MoveTowards(ball);  // Atrapando la pelota
                break;
            case State.DEFEND:
                Debug.Log("Defendiendo");
                MoveTowards(myGoal, 0.25f);  // Defendiendo la portería
                break;
            case State.ATTACK:
                Debug.Log("Atacando");
                MoveTowards(otherGoal, Random.Range(1, 2));  // Atacando hacia la portería contraria
                TranslateBall();  // Traduciendo la pelota (jugada de ataque)
                break;
        }
    }

    // Método que mueve al bot hacia un objetivo (la portería o la pelota)
    private void MoveTowards(Transform target, float offsetX = 0, float offsetY = 0)
    {
        if (!_goalKeeping)
        {
            var distance = transform.position.x - target.position.x;
            Debug.Log("X:"+distance);
            // Movimiento a la izquierda
            if (distance > 0.05f + offsetX)
            {
                _moveUp = false;
                _moveDown = false;
                _moveLeft = true;
                _moveRight = false;
                speedX = speed;
            }
            // Movimiento a la derecha
            else if (distance < -0.75f + offsetX)
            {
                _moveUp = false;
                _moveDown = false;
                _moveRight = true;
                _moveLeft = false;
                speedX = speed;
            }
            // Movimiento en el eje Y (arriba/abajo)
            else if (distance >= -0.75 + offsetX && distance <= 0.05f + offsetX)
            {
                _moveLeft = false;
                _moveRight = false;
                speedX = 0;
                var distanceY = transform.position.y - target.position.y;
                Debug.Log("Y:"+distanceY);
                if (distanceY > 0.05f + offsetY)
                {
                    _moveUp = false;
                    _moveDown = true;
                    speedY = speed;
                }
                else if (distanceY < -0.05 + offsetY)
                {
                    _moveUp = true;
                    _moveDown = false;
                    speedY = speed;
                }
                else if (distanceY >= -0.05 + offsetY && distanceY <= 0.05f + offsetY)
                {
                    _moveUp = false;
                    _moveDown = false;
                    speedY = 0;
                }
            }
        }
        else
        {
            // Si el bot está en modo portero, no se mueve
            _moveDown = false;
            _moveUp = false;
            _moveLeft = false;
            _moveRight = false;
        }
    }
}
