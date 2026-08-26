using UnityEngine;

public class ProyectilEnemigo : MonoBehaviour
{
    [Header("Daño")]
    [Tooltip("Daño por disparo. 25 = 4 disparos para eliminar al jugador con 100 de vida.")]
    public float daño = 25f;

    [Header("Vida del proyectil")]
    public float tiempoVida = 5f;

    private bool yaImpacto = false;


    // =====================================================
    // START
    // =====================================================

    void Start()
    {
        Destroy(
            gameObject,
            tiempoVida
        );
    }


    // =====================================================
    // COLISIÓN
    // =====================================================

    private void OnCollisionEnter(
        Collision collision
    )
    {
        if (yaImpacto)
        {
            return;
        }

        yaImpacto = true;


        // =================================================
        // BUSCAR AL JUGADOR
        // =================================================

        VidaJugador vidaJugador =
            collision.collider.GetComponent<VidaJugador>();


        // Si VidaJugador está en el padre
        if (vidaJugador == null)
        {
            vidaJugador =
                collision.collider.GetComponentInParent<VidaJugador>();
        }


        // =================================================
        // APLICAR DAÑO
        // =================================================

        if (vidaJugador != null)
        {
            vidaJugador.RecibirDaño(
                daño
            );

            Debug.Log(
                "🔴 Proyectil enemigo impactó al jugador. "
                + "Daño: "
                + daño
                + " | Vida restante: "
                + vidaJugador.vidaActual
            );
        }


        // =================================================
        // DESTRUIR PROYECTIL
        // =================================================

        Destroy(gameObject);
    }
}