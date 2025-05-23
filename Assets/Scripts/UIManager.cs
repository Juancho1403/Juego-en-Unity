using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Referencias a los elementos de la UI que muestran la puntuación
    [SerializeField] private TextMeshProUGUI player1Score;  // Texto que muestra la puntuación del jugador 1
    [SerializeField] private TextMeshProUGUI player2Score;  // Texto que muestra la puntuación del jugador 2
    [SerializeField] private GameObject results;  // Panel o ventana que muestra los resultados al final del partido

    // Función que muestra los resultados al final del partido
    public void DisplayResults(int player1, int player2)
    {
        results.SetActive(true);  // Activar el panel de resultados
        player1Score.text = player1.ToString();  // Actualizar la puntuación del jugador 1
        player2Score.text = player2.ToString();  // Actualizar la puntuación del jugador 2
    }
}
