using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ControlCamara : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidadMovimiento = 5f;
    public float sensibilidadRaton = 2f;

    private float rotacionX = 0f;
    private float rotacionY = 0f;

    private CharacterController controller;

    void Start()
    {
        // Esto oculta y bloquea el cursor en el centro de la pantalla (como en un Shooter real)
        Cursor.lockState = CursorLockMode.Locked;

        // Obtenemos automáticamente el componente Character Controller asignado
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 1. LÓGICA DE ROTACIÓN (Apuntar con el ratón)
        float ratonX = Input.GetAxis("Mouse X") * sensibilidadRaton;
        float ratonY = Input.GetAxis("Mouse Y") * sensibilidadRaton;

        rotacionY += ratonX;
        rotacionX -= ratonY;

        // Limitamos la vista hacia arriba y abajo para no dar giros de 360 grados con el cuello
        rotacionX = Mathf.Clamp(rotacionX, -90f, 90f);

        // Aplicamos la transformación de Rotación
        transform.localRotation = Quaternion.Euler(rotacionX, rotacionY, 0f);

        // 2. LÓGICA DE POSICIÓN (Moverse con WASD respetando los muros)
        float moverX = Input.GetAxis("Horizontal");
        float moverZ = Input.GetAxis("Vertical");

        // Obtenemos las direcciones de la cámara y limpiamos el eje Y para evitar alteraciones de altura
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Calculamos la dirección final del movimiento horizontal
        Vector3 movimiento = (right * moverX) + (forward * moverZ);

        // Aplicamos el movimiento físico a través del Character Controller
        controller.Move(movimiento * velocidadMovimiento * Time.deltaTime);
    }
}