using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public float duration = 20f;
    private float timer = 0f;
    private bool finished = false;

    public TMP_Text tiempoText;

    public CestaPuntos playerCesta;
    public CestaPuntos npc1Cesta;
    public CestaPuntos npc2Cesta;
    public CestaPuntos npc3Cesta;

    public PlayerController player;
    public NPCControllerSimple npc1;
    public NPCControllerSimple npc2;
    public NPCControllerSimple npc3;

    void Update()
    {
        if (finished) return;

        timer += Time.deltaTime;

        // Mostrar tiempo restante
        float remaining = Mathf.Clamp(duration - timer, 0, duration);
        tiempoText.text = "" + remaining.ToString("F1"); 

        if (timer >= duration)
        {
            finished = true;
            tiempoText.text = "0.0";
            DetermineWinner();
        }
    }

    void DetermineWinner()
    {
        int[] scores = {
            playerCesta.puntos,
            npc1Cesta.puntos,
            npc2Cesta.puntos,
            npc3Cesta.puntos
        };

        int winnerIndex = 0;
        int maxScore = scores[0];

        for (int i = 1; i < scores.Length; i++)
        {
            if (scores[i] > maxScore)
            {
                maxScore = scores[i];
                winnerIndex = i;
            }
        }

        switch (winnerIndex)
        {
            case 0: player.Victory(); break;
            case 1: npc1.Victory(); break;
            case 2: npc2.Victory(); break;
            case 3: npc3.Victory(); break;
        }
    }
}
