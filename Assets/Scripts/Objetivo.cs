using UnityEngine;

public class Objetivo : MonoBehaviour
{
    private bool yaContado = false;

    public void Impacto()
    {
        if (yaContado) return;

        yaContado = true;

        FindFirstObjectByType<SistemaMision>().ObjetivoCompletado();
    }
}