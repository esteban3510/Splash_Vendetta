using UnityEngine;

public class ControlDisparo : MonoBehaviour
{
    [Header("Configuración del Proyectil")]
    public GameObject prefabProyectil;
    public Transform puntoDisparo;
    public float fuerzaDisparo = 30f;

    private bool miraInicializada = false;
    private bool armaInicializada = false;

    [Header("Cámara")]
    public Camera camara;

    [Header("Apuntado / Zoom")]
    public float fovNormal = 60f;
    public float fovApuntando = 20f;
    public float velocidadZoom = 10f;

    private bool estaApuntando = false;

    [Header("Mira")]
    public RectTransform crosshair;

    [Header("Audio")]
    public AudioSource audioDisparo;

    [Header("Control de disparo")]
    public bool puedeDisparar = false;

    private VidaJugador vidaJugador;


    // =====================================================
    // INICIO
    // =====================================================

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        vidaJugador = GetComponent<VidaJugador>();

        // -------------------------------------------------
        // Buscar cámara automáticamente
        // -------------------------------------------------

        if (camara == null)
        {
            camara = Camera.main;
        }

        // -------------------------------------------------
        // Guardar FOV normal de la cámara
        // -------------------------------------------------

        if (camara != null)
        {
            camara.fieldOfView = fovNormal;
        }

        // -------------------------------------------------
        // Comprobar si el jugador ya tiene el arma
        // -------------------------------------------------

        if (GameManager.tieneArma)
        {
            puedeDisparar = true;
            miraInicializada = false;
        }
    }


    // =====================================================
    // INICIALIZAR MIRA
    // =====================================================

    void IntentarInicializarMira()
    {
        if (miraInicializada)
            return;

        // -------------------------------------------------
        // Buscar MiraPersistente si no tenemos crosshair
        // -------------------------------------------------

        if (crosshair == null)
        {
            MiraPersistente mira =
                FindFirstObjectByType<MiraPersistente>();

            if (mira != null)
            {
                Transform t =
                    mira.transform.Find("Mira");

                if (t != null)
                {
                    crosshair =
                        t.GetComponent<RectTransform>();
                }
                else
                {
                    Debug.LogWarning(
                        "⚠️ No se encontró un hijo llamado 'Mira'."
                    );
                }
            }
        }

        // -------------------------------------------------
        // Activar / desactivar mira
        // -------------------------------------------------

        if (crosshair != null)
        {
            bool estado =
                GameManager.tieneArma;

            crosshair.gameObject.SetActive(estado);

            puedeDisparar = estado;

            // Tamaño normal de la mira
            crosshair.sizeDelta =
                new Vector2(40f, 40f);

            miraInicializada = true;

            Debug.Log(
                "✅ Mira inicializada correctamente."
            );
        }
    }


    // =====================================================
    // UPDATE
    // =====================================================

    void Update()
    {
        // =====================================================
        // JUGADOR MUERTO
        // =====================================================

        if (vidaJugador != null &&
            vidaJugador.estaMuerto)
        {
            // Mientras está muerto NO bloqueamos el cursor.
            // Esto permite usar el botón Reintentar.
            return;
        }


        // =====================================================
        // JUGADOR VIVO
        // =====================================================

        // Asegurar que el cursor siempre vuelva a ser
        // la mira del juego.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


        // =====================================================
        // INICIALIZAR MIRA
        // =====================================================

        IntentarInicializarMira();


        // =====================================================
        // REACTIVAR ARMA
        // =====================================================

        if (GameManager.tieneArma &&
            !armaInicializada)
        {
            ActivarArma();

            armaInicializada = true;

            Debug.Log(
                "✅ Arma reactivada automáticamente."
            );
        }


        // =====================================================
        // ACTUALIZAR ZOOM
        // =====================================================

        ActualizarZoom();


        // =====================================================
        // COMPROBAR SI PUEDE DISPARAR
        // =====================================================

        if (!puedeDisparar)
            return;


        // =====================================================
        // DISPARO
        // =====================================================

        if (Input.GetMouseButtonDown(0))
        {
            DispararCapsula();
        }
    }


    // =====================================================
    // ZOOM / APUNTADO
    // =====================================================

    void ActualizarZoom()
    {
        // Si no tenemos cámara, no hacemos nada
        if (camara == null)
            return;

        // Si el jugador no tiene arma,
        // no puede apuntar.
        if (!GameManager.tieneArma)
        {
            estaApuntando = false;
        }
        else
        {
            // Clic derecho mantenido = apuntar
            estaApuntando =
                Input.GetMouseButton(1);
        }

        // -------------------------------------------------
        // Determinar FOV objetivo
        // -------------------------------------------------

        float fovObjetivo =
            estaApuntando
            ? fovApuntando
            : fovNormal;

        // -------------------------------------------------
        // Transición suave
        // -------------------------------------------------

        camara.fieldOfView =
            Mathf.Lerp(
                camara.fieldOfView,
                fovObjetivo,
                velocidadZoom * Time.deltaTime
            );
    }


    // =====================================================
    // DISPARAR
    // =====================================================

    void DispararCapsula()
    {
        // -------------------------------------------------
        // Comprobar referencias
        // -------------------------------------------------

        if (prefabProyectil == null)
        {
            Debug.LogWarning(
                "⚠️ ControlDisparo: falta prefabProyectil."
            );

            return;
        }

        if (puntoDisparo == null)
        {
            Debug.LogWarning(
                "⚠️ ControlDisparo: falta puntoDisparo."
            );

            return;
        }

        if (camara == null)
        {
            Debug.LogWarning(
                "⚠️ ControlDisparo: falta cámara."
            );

            return;
        }


        // =================================================
        // 1. RAYO DESDE EL CENTRO DE LA MIRA
        // =================================================

        Ray ray =
            camara.ViewportPointToRay(
                new Vector3(
                    0.5f,
                    0.5f,
                    0f
                )
            );


        // =================================================
        // 2. DETERMINAR PUNTO DE APUNTADO
        // =================================================

        Vector3 puntoObjetivo;

        RaycastHit hit;

        if (Physics.Raycast(
            ray,
            out hit,
            1000f
        ))
        {
            // La mira está apuntando a un objeto.

            puntoObjetivo =
                hit.point;

            Debug.DrawLine(
                ray.origin,
                puntoObjetivo,
                Color.red,
                1f
            );
        }
        else
        {
            // No hay ningún objeto a 1000 metros.

            puntoObjetivo =
                ray.origin +
                ray.direction * 1000f;

            Debug.DrawLine(
                ray.origin,
                puntoObjetivo,
                Color.yellow,
                1f
            );
        }


        // =================================================
        // 3. CALCULAR DIRECCIÓN DESDE EL ARMA
        // =================================================

        Vector3 direccion =
            puntoObjetivo -
            puntoDisparo.position;

        if (direccion.sqrMagnitude <= 0.001f)
        {
            Debug.LogWarning(
                "⚠️ Dirección del disparo inválida."
            );

            return;
        }

        direccion.Normalize();


        // =================================================
        // 4. ROTACIÓN DEL PROYECTIL
        // =================================================

        Quaternion rotacion =
            Quaternion.LookRotation(
                direccion
            );


        // =================================================
        // 5. CREAR PROYECTIL
        // =================================================

        GameObject nuevaBala =
            Instantiate(
                prefabProyectil,
                puntoDisparo.position,
                rotacion
            );


        // =================================================
        // 6. APLICAR FUERZA
        // =================================================

        Rigidbody rb =
            nuevaBala.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Limpiar movimiento previo
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.AddForce(
                direccion *
                fuerzaDisparo,
                ForceMode.Impulse
            );
        }
        else
        {
            Debug.LogWarning(
                "⚠️ El proyectil no tiene Rigidbody."
            );
        }


        // =================================================
        // 7. EVITAR COLISIÓN CON EL JUGADOR
        // =================================================

        Collider balaCollider =
            nuevaBala.GetComponent<Collider>();

        Collider[] playerColliders =
            GetComponentsInChildren<Collider>(
                true
            );

        if (balaCollider != null)
        {
            foreach (Collider col
                in playerColliders)
            {
                if (col == null)
                    continue;

                // Evitar que la pintura choque
                // inmediatamente con el propio jugador.

                Physics.IgnoreCollision(
                    balaCollider,
                    col
                );
            }
        }


        // =================================================
        // 8. SONIDO
        // =================================================

        if (audioDisparo != null &&
            audioDisparo.clip != null)
        {
            audioDisparo.PlayOneShot(
                audioDisparo.clip
            );
        }


        // =================================================
        // 9. ANIMACIÓN DE LA MIRA
        // =================================================

        AnimarMira();
    }


    // =====================================================
    // ANIMACIÓN DE MIRA
    // =====================================================

    void AnimarMira()
    {
        if (crosshair == null)
            return;

        if (!crosshair.gameObject.activeSelf)
            return;

        // -------------------------------------------------
        // Cancelar animación anterior
        // -------------------------------------------------

        CancelInvoke(
            nameof(ResetCrosshair)
        );

        // -------------------------------------------------
        // Aumentar tamaño
        // -------------------------------------------------

        crosshair.sizeDelta =
            new Vector2(
                80f,
                80f
            );

        // -------------------------------------------------
        // Volver al tamaño normal
        // -------------------------------------------------

        Invoke(
            nameof(ResetCrosshair),
            0.05f
        );
    }


    // =====================================================
    // REINICIAR MIRA
    // =====================================================

    void ResetCrosshair()
    {
        if (crosshair == null)
            return;

        crosshair.sizeDelta =
            new Vector2(
                40f,
                40f
            );
    }


    // =====================================================
    // ACTIVAR ARMA
    // =====================================================

    public void ActivarArma()
    {
        GameManager.tieneArma = true;

        puedeDisparar = true;

        // Permitir volver a buscar la mira
        // después de cambiar de escena.

        miraInicializada = false;

        if (crosshair == null)
        {
            IntentarInicializarMira();
        }

        if (crosshair != null)
        {
            crosshair.gameObject.SetActive(true);

            crosshair.sizeDelta =
                new Vector2(
                    40f,
                    40f
                );
        }
    }
}