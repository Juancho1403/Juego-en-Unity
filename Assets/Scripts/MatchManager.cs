using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchManager : MonoBehaviour
{
    // Referencias a los objetos de la escena
    [SerializeField] private GoalKeeper player1Goal;  // Referencia al portero del jugador 1
    [SerializeField] private GoalKeeper player2Goal;  // Referencia al portero del jugador 2

    [SerializeField] private FootballPlayer player1;  // Referencia al jugador 1
    [SerializeField] private FootballPlayer player2;  // Referencia al jugador 2
    [SerializeField] private Ball ball;  // Referencia a la pelota

    // Referencias a los textos de la UI
    [SerializeField] private TextMeshProUGUI player1ScoreText;  // Texto que muestra la puntuación de jugador 1
    [SerializeField] private TextMeshProUGUI player2ScoreText;  // Texto que muestra la puntuación de jugador 2
    [SerializeField] private TextMeshProUGUI goalText;  // Texto que muestra un mensaje cuando se marca un gol
    private int player1Score;  // Puntuación del jugador 1
    private int player2Score;  // Puntuación del jugador 2

    private int secondsToPlay = 59;  // Tiempo restante en segundos para el partido
    
    [SerializeField] private TextMeshProUGUI timer;  // Referencia al temporizador en la UI

    [SerializeField] private AudioSource source;  // Fuente de audio para efectos de sonido
    [SerializeField] private AudioClip goal;  // Sonido cuando se marca un gol
    [SerializeField] private AudioClip referee;  // Sonido del árbitro

    [SerializeField] private UIManager uiManager;  // Referencia al UIManager para mostrar los resultados

    // Evento que se ejecuta cuando un portero marca un gol
    private void Awake()
    {
        // Registrar las funciones que se ejecutarán cuando se marque un gol
        player1Goal.OnScore += ScoreP2;  // Si el portero 1 marca, se incrementa el marcador del jugador 2
        player2Goal.OnScore += ScoreP1;  // Si el portero 2 marca, se incrementa el marcador del jugador 1
        StartCoroutine(MatchTimer());  // Iniciar el temporizador del partido
        EnablePlayers();  // Habilitar jugadores al comenzar
    }

    // Recargar la escena si se presiona la tecla 'R'
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);  // Recarga la escena actual
        }
    }

    // Corutina que gestiona el temporizador del partido
    IEnumerator MatchTimer()
    {
        UpdateTimer();  // Actualizar el temporizador en la UI
        source.clip = referee;  // Sonido del árbitro al inicio
        source.Play();  // Reproducir sonido
        while (secondsToPlay > 0)
        {
            yield return new WaitForSeconds(1);  // Esperar un segundo
            secondsToPlay--;  // Reducir el tiempo restante
            UpdateTimer();  // Actualizar el temporizador en la UI
        }
        DisablePlayers();  // Deshabilitar a los jugadores cuando se acabe el tiempo
        uiManager.DisplayResults(player1Score, player2Score);  // Mostrar los resultados finales
        player1ScoreText.gameObject.SetActive(false);  // Ocultar los textos de puntuación
        player2ScoreText.gameObject.SetActive(false);
    }

    // Corutina que maneja el proceso de un gol
    IEnumerator ScoreGoal()
    {
        goalText.gameObject.SetActive(true);  // Mostrar mensaje de gol
        source.clip = goal;  // Reproducir sonido de gol
        source.Play();
        LeanTween.scale(goalText.rectTransform, goalText.transform.localScale * 2f, source.clip.length).setEasePunch();  // Animar el texto de gol
        DisablePlayers();  // Deshabilitar jugadores mientras se celebra el gol
        yield return new WaitForSeconds(source.clip.length);  // Esperar hasta que termine el sonido
        goalText.gameObject.SetActive(false);  // Ocultar mensaje de gol
        StartOver();  // Reiniciar la posición de los jugadores y la pelota
        source.clip = referee;  // Reproducir sonido del árbitro
        source.Play();
        yield return new WaitForSeconds(source.clip.length);  // Esperar hasta que termine el sonido
        EnablePlayers();  // Habilitar jugadores nuevamente
    }

    // Reiniciar la posición de los jugadores y la pelota
    private void StartOver()
    {
        player1.transform.position = new Vector2(-1.5f, 0);  // Posición inicial de jugador 1
        player1.transform.rotation = Quaternion.Euler(0, 0, 0);  // Rotación inicial de jugador 1
        player2.transform.position = new Vector2(1.5f, 0);  // Posición inicial de jugador 2
        player2.transform.rotation = Quaternion.Euler(0, 0, 180);  // Rotación inicial de jugador 2
        ball.transform.parent = null;  // Desvincular la pelota de cualquier objeto
        ball.EnableInteraction();  // Habilitar la interacción con la pelota
        ball.transform.position = Vector2.zero;  // Colocar la pelota en el centro
        ball.transform.rotation = Quaternion.identity;  // Asegurar que la pelota no tiene rotación
    }

    // Incrementar la puntuación del jugador 1
    private void ScoreP1()
    {
        player1Score++;  // Incrementar la puntuación de jugador 1
        UpdateScores();  // Actualizar los textos de puntuación
        StartCoroutine(ScoreGoal());  // Iniciar el proceso de celebración de gol
    }

    // Incrementar la puntuación del jugador 2
    private void ScoreP2()
    {
        player2Score++;  // Incrementar la puntuación de jugador 2
        UpdateScores();  // Actualizar los textos de puntuación
        StartCoroutine(ScoreGoal());  // Iniciar el proceso de celebración de gol
    }

    // Actualizar el temporizador en la UI
    private void UpdateTimer()
    {
        float minutes = Mathf.FloorToInt(secondsToPlay / 60);  // Calcular minutos
        float seconds = Mathf.FloorToInt(secondsToPlay % 60);  // Calcular segundos

        string secs = seconds.ToString();
        string mins = minutes.ToString();
        if (seconds < 10)
            secs = "0" + secs;  // Asegurar que los segundos tienen dos dígitos
        if (minutes < 10)
            mins = "0" + mins;  // Asegurar que los minutos tienen dos dígitos
        timer.text = mins + " : " + secs;  // Mostrar el temporizador en la UI
    }

    // Actualizar las puntuaciones en la UI
    private void UpdateScores()
    {
        player1ScoreText.text = player1Score.ToString();  // Actualizar el texto de la puntuación de jugador 1
        player2ScoreText.text = player2Score.ToString();  // Actualizar el texto de la puntuación de jugador 2
    }

    // Deshabilitar a los jugadores (por ejemplo, cuando el partido termina)
    private void DisablePlayers()
    {
        player1.DisablePlayer();  // Deshabilitar jugador 1
        player2.DisablePlayer();  // Deshabilitar jugador 2
    }

    // Habilitar a los jugadores para que puedan jugar
    private void EnablePlayers()
    {
        player1.EnablePlayer();  // Habilitar jugador 1
        player2.EnablePlayer();  // Habilitar jugador 2
    }
}
