using UnityEngine;

public class ProyectilPintura : MonoBehaviour
{
    [Header("Efecto de Impacto")]
    public GameObject prefabSplashPintura;
    public float tiempoVidaSplash = 2f;

    private bool yaImpacto = false;

    private void OnCollisionEnter(Collision collision)
    {
        // Ignorar jugador y arma
        if (collision.gameObject.name.Contains("Arma") ||
            collision.gameObject.name.Contains("Disparo") ||
            collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        if (yaImpacto) return;
        yaImpacto = true;

        if (prefabSplashPintura != null)
        {
            ContactPoint contacto = collision.contacts[0];

            Vector3 normal = contacto.normal;

            // Separar un poco de la superficie
            Vector3 posicionSpawn =
                contacto.point + normal * 0.03f;

            // Rotación del impacto
            Quaternion rotacionImpacto =
                Quaternion.LookRotation(-normal);

            GameObject splash = Instantiate(
                prefabSplashPintura,
                posicionSpawn,
                rotacionImpacto
            );

            // Ajustar el hijo visual
            Transform impacto =
                splash.transform.Find("Impacto_Pintura_1");

            if (impacto != null)
            {
                impacto.localPosition = Vector3.zero;

                // Prueba primero con identidad
                impacto.localRotation = Quaternion.identity;
            }

            Destroy(splash, tiempoVidaSplash);
        }

        Destroy(gameObject);
    }
}