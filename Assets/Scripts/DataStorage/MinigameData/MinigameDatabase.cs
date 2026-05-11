using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MinigameDatabase", menuName = "Minigames/Database")]
public class MinigameDatabase : ScriptableObject
{
    public MinigameData[] minigames;

    public List<MinigameData> GetMinigamesByType(MinigameType type)
    {
        return minigames.Where(m => m.type == type).ToList();
    }

    public List<MinigameData> GetRoundEndMinigames()
    {
        return minigames
            .Where(m =>
                m.type == MinigameType.AllVSAll ||
                m.type == MinigameType.TwoVSTwo ||
                m.type == MinigameType.ThreeVSOne
            )
            .ToList();
    }
}