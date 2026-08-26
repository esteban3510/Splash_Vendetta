using UnityEngine;

public class PlayerPersistente : MonoBehaviour
{
    private static PlayerPersistente instancia;

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // evita duplicados
        }
    }
}