using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public string defaultSpawn = "Spawn_Default"; // 👈 IMPORTANTE

    void Start()
    {
        Debug.Log("SPAWN EJECUTADO");

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("No se encontró el Player con tag 'Player'");
            return;
        }

        string spawnName = GameManager.lastSpawnPoint;

        // 🔥 SI NO HAY SPAWN, USAMOS UNO POR DEFECTO
        if (string.IsNullOrEmpty(spawnName))
        {
            Debug.Log("No había spawn guardado, usando default");
            spawnName = defaultSpawn;
        }

        Debug.Log("Spawn buscado: " + spawnName);

        GameObject spawn = GameObject.Find(spawnName);

        if (spawn != null)
        {
            Debug.Log("Spawn encontrado: " + spawn.name);

            CharacterController cc = player.GetComponent<CharacterController>();

            if (cc != null)
                cc.enabled = false;

            player.transform.position = spawn.transform.position;
            player.transform.rotation = spawn.transform.rotation;

            ControlCamara controlCamara = player.GetComponent<ControlCamara>();

            if (controlCamara != null)
            {
                controlCamara.ActualizarRotacionInicial();
            }

            if (cc != null)
                cc.enabled = true;
        }
        else
        {
            Debug.LogError("NO se encontró el spawn: " + spawnName);
        }
        
    }
}