using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static string lastSpawnPoint = "";

    public static bool tieneArma = false;

    // 🔥 NUEVO
    public static bool refugioCompletado = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}