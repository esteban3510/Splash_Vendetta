using UnityEngine;

public class ManiquiMovimiento : MonoBehaviour
{
    [Header("Movimiento")]
    public float distanciaMovimiento = 2f;
    public float velocidad = 0.5f;

    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.position;
    }

    void Update()
    {
        float movimiento = Mathf.Sin(Time.time * velocidad) * distanciaMovimiento;

        transform.position = posicionInicial + transform.right * movimiento;
    }
}