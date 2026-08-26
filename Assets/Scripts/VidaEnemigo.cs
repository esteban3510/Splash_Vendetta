using UnityEngine;
using UnityEngine.AI;

public class VidaEnemigo : MonoBehaviour
{
    [Header("Vida del enemigo")]
    public float vidaMaxima = 100f;
    public float vidaActual;

    [Header("Animación de muerte")]
    public string triggerMorir = "Morir";

    [Tooltip("Tiempo aproximado de la animación de muerte.")]
    public float tiempoMuerte = 2f;

    private EnemyFollow enemyFollow;
    private EnemyCombat enemyCombat;
    private Animator animator;
    private NavMeshAgent agent;
    private Rigidbody rb;

    private bool estaMuerto = false;


    // =====================================================
    // START
    // =====================================================

    void Start()
    {
        vidaActual = vidaMaxima;

        enemyFollow =
            GetComponent<EnemyFollow>();

        enemyCombat =
            GetComponent<EnemyCombat>();

        animator =
            GetComponent<Animator>();

        agent =
            GetComponent<NavMeshAgent>();

        rb =
            GetComponent<Rigidbody>();


        // =================================================
        // RIGIDBODY
        // =================================================

        if (rb != null)
        {
            // El Rigidbody NO controla al enemigo.
            // El movimiento lo controla NavMeshAgent.

            rb.isKinematic = true;
            rb.useGravity = false;
        }


        Debug.Log(
            "Enemigo creado con "
            + vidaActual
            + " de vida."
        );
    }


    // =====================================================
    // RECIBIR DAÑO
    // =====================================================

    public void RecibirDaño(float cantidad)
    {
        if (estaMuerto)
        {
            return;
        }


        vidaActual -= cantidad;


        vidaActual =
            Mathf.Clamp(
                vidaActual,
                0f,
                vidaMaxima
            );


        Debug.Log(
            "Enemigo recibió "
            + cantidad
            + " de daño. Vida actual: "
            + vidaActual
        );


        // =================================================
        // AVISAR A ENEMY FOLLOW
        // =================================================

        if (enemyFollow != null &&
            vidaActual > 0f)
        {
            enemyFollow.RecibirDisparo();
        }


        // =================================================
        // MUERTE
        // =================================================

        if (vidaActual <= 0f)
        {
            Morir();
        }
    }


    // =====================================================
    // MORIR
    // =====================================================

    void Morir()
    {
        if (estaMuerto)
        {
            return;
        }


        estaMuerto = true;


        Debug.Log(
            "💀 ENEMIGO ELIMINADO"
        );


        // =================================================
        // DETENER ENEMY FOLLOW
        // =================================================

        if (enemyFollow != null)
        {
            enemyFollow.enabled = false;
        }


        // =================================================
        // DETENER ENEMY COMBAT
        // =================================================

        if (enemyCombat != null)
        {
            enemyCombat.estaEnCombate = false;

            enemyCombat.enabled = false;
        }


        // =================================================
        // DETENER NAVMESH
        // =================================================

        if (agent != null &&
            agent.enabled)
        {
            agent.isStopped = true;

            agent.ResetPath();

            agent.updatePosition = false;

            agent.updateRotation = false;

            agent.enabled = false;
        }


        // =================================================
        // RIGIDBODY
        // =================================================

        // IMPORTANTE:
        // NO activamos física.
        //
        // La caída la realiza la animación "Muere".

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }


        // =================================================
        // ANIMACIÓN DE MUERTE
        // =================================================

        if (animator != null)
        {
            animator.SetBool(
                "EstaCaminando",
                false
            );

            animator.SetBool(
                "JugadorDetectado",
                false
            );

            animator.SetBool(
                "EnemigoAlertado",
                false
            );


            animator.SetTrigger(
                triggerMorir
            );
        }
        else
        {
            Debug.LogWarning(
                "⚠️ VidaEnemigo: no se encontró Animator."
            );
        }


        // =================================================
        // DESTRUIR
        // =================================================

        Destroy(
            gameObject,
            tiempoMuerte
        );
    }
}