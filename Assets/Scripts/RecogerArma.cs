using UnityEngine;
using UnityEngine.SceneManagement;

public class RecogerArma : MonoBehaviour
{
    [Header("Configuración")]
    public float distancia = 3f;
    public LayerMask capaArma;

    [Header("Armas")]
    public GameObject armaEnMesa;
    public GameObject armaMano;
    public GameObject manos;

    [Header("UI")]
    public GameObject textoRecoger;

    [Header("Disparo")]
    public ControlDisparo controlDisparo;

    private GameObject armaCercana;

    void Start()
    {
        // Revisar el estado del arma al iniciar la escena
        ComprobarEstadoArma();
    }

    void OnEnable()
    {
        // Se ejecuta cuando este objeto vuelve a estar activo
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= AlCargarEscena;
    }

    void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        ComprobarEstadoArma();
    }

    void ComprobarEstadoArma()
    {
        // =====================================================
        // EL JUGADOR YA TIENE EL ARMA
        // =====================================================

        if (GameManager.tieneArma)
        {
            // Buscar el arma de la mesa de esta escena
            GameObject armaMesa = GameObject.Find("Mod_Arma_Mesa");

            if (armaMesa != null)
            {
                armaMesa.SetActive(false);

                Debug.Log("✅ Mod_Arma_Mesa ocultada. El jugador ya tiene el arma.");
            }
            else
            {
                Debug.Log("ℹ️ No se encontró Mod_Arma_Mesa en esta escena.");
            }

            // Activar arma del jugador
            ActivarArmaVisual();

            // Ocultar texto de recoger
            if (textoRecoger != null)
            {
                textoRecoger.SetActive(false);
            }

            // Activar disparo
            if (controlDisparo != null)
            {
                controlDisparo.ActivarArma();
            }
        }
    }

    void Update()
    {
        // Si ya tiene el arma, no puede recoger otra
        if (GameManager.tieneArma)
            return;

        if (textoRecoger == null)
            return;

        DetectarArma();

        if (armaCercana != null)
        {
            textoRecoger.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                Recoger();
            }
        }
        else
        {
            textoRecoger.SetActive(false);
        }
    }

    void DetectarArma()
    {
        RaycastHit hit;

        if (Physics.Raycast(
            Camera.main.transform.position,
            Camera.main.transform.forward,
            out hit,
            distancia,
            capaArma))
        {
            armaCercana = hit.collider.gameObject;
        }
        else
        {
            armaCercana = null;
        }
    }

    void Recoger()
    {
        // =====================================================
        // GUARDAR ESTADO GLOBAL
        // =====================================================

        GameManager.tieneArma = true;

        // Desactivar arma de mesa
        if (armaEnMesa != null)
        {
            armaEnMesa.SetActive(false);
        }
        else if (armaCercana != null)
        {
            armaCercana.SetActive(false);
        }

        // Activar arma en la mano
        ActivarArmaVisual();

        // Ocultar texto
        if (textoRecoger != null)
        {
            textoRecoger.SetActive(false);
        }

        // Activar disparo
        if (controlDisparo != null)
        {
            controlDisparo.ActivarArma();

            Debug.Log("✅ Arma recogida. GameManager.tieneArma = TRUE");
        }
    }

    void ActivarArmaVisual()
    {
        if (armaMano != null)
        {
            armaMano.SetActive(true);
        }

        if (manos != null)
        {
            manos.SetActive(true);
        }
    }
}