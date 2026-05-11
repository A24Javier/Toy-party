using System.Collections.Generic;

public static class MinigameResultSession
{
    public static List<MinigameResultData> Results = new List<MinigameResultData>();

    public static void Clear()
    {
        Results.Clear();
    }

    public static void AddResult(int characterId, int position, int coinsWon)
    {
        Results.Add(new MinigameResultData
        {
            characterId = characterId,
            position = position,
            coinsWon = coinsWon
        });
    }
}