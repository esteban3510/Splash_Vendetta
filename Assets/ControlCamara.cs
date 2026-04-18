using UnityEngine;

public class ControlCamara : MonoBehaviour
{
    // Esta variable controla qué tan rápido se mueve la cámara
    public float velocidad = 5.0f;

    void Update()
    {
        // Captura las teclas W, S (Vertical) y A, D (Horizontal)
        float movimientoX = Input.GetAxis("Horizontal") * velocidad * Time.deltaTime;
        float movimientoZ = Input.GetAxis("Vertical") * velocidad * Time.deltaTime;

        // Aplica el movimiento a la cámara
        transform.Translate(movimientoX, 0, movimientoZ);
    }
}