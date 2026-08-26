using UnityEngine;

public class AtaqueEnemigo : MonoBehaviour
{
    [Header("Configuración del ataque")]
    public float distanciaAtaque = 2.2f;
    public float daño = 10f;
    public float tiempoEntreAtaques = 1f;

    private Transform jugador;
    private VidaJugador vidaJugador;

    private float siguienteAtaque = 0f;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            jugador = playerObject.transform;
            vidaJugador = playerObject.GetComponent<VidaJugador>();

            if (vidaJugador == null)
            {
                Debug.LogError("❌ El Player no tiene el componente VidaJugador.");
            }
        }
        else
        {
            Debug.LogError("❌ No se encontró el Player.");
        }
    }

    void Update()
    {
        if (jugador == null || vidaJugador == null)
            return;

        float distancia =
            Vector3.Distance(transform.position, jugador.position);

        if (distancia <= distanciaAtaque)
        {
            Atacar();
        }
    }

    void Atacar()
    {
        if (Time.time < siguienteAtaque)
            return;

        siguienteAtaque = Time.time + tiempoEntreAtaques;

        vidaJugador.RecibirDaño(daño);

        Debug.Log("⚠️ El enemigo atacó al jugador. Daño: " + daño);
    }
}