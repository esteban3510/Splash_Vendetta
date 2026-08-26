using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyFollow : MonoBehaviour
{
    [Header("Jugador")]
    private Transform player;
    private ControlCamara playerScript;

    // =====================================================
    // DETECCIÓN
    // =====================================================

    [Header("Detección")]
    public float distanciaDeteccion = 10f;
    public float distanciaParar = 8f;

    // =====================================================
    // DISTANCIA DE COMBATE
    // =====================================================

    [Header("Distancia de combate")]
    public float distanciaMinimaCombate = 5f;
    public float distanciaIdealCombate = 6.5f;
    public float distanciaMaximaCombate = 8f;

    // =====================================================
    // ESCENA
    // =====================================================

    [Header("Escena permitida")]
    public string escenaPermitida = "Level_01";

    // =====================================================
    // PATRULLA
    // =====================================================

    [Header("Patrulla")]
    public Transform[] puntosPatrulla;
    public float distanciaLlegadaPatrulla = 0.5f;
    public bool patrullaRepetitiva = true;

    // =====================================================
    // VISIÓN
    // =====================================================

    [Header("Visión")]
    public Transform puntoVista;
    public LayerMask capasVision = ~0;

    // =====================================================
    // SONIDO
    // =====================================================

    [Header("Sonido")]
    public bool puedeEscucharPasos = true;

    // =====================================================
    // ESCUCHA DE DISPAROS
    // =====================================================

    [Header("Escucha de disparos")]
    public float radioEscuchaDisparo = 15f;
    public int disparosParaDetectarJugador = 4;

    // =====================================================
    // BÚSQUEDA ALERTADO
    // =====================================================

    [Header("Búsqueda mientras está alertado")]
    public float velocidadBusqueda = 60f;
    public bool buscarMientrasEstaAlertado = true;
    public float tiempoCambioBusqueda = 2f;

    // =====================================================
    // MOVIMIENTO EVASIVO
    // =====================================================

    [Header("Movimiento evasivo")]
    public bool usarMovimientoEvasivo = true;

    public float tiempoMinimoEvasion = 1.5f;
    public float tiempoMaximoEvasion = 2.5f;

    public float distanciaLateral = 2.5f;
    public float distanciaReversa = 2f;

    [Range(0f, 1f)]
    public float probabilidadLateral = 0.65f;

    // =====================================================
    // ANIMACIÓN DE MOVIMIENTO
    // =====================================================

    [Header("Animación de movimiento")]

    [Tooltip("0 = CaminaApunta, 1 = CaminaReversa, 2 = CaminaLateral, 3 = EnemigoCaminaLateral")]
    [Range(0, 3)]
    public int movimientoCombate = 0;

    // =====================================================
    // CONTADOR DE DISPAROS
    // =====================================================

    private int disparosEscuchados = 0;

    // =====================================================
    // ESTADOS
    // =====================================================

    private bool alertaPermanente = false;
    private bool enemigoAlertado = false;

    [HideInInspector]
    public bool jugadorDetectado = false;

    // =====================================================
    // COMBATE
    // =====================================================

    [Header("Combate")]
    public EnemyCombat enemyCombat;

    // =====================================================
    // COMPONENTES
    // =====================================================

    private NavMeshAgent agent;
    private Animator animator;

    // =====================================================
    // JUGADOR
    // =====================================================

    private Vector3 posicionAnteriorJugador;

    // =====================================================
    // PATRULLA
    // =====================================================

    private int indicePatrulla = 0;

    // =====================================================
    // BÚSQUEDA
    // =====================================================

    private float direccionBusqueda = 1f;
    private float temporizadorBusqueda = 0f;

    // =====================================================
    // EVASIÓN
    // =====================================================

    private float temporizadorEvasion = 0f;
    private float duracionEvasionActual = 0f;

    private bool movimientoEvasivoActivo = false;

    private Vector3 destinoEvasion;

    // =====================================================
    // START
    // =====================================================

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agent != null)
        {
            agent.updateRotation = true;
            agent.updateUpAxis = true;
        }

        if (animator != null)
        {
            animator.applyRootMotion = false;

            animator.SetBool("EstaCaminando", false);
            animator.SetBool("JugadorDetectado", false);
            animator.SetBool("EnemigoAlertado", false);

            animator.SetInteger("MovimientoCombate", 0);
        }

        if (enemyCombat == null)
        {
            enemyCombat = GetComponent<EnemyCombat>();
        }

        GameObject jugador =
            GameObject.FindGameObjectWithTag("Player");

        if (jugador != null)
        {
            player = jugador.transform;

            playerScript =
                player.GetComponent<ControlCamara>();

            posicionAnteriorJugador =
                player.position;
        }
        else
        {
            Debug.LogError(
                "❌ EnemyFollow: no se encontró Player."
            );
        }

        if (puntoVista == null)
        {
            puntoVista = transform;
        }

        alertaPermanente = false;
        enemigoAlertado = false;
        jugadorDetectado = false;

        disparosEscuchados = 0;

        direccionBusqueda = 1f;
        temporizadorBusqueda = 0f;

        temporizadorEvasion = 0f;
        duracionEvasionActual = 0f;

        movimientoEvasivoActivo = false;

        movimientoCombate = 0;

        if (puntosPatrulla != null &&
            puntosPatrulla.Length > 0)
        {
            indicePatrulla = 0;
            IrAlPuntoDePatrulla();
        }
        else
        {
            Debug.LogWarning(
                "⚠️ EnemyFollow: no hay puntos de patrulla."
            );

            DetenerEnemigo();
        }
    }

    // =====================================================
    // UPDATE
    // =====================================================

    void Update()
    {
        if (SceneManager.GetActiveScene().name !=
            escenaPermitida)
        {
            DetenerEnemigo();
            return;
        }

        if (player == null ||
            playerScript == null)
        {
            return;
        }

        if (alertaPermanente)
        {
            EstadoCombate();
            return;
        }

        if (enemigoAlertado)
        {
            EstadoAlertado();
            return;
        }

        ComprobarDeteccionNormal();

        if (!alertaPermanente &&
            !enemigoAlertado)
        {
            Patrullar();
        }
    }

    // =====================================================
    // DETECCIÓN NORMAL
    // =====================================================

    void ComprobarDeteccionNormal()
    {
        if (player == null)
            return;

        float distancia =
            Vector3.Distance(
                transform.position,
                player.position
            );

        float movimiento =
            Vector3.Distance(
                player.position,
                posicionAnteriorJugador
            );

        bool jugadorSeMueve =
            movimiento > 0.01f;

        posicionAnteriorJugador =
            player.position;

        bool jugadorAgachado =
            playerScript.agachado;

        bool puedeVerJugador = false;

        if (distancia <= distanciaDeteccion)
        {
            puedeVerJugador =
                PuedeVerAlJugador();
        }

        bool puedeEscucharJugador = false;

        if (puedeEscucharPasos &&
            jugadorSeMueve &&
            !jugadorAgachado &&
            distancia <= distanciaDeteccion)
        {
            puedeEscucharJugador = true;
        }

        if (puedeVerJugador ||
            puedeEscucharJugador)
        {
            DetectarJugador("detección normal");
        }
    }

    // =====================================================
    // RECIBIR DISPARO
    // =====================================================

    public void RecibirDisparo()
    {
        if (player == null)
            return;

        Debug.Log(
            "🚨 ENEMIGO RECIBIÓ DISPARO DIRECTO."
        );

        DetectarJugador("impacto directo");

        disparosEscuchados =
            disparosParaDetectarJugador;
    }

    // =====================================================
    // ESCUCHAR DISPARO
    // =====================================================

    public void EscucharDisparo(
        Vector3 posicionDisparo)
    {
        if (alertaPermanente)
            return;

        float distancia =
            Vector3.Distance(
                transform.position,
                posicionDisparo
            );

        if (distancia > radioEscuchaDisparo)
            return;

        disparosEscuchados++;

        Debug.Log(
            "🔊 DISPARO LEJANO ESCUCHADO #"
            + disparosEscuchados
            + " / "
            + disparosParaDetectarJugador
        );

        if (disparosEscuchados >=
            disparosParaDetectarJugador)
        {
            DetectarJugador("cuarto disparo");
            return;
        }

        ActivarAlerta();
    }

    // =====================================================
    // ACTIVAR ALERTA
    // =====================================================

    void ActivarAlerta()
    {
        enemigoAlertado = true;

        direccionBusqueda = 1f;
        temporizadorBusqueda = 0f;

        if (agent != null &&
            agent.enabled)
        {
            agent.ResetPath();
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        SetAnimacionMovimiento(0);

        if (animator != null)
        {
            animator.SetBool(
                "JugadorDetectado",
                false
            );

            animator.SetBool(
                "EnemigoAlertado",
                true
            );

            animator.SetBool(
                "EstaCaminando",
                false
            );
        }
    }

    // =====================================================
    // ESTADO ALERTADO
    // =====================================================

    void EstadoAlertado()
    {
        if (agent != null &&
            agent.enabled)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

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
                true
            );
        }

        SetAnimacionMovimiento(0);

        BuscarJugadorMientrasEstaAlertado();

        if (PuedeVerAlJugador())
        {
            DetectarJugador(
                "enemigo alertado encontró al jugador"
            );
        }
    }

    // =====================================================
    // BUSCAR JUGADOR
    // =====================================================

    void BuscarJugadorMientrasEstaAlertado()
    {
        if (!buscarMientrasEstaAlertado)
            return;

        temporizadorBusqueda +=
            Time.deltaTime;

        float giro =
            velocidadBusqueda *
            direccionBusqueda *
            Time.deltaTime;

        transform.Rotate(
            Vector3.up,
            giro
        );

        if (temporizadorBusqueda >=
            tiempoCambioBusqueda)
        {
            direccionBusqueda *= -1f;
            temporizadorBusqueda = 0f;
        }
    }

    // =====================================================
    // DETECTAR JUGADOR
    // =====================================================

    void DetectarJugador(string motivo)
    {
        alertaPermanente = true;

        jugadorDetectado = true;
        enemigoAlertado = false;

        movimientoEvasivoActivo = false;

        temporizadorEvasion = 0f;
        duracionEvasionActual = 0f;

        SetAnimacionMovimiento(0);

        if (animator != null)
        {
            animator.SetBool(
                "EnemigoAlertado",
                false
            );

            animator.SetBool(
                "JugadorDetectado",
                true
            );

            animator.SetBool(
                "EstaCaminando",
                true
            );
        }

        if (agent != null &&
            agent.enabled)
        {
            agent.ResetPath();

            agent.isStopped = false;

            agent.updateRotation = true;
        }

        Debug.Log(
            "🚨 JUGADOR DETECTADO: "
            + motivo
        );
    }

    // =====================================================
    // ESTADO COMBATE
    // =====================================================

    void EstadoCombate()
    {
        jugadorDetectado = true;

        enemigoAlertado = false;

        float distancia =
            Vector3.Distance(
                transform.position,
                player.position
            );

        if (distancia >
            distanciaMaximaCombate)
        {
            PerseguirJugador();
            return;
        }

        EntrarEnCombate();
    }

    // =====================================================
    // PATRULLAR
    // =====================================================

    void Patrullar()
    {
        if (puntosPatrulla == null ||
            puntosPatrulla.Length == 0)
        {
            DetenerEnemigo();
            return;
        }

        if (agent == null ||
            !agent.enabled)
        {
            return;
        }

        if (animator != null)
        {
            animator.SetBool(
                "JugadorDetectado",
                false
            );

            animator.SetBool(
                "EnemigoAlertado",
                false
            );

            animator.SetBool(
                "EstaCaminando",
                true
            );
        }

        SetAnimacionMovimiento(0);

        agent.updateRotation = true;
        agent.isStopped = false;

        Transform puntoActual =
            puntosPatrulla[indicePatrulla];

        if (puntoActual == null)
        {
            AvanzarSiguientePunto();
            return;
        }

        float distanciaAlPunto =
            Vector3.Distance(
                transform.position,
                puntoActual.position
            );

        if (distanciaAlPunto <=
            distanciaLlegadaPatrulla)
        {
            AvanzarSiguientePunto();
            return;
        }

        if (!agent.hasPath ||
            agent.isPathStale)
        {
            agent.SetDestination(
                puntoActual.position
            );
        }
    }

    // =====================================================
    // SIGUIENTE PUNTO
    // =====================================================

    void AvanzarSiguientePunto()
    {
        if (puntosPatrulla == null ||
            puntosPatrulla.Length == 0)
        {
            return;
        }

        indicePatrulla++;

        if (indicePatrulla >=
            puntosPatrulla.Length)
        {
            if (patrullaRepetitiva)
            {
                indicePatrulla = 0;
            }
            else
            {
                indicePatrulla =
                    puntosPatrulla.Length - 1;

                DetenerEnemigo();
                return;
            }
        }

        IrAlPuntoDePatrulla();
    }

    // =====================================================
    // IR A PUNTO
    // =====================================================

    void IrAlPuntoDePatrulla()
    {
        if (agent == null ||
            !agent.enabled ||
            puntosPatrulla == null ||
            puntosPatrulla.Length == 0)
        {
            return;
        }

        Transform punto =
            puntosPatrulla[indicePatrulla];

        if (punto == null)
            return;

        SetAnimacionMovimiento(0);

        if (animator != null)
        {
            animator.SetBool(
                "JugadorDetectado",
                false
            );

            animator.SetBool(
                "EnemigoAlertado",
                false
            );

            animator.SetBool(
                "EstaCaminando",
                true
            );
        }

        agent.updateRotation = true;
        agent.isStopped = false;

        agent.SetDestination(
            punto.position
        );
    }

    // =====================================================
    // ENTRAR EN COMBATE
    // =====================================================

    void EntrarEnCombate()
    {
        if (enemyCombat != null)
        {
            enemyCombat.estaEnCombate = true;
        }

        if (animator != null)
        {
            animator.SetBool(
                "JugadorDetectado",
                true
            );

            animator.SetBool(
                "EstaCaminando",
                true
            );
        }

        // IMPORTANTE:
        // Si ya está realizando una evasión,
        // NO volver a poner MovimientoCombate = 0.

        if (movimientoEvasivoActivo)
        {
            EjecutarMovimientoEvasivo();
        }
        else if (usarMovimientoEvasivo)
        {
            EjecutarMovimientoEvasivo();
        }
        else
        {
            MovimientoCombateNormal();
        }

        MirarAlJugador();
    }

    // =====================================================
    // PERSEGUIR JUGADOR
    // =====================================================

    void PerseguirJugador()
    {
        if (enemyCombat != null)
        {
            enemyCombat.estaEnCombate = false;
        }

        movimientoEvasivoActivo = false;
        temporizadorEvasion = 0f;

        SetAnimacionMovimiento(0);

        if (agent != null &&
            agent.enabled)
        {
            Vector3 direccion =
                player.position -
                transform.position;

            direccion.y = 0f;

            if (direccion.sqrMagnitude > 0.01f)
            {
                direccion.Normalize();

                Vector3 destino =
                    player.position -
                    direccion *
                    distanciaIdealCombate;

                NavMeshHit hit;

                if (NavMesh.SamplePosition(
                    destino,
                    out hit,
                    2f,
                    NavMesh.AllAreas
                ))
                {
                    agent.updateRotation = true;
                    agent.isStopped = false;

                    agent.SetDestination(
                        hit.position
                    );
                }
            }
        }

        if (animator != null)
        {
            animator.SetBool(
                "JugadorDetectado",
                true
            );

            animator.SetBool(
                "EstaCaminando",
                true
            );

            animator.SetBool(
                "EnemigoAlertado",
                false
            );
        }
    }

    // =====================================================
    // MOVIMIENTO COMBATE NORMAL
    // =====================================================

    void MovimientoCombateNormal()
    {
        if (agent == null ||
            !agent.enabled ||
            player == null)
        {
            return;
        }

        // Si todavía está ejecutando una evasión,
        // no se debe modificar su animación.

        if (movimientoEvasivoActivo)
        {
            return;
        }

        movimientoCombate = 0;

        float distancia =
            Vector3.Distance(
                transform.position,
                player.position
            );

        if (distancia <
            distanciaMinimaCombate)
        {
            Vector3 direccion =
                transform.position -
                player.position;

            direccion.y = 0f;

            if (direccion.sqrMagnitude > 0.01f)
            {
                direccion.Normalize();

                Vector3 destino =
                    player.position +
                    direccion *
                    distanciaIdealCombate;

                NavMeshHit hit;

                if (NavMesh.SamplePosition(
                    destino,
                    out hit,
                    2f,
                    NavMesh.AllAreas
                ))
                {
                    agent.updateRotation = false;
                    agent.isStopped = false;

                    agent.SetDestination(
                        hit.position
                    );
                }
            }
        }
        else if (distancia <=
                 distanciaMaximaCombate)
        {
            agent.ResetPath();

            agent.isStopped = true;
            agent.velocity = Vector3.zero;

            agent.updateRotation = false;
        }
        else
        {
            PerseguirJugador();
            return;
        }

        // CaminaApunta
        SetAnimacionMovimiento(0);

        if (animator != null)
        {
            animator.SetBool(
                "JugadorDetectado",
                true
            );

            animator.SetBool(
                "EstaCaminando",
                true
            );
        }
    }

    // =====================================================
    // MOVIMIENTO EVASIVO
    // =====================================================

    void EjecutarMovimientoEvasivo()
    {
        if (agent == null ||
            !agent.enabled ||
            player == null)
        {
            return;
        }

        temporizadorEvasion +=
            Time.deltaTime;

        // =================================================
        // EVASIÓN ACTIVA
        // =================================================

        if (movimientoEvasivoActivo)
        {
            agent.isStopped = false;

            agent.SetDestination(
                destinoEvasion
            );

            // IMPORTANTE:
            // Mantenemos el valor 1, 2 o 3.
            // NO llamamos MovimientoCombateNormal()
            // mientras la evasión esté activa.

            SetAnimacionMovimiento(
                movimientoCombate
            );

            if (animator != null)
            {
                animator.SetBool(
                    "JugadorDetectado",
                    true
                );

                animator.SetBool(
                    "EstaCaminando",
                    true
                );
            }

            MirarAlJugador();

            float distanciaDestino =
                Vector3.Distance(
                    transform.position,
                    destinoEvasion
                );

            if (distanciaDestino <= 0.35f ||
                temporizadorEvasion >=
                duracionEvasionActual)
            {
                movimientoEvasivoActivo = false;

                temporizadorEvasion = 0f;

                duracionEvasionActual = 0f;

                // Al terminar vuelve a CaminaApunta
                SetAnimacionMovimiento(0);

                MovimientoCombateNormal();
            }

            return;
        }

        // =================================================
        // TIEMPO ENTRE EVASIONES
        // =================================================

        if (temporizadorEvasion <
            tiempoMinimoEvasion)
        {
            MovimientoCombateNormal();

            MirarAlJugador();

            return;
        }

        // =================================================
        // NUEVA EVASIÓN
        // =================================================

        CrearNuevoMovimientoEvasivo();
    }

    // =====================================================
    // CREAR NUEVA EVASIÓN
    // =====================================================

    void CrearNuevoMovimientoEvasivo()
    {
        if (player == null ||
            agent == null ||
            !agent.enabled)
        {
            return;
        }

        Vector3 direccionAlJugador =
            player.position -
            transform.position;

        direccionAlJugador.y = 0f;

        if (direccionAlJugador.sqrMagnitude <
            0.01f)
        {
            return;
        }

        direccionAlJugador.Normalize();

        Vector3 derecha =
            Vector3.Cross(
                Vector3.up,
                direccionAlJugador
            ).normalized;

        Vector3 izquierda =
            -derecha;

        Vector3 destino;

        float aleatorio =
            Random.value;

        // =================================================
        // LATERAL
        // =================================================

        if (aleatorio <=
            probabilidadLateral)
        {
            bool moverDerecha =
                Random.value > 0.5f;

            Vector3 lateral =
                moverDerecha
                ? derecha
                : izquierda;

            // DERECHA = 2
            // IZQUIERDA = 3
            movimientoCombate =
                moverDerecha ? 2 : 3;

            Vector3 posicionBase =
                player.position -
                direccionAlJugador *
                distanciaIdealCombate;

            destino =
                posicionBase +
                lateral *
                distanciaLateral;
        }

        // =================================================
        // REVERSA
        // =================================================

        else
        {
            // REVERSA = 1
            movimientoCombate = 1;

            destino =
                transform.position -
                direccionAlJugador *
                distanciaReversa;
        }

        // =================================================
        // VALIDAR NAVMESH
        // =================================================

        NavMeshHit hit;

        if (!NavMesh.SamplePosition(
            destino,
            out hit,
            1.5f,
            NavMesh.AllAreas
        ))
        {
            SetAnimacionMovimiento(0);

            MovimientoCombateNormal();

            return;
        }

        // =================================================
        // DISTANCIA MÍNIMA
        // =================================================

        float distanciaAlDestino =
            Vector3.Distance(
                hit.position,
                player.position
            );

        if (distanciaAlDestino <
            distanciaMinimaCombate)
        {
            SetAnimacionMovimiento(0);

            MovimientoCombateNormal();

            return;
        }

        // =================================================
        // CONFIGURAR EVASIÓN
        // =================================================

        destinoEvasion =
            hit.position;

        movimientoEvasivoActivo = true;

        temporizadorEvasion = 0f;

        duracionEvasionActual =
            Random.Range(
                tiempoMinimoEvasion,
                tiempoMaximoEvasion
            );

        // =================================================
        // NAVMESH
        // =================================================

        agent.updateRotation = false;
        agent.isStopped = false;

        agent.SetDestination(
            destinoEvasion
        );

        // =================================================
        // ANIMACIÓN
        // =================================================

        SetAnimacionMovimiento(
            movimientoCombate
        );

        if (animator != null)
        {
            animator.SetBool(
                "JugadorDetectado",
                true
            );

            animator.SetBool(
                "EstaCaminando",
                true
            );
        }

        Debug.Log(
            "🏃 EVASIÓN | MovimientoCombate = "
            + movimientoCombate
            + " | "
            + ObtenerNombreMovimiento(
                movimientoCombate
            )
        );
    }

    // =====================================================
    // CONTROL CENTRAL DE ANIMACIÓN
    // =====================================================

    void SetAnimacionMovimiento(int movimiento)
    {
        movimiento =
            Mathf.Clamp(
                movimiento,
                0,
                3
            );

        movimientoCombate =
            movimiento;

        if (animator == null)
            return;

        animator.SetInteger(
            "MovimientoCombate",
            movimiento
        );
    }

    // =====================================================
    // NOMBRE DEL MOVIMIENTO
    // =====================================================

    string ObtenerNombreMovimiento(
        int movimiento)
    {
        switch (movimiento)
        {
            case 0:
                return "CaminaApunta";

            case 1:
                return "CaminaReversa";

            case 2:
                return "CaminaLateral";

            case 3:
                return "EnemigoCaminaLateral";

            default:
                return "Desconocido";
        }
    }

    // =====================================================
    // MIRAR AL JUGADOR
    // =====================================================

    void MirarAlJugador()
    {
        if (player == null)
            return;

        Vector3 direccion =
            player.position -
            transform.position;

        direccion.y = 0f;

        if (direccion.sqrMagnitude <
            0.01f)
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
    // DETENER ENEMIGO
    // =====================================================

    void DetenerEnemigo()
    {
        if (agent != null &&
            agent.enabled)
        {
            agent.ResetPath();

            agent.isStopped = true;

            agent.velocity =
                Vector3.zero;

            agent.updateRotation = true;
        }

        movimientoEvasivoActivo = false;

        temporizadorEvasion = 0f;

        movimientoCombate = 0;

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
        }

        SetAnimacionMovimiento(0);

        if (enemyCombat != null)
        {
            enemyCombat.estaEnCombate = false;
        }
    }

    // =====================================================
    // VISIÓN
    // =====================================================

    bool PuedeVerAlJugador()
    {
        if (player == null ||
            puntoVista == null)
        {
            return false;
        }

        Vector3 origen =
            puntoVista.position;

        Vector3 destino =
            player.position +
            Vector3.up * 1f;

        Vector3 direccion =
            destino - origen;

        float distancia =
            direccion.magnitude;

        if (distancia <= 0.01f)
        {
            return false;
        }

        RaycastHit hit;

        if (Physics.Raycast(
            origen,
            direccion.normalized,
            out hit,
            distancia,
            capasVision
        ))
        {
            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }
}