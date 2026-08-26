using UnityEngine;

public class PantallaInicioUI : MonoBehaviour
{
    public GameObject panelInicio;

    private static bool yaSeMostro = false;

    void Start()
    {
        // 👇 SI YA SE MOSTRÓ, ME ELIMINO
        if (yaSeMostro)
        {
            Destroy(gameObject);
            return;
        }

        // 👇 MARCAMOS QUE YA SE MOSTRÓ
        yaSeMostro = true;

        panelInicio.SetActive(true);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            panelInicio.SetActive(false);

            Time.timeScale = 1f;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // 👇 NOS DESTRUIMOS PARA SIEMPRE
            Destroy(gameObject);
        }
    }
}