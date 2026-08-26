using UnityEngine;
using UnityEngine.SceneManagement;

public class Puerta : MonoBehaviour
{
    public string nombreEscena;
    public string spawnPointID;

    [Header("UI")]
    public GameObject textoUI;

    [Header("Configuración")]
    public float distanciaInteraccion = 3f;

    private Transform jugador;
    private bool cargandoEscena = false;

    void Start()
    {
        BuscarJugador();
        BuscarTextoPuerta();

        if (textoUI != null)
            textoUI.SetActive(false);
    }

    void Update()
    {
        if (cargandoEscena)
            return;

        // Buscar jugador nuevamente si se perdió la referencia
        if (jugador == null)
        {
            BuscarJugador();

            if (jugador == null)
                return;
        }

        // Buscar el texto nuevamente si la referencia se perdió
        if (textoUI == null)
        {
            BuscarTextoPuerta();
        }

        float distancia = Vector3.Distance(
            transform.position,
            jugador.position
        );

        bool jugadorCerca = distancia <= distanciaInteraccion;

        // Mostrar u ocultar el texto según la distancia
        if (textoUI != null)
        {
            textoUI.SetActive(jugadorCerca);
        }

        // Cambiar de escena
        if (jugadorCerca && Input.GetKeyDown(KeyCode.F))
        {
            CambiarEscena();
        }
    }

    void BuscarJugador()
    {
        GameObject objetoJugador = GameObject.FindGameObjectWithTag("Player");

        if (objetoJugador != null)
        {
            jugador = objetoJugador.transform;
        }
    }

    void BuscarTextoPuerta()
    {
        // Primero intenta utilizar la referencia del Inspector
        if (textoUI != null)
            return;

        MiraPersistente canvasPersistente =
            FindFirstObjectByType<MiraPersistente>();

        if (canvasPersistente != null)
        {
            Transform texto = canvasPersistente.transform.Find("TextoPuerta");

            if (texto != null)
            {
                textoUI = texto.gameObject;
            }
        }
    }

    void CambiarEscena()
    {
        if (cargandoEscena)
            return;

        cargandoEscena = true;

        // Ocultar el mensaje antes del cambio
        if (textoUI != null)
        {
            textoUI.SetActive(false);
        }

        // Guardar el punto de aparición
        GameManager.lastSpawnPoint = spawnPointID;

        // Cambiar de escena
        SceneManager.LoadScene(nombreEscena);
    }

    void OnDisable()
    {
        // Evita que el texto quede visible
        // si esta puerta es destruida al cambiar de escena.
        if (textoUI != null)
        {
            textoUI.SetActive(false);
        }
    }
}