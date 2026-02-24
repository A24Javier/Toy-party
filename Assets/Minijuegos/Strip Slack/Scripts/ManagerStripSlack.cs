using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ManagerStripSlack : MonoBehaviour
{
    enum BattleVS { PlayerVsPlayer, PlayerVsIA}
    BattleVS StatBattle;

    //varibale para comprbar que es ia
    bool isIA = true;

    //variable contoler time 
    float timeJoc = 30;

    public IAController ia;
    public PlayersControllers player;

    [SerializeField] TextMeshProUGUI[] texts;
    [SerializeField] GameObject textovictoria;

    private void Start()
    {
        //Aqu� habra que decedeir con funicon que devulva booleano o booleano para aplicar el estado concreto
        if (isIA)
        {
            StatBattle = BattleVS.PlayerVsIA;
        }
        else
        {
            StatBattle = BattleVS.PlayerVsPlayer;
        }
        textovictoria.SetActive(false);
    }

    private void FixedUpdate()
    {
        switch (StatBattle)
        {
            case BattleVS.PlayerVsPlayer:
                PlayerBattle();
                break;
            case BattleVS.PlayerVsIA:
                IACombate();
                break;
        }
        timeController();
        texts[1].text = "Fuerza del jugador: " + player.forcePlayer.ToString();
        texts[2].text = "Fuerza de la IA: " + ia.forceIA.ToString();
    }

    void IACombate()
    {
        
    }

    void PlayerBattle()
    {
        
    }
    void ActualizarInterfaz()
    {
        // Formateamos para mostrar Minutos:Segundos
        float minutos = Mathf.FloorToInt(timeJoc / 60);
        float segundos = Mathf.FloorToInt(timeJoc % 60);
        texts[0].text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }
    void timeController()
    {
        if (timeJoc > 1)
        {
            timeJoc -= Time.deltaTime;
            ActualizarInterfaz();
        }
        else
        {
            //aplicar la logica para volver al tablero
            //condicion de victoria
            textovictoria.SetActive(true);
            if (player.forcePlayer > ia.forceIA)
            {
                Debug.Log("Victoria del jugador");
                texts[3].text = "Victoria del jugador";
            }
            else
            {
                Debug.Log("Vicotria de la IA");
                texts[3].text = "Vicotria de la IA";
            }
            Time.timeScale = 0;
        }
    }
}
