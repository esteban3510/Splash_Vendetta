using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class VidaJugador : MonoBehaviour
{
    [Header("Vida")]
    public float vidaMaxima = 100f;
    public float vidaActual;

    [HideInInspector] public bool estaMuerto = false;

    [Header("Barra de vida")]
    public Image barraVida;

    [Header("Pantalla de derrota")]
    public GameObject pantallaDerrota;

    [Header("Aviso visual de daño")]
    public Image avisoDaño;
    public float duracionAvisoDaño = 0.35f;

    private Coroutine corrutinaAvisoDaño;


    // =====================================================
    // INICIO
    // =====================================================

    void Start()
    {
        // Restaurar el cursor al entrar nuevamente a la escena
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        vidaActual = vidaMaxima;
        estaMuerto = false;

        ActualizarBarraVida();

        if (pantallaDerrota != null)
        {
            pantallaDerrota.SetActive(false);
        }

        // =================================================
        // CONFIGURAR AVISO DE DAÑO
        // =================================================

        if (avisoDaño != null)
        {
            // Muy importante:
            // El aviso visual NO debe bloquear botones.
            avisoDaño.raycastTarget = false;

            // Comenzar completamente transparente.
            Color color = avisoDaño.color;
            color.a = 0f;
            avisoDaño.color = color;
        }
        
    }


    // =====================================================
    // RECIBIR DAÑO
    // =====================================================

    public void RecibirDaño(float cantidad)
    {
        if (estaMuerto)
            return;

        vidaActual -= cantidad;

        vidaActual = Mathf.Clamp(
            vidaActual,
            0f,
            vidaMaxima
        );

        ActualizarBarraVida();

        // Mostrar aviso visual
        MostrarAvisoDaño();

        Debug.Log(
            "Jugador recibió daño. Vida actual: " +
            vidaActual
        );

        if (vidaActual <= 0f)
        {
            Morir();
        }
    }


    // =====================================================
    // MOSTRAR AVISO DE DAÑO
    // =====================================================

    void MostrarAvisoDaño()
    {
        if (avisoDaño == null)
            return;

        // Asegurarnos de que nunca bloquee la interfaz.
        avisoDaño.raycastTarget = false;

        // Si ya había una animación ejecutándose,
        // detenerla para reiniciar el efecto.
        if (corrutinaAvisoDaño != null)
        {
            StopCoroutine(corrutinaAvisoDaño);
        }

        corrutinaAvisoDaño =
            StartCoroutine(AnimarAvisoDaño());
    }


    // =====================================================
    // ANIMACIÓN DEL AVISO
    // =====================================================

    IEnumerator AnimarAvisoDaño()
    {
        Color color = avisoDaño.color;

        // Duraciones de cada parte del efecto.
        float tiempoAparicion = 0.08f;
        float tiempoMantener = 0.08f;

        // El resto de la duración se utiliza para desaparecer.
        float tiempoDesvanecer =
            Mathf.Max(
                0.05f,
                duracionAvisoDaño -
                tiempoAparicion -
                tiempoMantener
            );


        // =================================================
        // APARECER
        // =================================================

        float tiempo = 0f;

        while (tiempo < tiempoAparicion)
        {
            tiempo += Time.deltaTime;

            float progreso =
                tiempo / tiempoAparicion;

            color.a =
                Mathf.Lerp(
                    0f,
                    0.55f,
                    progreso
                );

            avisoDaño.color = color;

            yield return null;
        }


        // =================================================
        // MANTENER
        // =================================================

        color.a = 0.55f;
        avisoDaño.color = color;

        yield return new WaitForSeconds(
            tiempoMantener
        );


        // =================================================
        // DESVANECER
        // =================================================

        tiempo = 0f;

        while (tiempo < tiempoDesvanecer)
        {
            tiempo += Time.deltaTime;

            float progreso =
                tiempo / tiempoDesvanecer;

            color.a =
                Mathf.Lerp(
                    0.55f,
                    0f,
                    progreso
                );

            avisoDaño.color = color;

            yield return null;
        }


        // =================================================
        // FINAL
        // =================================================

        color.a = 0f;
        avisoDaño.color = color;

        corrutinaAvisoDaño = null;
    }


    // =====================================================
    // BARRA DE VIDA
    // =====================================================

    void ActualizarBarraVida()
    {
        if (barraVida != null)
        {
            barraVida.fillAmount =
                vidaActual / vidaMaxima;
        }
    }


    // =====================================================
    // MORIR
    // =====================================================

    void Morir()
    {
        if (estaMuerto)
            return;

        estaMuerto = true;

        // Detener el efecto de daño si estaba activo.
        if (corrutinaAvisoDaño != null)
        {
            StopCoroutine(corrutinaAvisoDaño);
            corrutinaAvisoDaño = null;
        }

        // Ocultar completamente el aviso.
        if (avisoDaño != null)
        {
            Color color = avisoDaño.color;
            color.a = 0f;
            avisoDaño.color = color;

            // Asegurar que no bloquee la interfaz.
            avisoDaño.raycastTarget = false;
        }

        Debug.Log("💀 EL JUGADOR HA MUERTO");

        if (pantallaDerrota != null)
        {
            pantallaDerrota.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    // =====================================================
    // REINTENTAR
    // =====================================================

    public void Reintentar()
    {
        vidaActual = vidaMaxima;
        estaMuerto = false;

        if (barraVida != null)
        {
            barraVida.fillAmount = 1f;
        }

        if (pantallaDerrota != null)
        {
            pantallaDerrota.SetActive(false);
        }

        // Asegurar que el aviso no interfiera
        // después de reiniciar.
        if (avisoDaño != null)
        {
            Color color = avisoDaño.color;
            color.a = 0f;
            avisoDaño.color = color;

            avisoDaño.raycastTarget = false;
        }

        // Al reintentar desde Level_01,
        // volver al punto de entrada del Refugio.
        GameManager.lastSpawnPoint =
            "Spawn_DesdeLevel01";

        SceneManager.LoadScene("El_Refugio");
    }
}