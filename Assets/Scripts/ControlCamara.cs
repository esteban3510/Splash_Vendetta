using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ControlCamara : MonoBehaviour
{
    public float velocidadMovimiento = 5f;
    public float velocidadAgachado = 2f;
    public float sensibilidadRaton = 2f;

    public Transform camara;

    [HideInInspector] public bool agachado = false;
    [HideInInspector] public bool estaMoviendose = false;

    public float alturaNormal = 2f;
    public float alturaAgachado = 1f;

    public float velocidadTransicion = 8f;

    private float rotacionX = 0f;
    private float rotacionY = 0f;

    private CharacterController controller;

    private VidaJugador vidaJugador;

    private float alturaActual;

    // 🔥 CLAVE: guardar altura real inicial
    private float camaraAlturaNormal;
    private float camaraAlturaAgachado;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
        vidaJugador = GetComponent<VidaJugador>();

        // 🔥 IMPORTANTE: tomar rotación actual del player
        rotacionY = transform.eulerAngles.y;

        // Guardar altura REAL de la cámara
        camaraAlturaNormal = camara.localPosition.y;
        camaraAlturaAgachado = camaraAlturaNormal * 0.5f;

        // Inicializar correctamente
        alturaActual = alturaNormal;
        controller.height = alturaNormal;
        controller.center = new Vector3(0, alturaNormal / 2f, 0);
    }

    void Update()
    {
        if (vidaJugador != null && vidaJugador.estaMuerto)
        {
            return;
        }

        // 🎯 ROTACIÓN
        float ratonX = Input.GetAxis("Mouse X") * sensibilidadRaton;
        float ratonY = Input.GetAxis("Mouse Y") * sensibilidadRaton;

        rotacionY += ratonX;
        rotacionX -= ratonY;

        rotacionX = Mathf.Clamp(rotacionX, -90f, 90f);

        transform.rotation = Quaternion.Euler(0f, rotacionY, 0f);
        camara.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);

        // 🎮 MOVIMIENTO
        float moverX = Input.GetAxis("Horizontal");
        float moverZ = Input.GetAxis("Vertical");

        Vector3 movimiento = transform.right * moverX + transform.forward * moverZ;

        //estaMoviendose = movimiento.magnitude > 0.1f;
        estaMoviendose = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;

        // 🕶️ TOGGLE AGACHARSE
        if (Input.GetKeyDown(KeyCode.C))
        {
            agachado = !agachado;
        }

        float velocidadActual = agachado ? velocidadAgachado : velocidadMovimiento;
        controller.Move(movimiento * velocidadActual * Time.deltaTime);

        // 🔽 ALTURA PLAYER
        float alturaObjetivo = agachado ? alturaAgachado : alturaNormal;
        alturaActual = Mathf.Lerp(alturaActual, alturaObjetivo, Time.deltaTime * velocidadTransicion);

        controller.height = alturaActual;
        controller.center = new Vector3(0, alturaActual / 2f, 0);

        // 🎥 ALTURA CÁMARA (USANDO VALOR REAL)
        Vector3 posCam = camara.localPosition;

        float objetivoCam = agachado ? camaraAlturaAgachado : camaraAlturaNormal;

        posCam.y = Mathf.Lerp(posCam.y, objetivoCam, Time.deltaTime * velocidadTransicion);
        camara.localPosition = posCam;
    }

    public void ActualizarRotacionInicial()
    {
        rotacionY = transform.eulerAngles.y;
        rotacionX = camara.localEulerAngles.x;

        if (rotacionX > 180f)
            rotacionX -= 360f;
    }
}