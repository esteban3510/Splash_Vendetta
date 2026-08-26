using UnityEngine;
using TMPro;
using System.Collections;

public class SistemaMision : MonoBehaviour
{
    public TextMeshProUGUI textoMision;

    public int objetivosTotales = 3;
    private int objetivosCompletados = 0;

    public GameObject paredBloqueo;

    public float tiempoMostrarCompletado = 3f;

    void Start()
    {
        // 🔥 Validación extra (por seguridad)
        if (textoMision == null) return;

        if (GameManager.refugioCompletado)
        {
            objetivosCompletados = objetivosTotales;

            // 🚫 Ocultar UI si ya estaba completado
            textoMision.gameObject.SetActive(false);

            if (paredBloqueo != null)
            {
                paredBloqueo.SetActive(false);
            }

            return;
        }

        ActualizarUI();
    }

    public void ObjetivoCompletado()
    {
        objetivosCompletados++;

        Debug.Log("Objetivos: " + objetivosCompletados);

        ActualizarUI();

        if (objetivosCompletados >= objetivosTotales)
        {
            GameManager.refugioCompletado = true;

            if (paredBloqueo != null)
            {
                paredBloqueo.SetActive(false);
            }

            StartCoroutine(MostrarMisionCompletada());
        }
    }

    void ActualizarUI()
    {
        // 🔥 Validación clave
        if (textoMision == null) return;

        textoMision.text = "BLANCOS ELIMINADOS:\n" + objetivosCompletados + " / " + objetivosTotales;
    }

    IEnumerator MostrarMisionCompletada()
    {
        // 🔥 Validar antes de usar
        if (textoMision == null) yield break;

        textoMision.text = "ENTRENAMIENTO COMPLETADO";

        yield return new WaitForSeconds(tiempoMostrarCompletado);

        // 🔥 Validar otra vez (esto evita el error)
        if (textoMision != null)
        {
            textoMision.gameObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
        // 🔥 MUY IMPORTANTE: detener corutinas al cambiar de escena
        StopAllCoroutines();
    }
}