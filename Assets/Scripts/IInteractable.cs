// Interfaz que define la interacción entre objetos en el juego (por ejemplo, jugadores y balón)
public interface IInteractable
{
    // Método que se llama cuando un jugador interactúa con el objeto.
    // El parámetro `player` representa al jugador que está interactuando con el objeto.
    void Interact(FootballPlayer player);

    // Método que se llama para eliminar la interacción entre el objeto y el jugador.
    void RemoveInteraction();
}
