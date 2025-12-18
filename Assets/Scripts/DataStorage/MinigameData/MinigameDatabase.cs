using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MinigameDatabase", menuName = "Data/Minigame Database")]
public class MinigameDatabase : ScriptableObject
{
    public MinigameData[] minigames;

    public List<MinigameData> GetMinigamesByType(MinigameType type)
    {
        return minigames.Where(m => m.type == type).ToList();
    }
}