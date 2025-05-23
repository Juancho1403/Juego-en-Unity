using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Carga el menú principal
    public void LoadMenu()
    {
        SceneManager.LoadScene(0);  // Asegúrate de que el menú esté en el índice 0 en el Build Settings
    }

    // Carga la escena de 1 vs IA
    public void Play1vsAI()
    {
        SceneManager.LoadScene(1);  // Asegúrate de que la escena de 1 vs IA esté en el índice 1
    }

    // Carga la escena de 1 vs 1
    public void Play1vs1()
    {
        SceneManager.LoadScene(2);  // Asegúrate de que la escena de 1 vs 1 esté en el índice 2
    }

    // Carga la escena de IA vs IA
    public void PlayAIvsAI()
    {
        SceneManager.LoadScene(3);  // Asegúrate de que la escena de IA vs IA esté en el índice 3
    }

    // Sale del juego
    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");  // Solo para pruebas en el editor
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Detiene el juego en el editor
        #endif
    }
}