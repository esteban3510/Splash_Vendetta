using UnityEngine;

public class ProyectilPintura : MonoBehaviour
{
    [Header("Impacto visual")]
    public GameObject decalPrefab;
    public float tiempoVidaDecal = 5f;

    [Header("Luz del impacto")]
    public float intensidadLuz = 5f;
    public float rangoLuz = 3f;

    // =====================================================
    // ESTADO DEL PROYECTIL
    // =====================================================

    private bool yaImpacto = false;


    // =====================================================
    // COLISIÓN DEL PROYECTIL
    // =====================================================

    private void OnCollisionEnter(Collision collision)
    {
        // =================================================
        // 1. EVITAR DOBLE PROCESAMIENTO
        // =================================================

        if (yaImpacto)
            return;


        // =================================================
        // 2. IGNORAR JUGADOR
        // =================================================

        if (collision.gameObject.CompareTag("Player"))
        {
            return;
        }


        // =================================================
        // 3. IGNORAR ARMA
        // =================================================

        if (collision.gameObject.name.Contains("Arma"))
        {
            return;
        }


        // =================================================
        // 4. IGNORAR ELEMENTOS DEL DISPARO
        // =================================================

        if (collision.gameObject.name.Contains("Disparo"))
        {
            return;
        }


        // =================================================
        // 5. BLOQUEAR EL PROYECTIL
        // =================================================

        yaImpacto = true;


        // =================================================
        // 6. COMPROBAR CONTACTO
        // =================================================

        if (collision.contactCount <= 0)
        {
            DestruirProyectil();
            return;
        }


        // =================================================
        // 7. OBTENER INFORMACIÓN DEL IMPACTO
        // =================================================

        ContactPoint contacto =
            collision.GetContact(0);

        Vector3 puntoImpacto =
            contacto.point;

        Vector3 normalImpacto =
            contacto.normal;


        Debug.Log(
            "🎯 PROYECTIL IMPACTÓ EN: "
            + collision.gameObject.name
            + " | POSICIÓN: "
            + puntoImpacto
        );


        // =================================================
        // 8. COMPROBAR SI GOLPEÓ AL ENEMIGO
        // =================================================

        VidaEnemigo vidaEnemigo =
            collision.gameObject.GetComponentInParent<VidaEnemigo>();


        if (vidaEnemigo != null)
        {
            Debug.Log(
                "💥 PROYECTIL IMPACTÓ DIRECTAMENTE AL ENEMIGO."
            );


            // =================================================
            // DAÑO
            // =================================================

            vidaEnemigo.RecibirDaño(25f);


            // =================================================
            // IMPORTANTE
            //
            // NO llamamos EscucharDisparo().
            //
            // VidaEnemigo.RecibirDaño() ya llama:
            //
            // EnemyFollow.RecibirDisparo()
            //
            // Por lo tanto este disparo solamente cuenta
            // como detección inmediata UNA VEZ.
            // =================================================


            CrearImpactoVisual(
                puntoImpacto,
                normalImpacto
            );


            DestruirProyectil();

            return;
        }


        // =================================================
        // 9. COMPROBAR MANIQUÍ
        // =================================================

        Objetivo objetivo =
            collision.gameObject.GetComponentInParent<Objetivo>();


        if (objetivo != null)
        {
            objetivo.Impacto();

            Debug.Log(
                "🎯 PROYECTIL IMPACTÓ EN MANIQUÍ."
            );
        }


        // =================================================
        // 10. AVISAR A LOS ENEMIGOS
        // =================================================
        //
        // Solo se ejecuta si NO golpeamos directamente
        // al enemigo.
        //
        // Por lo tanto:
        //
        // DISPARO 1 → 1 aviso
        // DISPARO 2 → 1 aviso
        // DISPARO 3 → 1 aviso
        // DISPARO 4 → 1 aviso
        //
        // Nunca dos avisos por el mismo proyectil.
        // =================================================

        AvisarEnemigosDelDisparo(
            puntoImpacto
        );


        // =================================================
        // 11. CREAR IMPACTO VISUAL
        // =================================================

        CrearImpactoVisual(
            puntoImpacto,
            normalImpacto
        );


        // =================================================
        // 12. DESTRUIR PROYECTIL
        // =================================================

        DestruirProyectil();
    }


    // =====================================================
    // AVISAR A LOS ENEMIGOS
    // =====================================================

    private void AvisarEnemigosDelDisparo(
        Vector3 posicionImpacto)
    {
        EnemyFollow[] enemigos =
            FindObjectsByType<EnemyFollow>(
                FindObjectsSortMode.None
            );


        foreach (EnemyFollow enemigo in enemigos)
        {
            if (enemigo == null)
                continue;


            enemigo.EscucharDisparo(
                posicionImpacto
            );
        }


        Debug.Log(
            "🔊 IMPACTO DE PINTURA: "
            + "enemigos notificados UNA SOLA VEZ."
        );
    }


    // =====================================================
    // CREAR IMPACTO VISUAL
    // =====================================================

    private void CrearImpactoVisual(
        Vector3 puntoImpacto,
        Vector3 normalImpacto)
    {
        if (decalPrefab == null)
            return;


        // =================================================
        // ROTACIÓN DEL DECAL
        // =================================================

        Quaternion rotacionImpacto =
            Quaternion.LookRotation(
                -normalImpacto
            );


        // =================================================
        // CREAR DECAL
        // =================================================

        GameObject decal =
            Instantiate(
                decalPrefab,
                puntoImpacto +
                normalImpacto * 0.05f,
                rotacionImpacto
            );


        // =================================================
        // ROTACIÓN ALEATORIA
        // =================================================

        decal.transform.Rotate(
            0f,
            0f,
            Random.Range(
                0f,
                360f
            )
        );


        // =================================================
        // COLORES
        // =================================================

        Color verdeNeon =
            new Color(
                0.2f,
                1f,
                0.2f
            );

        Color naranjaNeon =
            new Color(
                1f,
                0.5f,
                0f
            );


        Color colorFinal =
            Random.value > 0.5f
            ? verdeNeon
            : naranjaNeon;


        // =================================================
        // LUZ DEL IMPACTO
        // =================================================
        //
        // ESTA ES LA PARTE QUE FUNCIONABA BIEN
        // EN TU VERSIÓN ORIGINAL.
        //
        // La posición se establece en WORLD SPACE,
        // antes de hacerla hija del decal.
        //
        // De esta forma la luz queda delante de la
        // superficie y no dentro de ella.
        // =================================================

        GameObject luz =
            new GameObject(
                "LuzImpacto"
            );


        luz.transform.position =
            puntoImpacto +
            normalImpacto * 0.8f;


        Light lightComp =
            luz.AddComponent<Light>();


        lightComp.color =
            colorFinal;

        lightComp.intensity =
            intensidadLuz;

        lightComp.range =
            rangoLuz;

        lightComp.shadows =
            LightShadows.None;


        // =================================================
        // HACER LA LUZ HIJA DEL DECAL
        // =================================================

        luz.transform.SetParent(
            decal.transform
        );


        // =================================================
        // IMPORTANTE
        // =================================================
        //
        // NO modificamos aquí localPosition.
        //
        // Al hacer SetParent() sin conservar una posición
        // local explícita, Unity mantiene la posición
        // mundial de la luz.
        //
        // Esto conserva el comportamiento visual que
        // tenías en tu código original.
        // =================================================


        // =================================================
        // DESTRUIR DECAL
        // =================================================

        Destroy(
            decal,
            tiempoVidaDecal
        );
    }


    // =====================================================
    // DESTRUIR PROYECTIL
    // =====================================================

    private void DestruirProyectil()
    {
        Destroy(
            gameObject
        );
    }
}