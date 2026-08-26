using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyCombat : MonoBehaviour
{
    [Header("Combate")]
    public float distanciaAtaque = 8f;

    [Tooltip("Tiempo entre cada disparo.")]
    public float tiempoEntreDisparos = 1.2f;

    [Tooltip("Tiempo que espera antes del primer disparo.")]
    public float tiempoAntesDeDisparar = 0.5f;


    [Header("Daño")]
    [Tooltip("Daño que hace cada disparo del enemigo.")]
    public float dañoPorDisparo = 25f;


    [Header("Munición")]
    [Tooltip("Cantidad de balas que puede disparar antes de recargar.")]
    public int balasMaximas = 5;

    [Tooltip("Tiempo que tarda la recarga.")]
    public float tiempoRecarga = 2.5f;

    [HideInInspector]
    public bool estaEnCombate = false;


    [Header("Apuntado")]
    public Transform puntoDisparo;
    public Transform objetivo;


    [Header("Proyectil")]
    public GameObject prefabProyectil;
    public float fuerzaDisparo = 20f;


    [Header("Detección visual")]
    public float alturaVista = 1.5f;
    public LayerMask capasObstaculos;


    [Header("Mirada")]
    public float alturaMirada = 1.4f;
    public float pesoMirada = 1f;


    // =====================================================
    // VARIABLES INTERNAS
    // =====================================================

    private float temporizadorDisparo = 0f;
    private float temporizadorDeteccion = 0f;

    private int balasActuales;

    private bool estaRecargando = false;

    private NavMeshAgent agent;
    private EnemyFollow enemyFollow;
    private Animator animator;


    // =====================================================
    // INICIO
    // =====================================================

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        enemyFollow =
            GetComponent<EnemyFollow>();

        animator =
            GetComponent<Animator>();


        // =================================================
        // MUNICIÓN INICIAL
        // =================================================

        balasActuales =
            Mathf.Max(
                1,
                balasMaximas
            );


        // =================================================
        // BUSCAR AL JUGADOR
        // =================================================

        GameObject jugador =
            GameObject.FindGameObjectWithTag(
                "Player"
            );

        if (jugador != null)
        {
            objetivo =
                jugador.transform;
        }
        else
        {
            Debug.LogError(
                "❌ EnemyCombat: no se encontró Player."
            );
        }


        // =================================================
        // COMPROBAR ANIMATOR
        // =================================================

        if (animator == null)
        {
            Debug.LogWarning(
                "⚠️ EnemyCombat: no se encontró Animator."
            );
        }
    }


    // =====================================================
    // UPDATE
    // =====================================================

    void Update()
    {
        if (objetivo == null)
        {
            return;
        }


        // =================================================
        // JUGADOR MUERTO
        // =================================================

        VidaJugador vidaJugador =
            objetivo.GetComponent<VidaJugador>();

        if (vidaJugador != null &&
            vidaJugador.estaMuerto)
        {
            temporizadorDeteccion = 0f;
            temporizadorDisparo = 0f;

            estaEnCombate = false;

            return;
        }


        // =================================================
        // TODAVÍA NO HA SIDO DETECTADO
        // =================================================

        if (enemyFollow != null &&
            !enemyFollow.jugadorDetectado)
        {
            temporizadorDeteccion = 0f;
            temporizadorDisparo = 0f;

            return;
        }


        // =================================================
        // NO ESTÁ EN COMBATE
        // =================================================

        if (!estaEnCombate)
        {
            temporizadorDeteccion = 0f;
            temporizadorDisparo = 0f;

            return;
        }


        // =================================================
        // RECARGANDO
        // =================================================

        if (estaRecargando)
        {
            ApuntarAlJugador();

            return;
        }


        // =================================================
        // DISTANCIA
        // =================================================

        float distancia =
            Vector3.Distance(
                transform.position,
                objetivo.position
            );


        if (distancia > distanciaAtaque)
        {
            temporizadorDeteccion = 0f;
            temporizadorDisparo = 0f;

            return;
        }


        // =================================================
        // VISIÓN
        // =================================================

        if (!PuedeVerAlJugador())
        {
            temporizadorDeteccion = 0f;
            temporizadorDisparo = 0f;

            return;
        }


        // =================================================
        // APUNTAR
        // =================================================

        ApuntarAlJugador();


        // =================================================
        // ESPERA ANTES DEL PRIMER DISPARO
        // =================================================

        temporizadorDeteccion +=
            Time.deltaTime;


        if (temporizadorDeteccion <
            tiempoAntesDeDisparar)
        {
            return;
        }


        // =================================================
        // COMPROBAR MUNICIÓN
        // =================================================

        if (balasActuales <= 0)
        {
            IniciarRecarga();

            return;
        }


        // =================================================
        // TIEMPO ENTRE DISPAROS
        // =================================================

        temporizadorDisparo +=
            Time.deltaTime;


        if (temporizadorDisparo >=
            tiempoEntreDisparos)
        {
            Disparar();

            temporizadorDisparo = 0f;
        }
    }


    // =====================================================
    // INICIAR RECARGA
    // =====================================================

    void IniciarRecarga()
    {
        if (estaRecargando)
        {
            return;
        }


        estaRecargando = true;

        temporizadorDisparo = 0f;


        if (animator != null)
        {
            animator.SetTrigger(
                "Recargar"
            );
        }


        Debug.Log(
            "🔄 ENEMIGO RECARGANDO..."
        );


        StartCoroutine(
            TerminarRecarga()
        );
    }


    // =====================================================
    // TERMINAR RECARGA
    // =====================================================

    IEnumerator TerminarRecarga()
    {
        yield return new WaitForSeconds(
            tiempoRecarga
        );


        balasActuales =
            Mathf.Max(
                1,
                balasMaximas
            );


        estaRecargando = false;

        temporizadorDeteccion = 0f;

        temporizadorDisparo = 0f;


        Debug.Log(
            "🔫 ENEMIGO RECARGADO - BALAS: "
            + balasActuales
        );
    }


    // =====================================================
    // VISIÓN DEL JUGADOR
    // =====================================================

    bool PuedeVerAlJugador()
    {
        if (objetivo == null)
        {
            return false;
        }


        Vector3 origen;


        if (puntoDisparo != null)
        {
            origen =
                puntoDisparo.position;
        }
        else
        {
            origen =
                transform.position +
                Vector3.up * alturaVista;
        }


        Vector3 destino =
            objetivo.position +
            Vector3.up * 1f;


        Vector3 direccion =
            destino - origen;


        float distancia =
            direccion.magnitude;


        if (distancia <= 0.01f)
        {
            return false;
        }


        direccion.Normalize();


        RaycastHit hit;


        if (Physics.Raycast(
            origen,
            direccion,
            out hit,
            distancia,
            capasObstaculos
        ))
        {
            return false;
        }


        return true;
    }


    // =====================================================
    // GIRAR CUERPO HACIA EL JUGADOR
    // =====================================================

    void ApuntarAlJugador()
    {
        if (objetivo == null)
        {
            return;
        }


        Vector3 direccion =
            objetivo.position -
            transform.position;


        direccion.y = 0f;


        if (direccion.sqrMagnitude < 0.01f)
        {
            return;
        }


        Quaternion rotacionObjetivo =
            Quaternion.LookRotation(
                direccion
            );


        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                rotacionObjetivo,
                Time.deltaTime * 8f
            );
    }


    // =====================================================
    // MIRADA DEL ANIMATOR - IK
    // =====================================================

    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null ||
            objetivo == null ||
            enemyFollow == null)
        {
            return;
        }


        if (!enemyFollow.jugadorDetectado)
        {
            animator.SetLookAtWeight(0f);

            return;
        }


        Vector3 puntoMirada =
            objetivo.position +
            Vector3.up * alturaMirada;


        animator.SetLookAtWeight(
            pesoMirada,
            0.6f,
            0.9f,
            0.9f,
            0.5f
        );


        animator.SetLookAtPosition(
            puntoMirada
        );
    }


    // =====================================================
    // DISPARAR
    // =====================================================

    void Disparar()
    {
        if (prefabProyectil == null)
        {
            Debug.LogWarning(
                "⚠️ EnemyCombat: no hay Prefab Proyectil asignado."
            );

            return;
        }


        if (puntoDisparo == null)
        {
            Debug.LogWarning(
                "⚠️ EnemyCombat: no hay Punto Disparo asignado."
            );

            return;
        }


        if (objetivo == null)
        {
            return;
        }


        // =================================================
        // DIRECCIÓN
        // =================================================

        Vector3 direccion =
            (
                objetivo.position -
                puntoDisparo.position
            ).normalized;


        // =================================================
        // ROTACIÓN
        // =================================================

        Quaternion rotacion =
            Quaternion.LookRotation(
                direccion
            );


        // =================================================
        // CREAR PROYECTIL
        // =================================================

        GameObject nuevaBala =
            Instantiate(
                prefabProyectil,
                puntoDisparo.position,
                rotacion
            );


        // =================================================
        // ⭐ ASIGNAR DAÑO AL PROYECTIL
        // =================================================

        ProyectilEnemigo proyectil =
            nuevaBala.GetComponent<ProyectilEnemigo>();


        if (proyectil != null)
        {
            proyectil.daño =
                dañoPorDisparo;


            Debug.Log(
                "💥 PROYECTIL CREADO | DAÑO ASIGNADO: "
                + proyectil.daño
            );
        }
        else
        {
            Debug.LogError(
                "❌ EnemyCombat: el prefab del proyectil "
                + "NO tiene el componente ProyectilEnemigo."
            );
        }


        // =================================================
        // FUERZA
        // =================================================

        Rigidbody rb =
            nuevaBala.GetComponent<Rigidbody>();


        if (rb != null)
        {
            rb.AddForce(
                direccion * fuerzaDisparo,
                ForceMode.Impulse
            );
        }


        // =================================================
        // EVITAR COLISIÓN CON EL ENEMIGO
        // =================================================

        Collider balaCollider =
            nuevaBala.GetComponent<Collider>();


        Collider[] collidersEnemigo =
            GetComponentsInChildren<Collider>();


        if (balaCollider != null)
        {
            foreach (
                Collider col
                in collidersEnemigo
            )
            {
                if (col != null)
                {
                    Physics.IgnoreCollision(
                        balaCollider,
                        col
                    );
                }
            }
        }


        // =================================================
        // RESTAR MUNICIÓN
        // =================================================

        balasActuales--;


        Debug.Log(
            "🔫 ENEMIGO DISPARÓ | BALAS RESTANTES: "
            + balasActuales
            + " | DAÑO: "
            + dañoPorDisparo
        );


        // =================================================
        // ANIMACIÓN DISPARO
        // =================================================

        if (animator != null)
        {
            animator.SetTrigger(
                "Disparar"
            );
        }
    }
}