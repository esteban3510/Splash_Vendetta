using UnityEngine;

public class ControlDisparo : MonoBehaviour
{
    [Header("Configuración del Proyectil")]
    public GameObject prefabProyectil; // Aquí arrastraremos el Prefab de la bala
    public Transform puntoDisparo;      // Aquí arrastraremos el objeto Punto_Disparo
    public float fuerzaDisparo = 30f;   // Velocidad de salida del proyectil

    private AudioSource audioDisparo;

    void Start()
    {
        // Buscamos el AudioSource configurado previamente en la marcadora
        audioDisparo = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Detecta el clic izquierdo del ratón o el botón de disparo principal
        if (Input.GetMouseButtonDown(0))
        {
            DispararCápsula();
        }
    }

    void DispararCápsula()
    {
        if (prefabProyectil != null && puntoDisparo != null)
        {
            // 1. Clonar el Prefab en la posición y rotación del punto de disparo
            GameObject nuevaBala = Instantiate(prefabProyectil, puntoDisparo.position, puntoDisparo.rotation);

            // 2. Obtener el Rigidbody de la nueva bala para aplicarle fuerza física
            Rigidbody rb = nuevaBala.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Empujamos la bala hacia adelante (eje Z del punto de disparo)
                rb.AddForce(puntoDisparo.forward * fuerzaDisparo, ForceMode.Impulse);
            }

            // 3. Reproducir el sonido de aire comprimido mecánico
            if (audioDisparo != null)
            {
                audioDisparo.Play();
            }
        }
    }
}
