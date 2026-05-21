using MongoDB.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsPage_AbilityCoroutine : MonoBehaviour
{
    public static MsPage_AbilityCoroutine Instance;

    void Awake()
    {
        if(Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartCoroutineAbility(Character charac)
    {
        StartCoroutine(UseAbility(charac));
    }

    private IEnumerator UseAbility(Character mspage)
    {
        float rand = Random.Range(0, 101);

        // Hacer animación de pintar
        yield return new WaitForSeconds(12f);

        if (rand >= 0 && rand <= 47.5f) // Dibuja y gana 4 monedas
        {
            UIManager.instance.FunctionUpdateTextCoins(mspage, 4);
            mspage.usingAbility = false;
        }
        else if (rand > 47.5f && rand <= 95) // Dibuja una bomba que hace retroceder a un rival
        {
            UIManager.instance.ConfigureSelectPlayer(mspage, "Bomb", 3);
        }
        else // Se crea un portal y se usa al instante
        {
            UIManager.instance.ConfigureSelectPlayer(mspage, "TP_OtherPlayer");
        }
    }
}
