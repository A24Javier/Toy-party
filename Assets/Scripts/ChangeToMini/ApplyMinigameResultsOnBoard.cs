using UnityEngine;

public class ApplyMinigameResultsOnBoard : MonoBehaviour
{
    private void Start()
    {
        if (PartySession.instance == null) return;

        for (int i = 0; i < 4; i++)
        {
            CharacterSnapshot snap = PartySession.instance.characters[i];
            if (snap == null) continue;

            for (int j = 0; j < 4; j++)
            {
                Character boardChar = GameController.instance.GetCharacter(j);

                if (boardChar != null && boardChar.characterId == snap.characterId)
                {
                    boardChar.coins = snap.coins;
                    boardChar.stars = snap.stars;
                }
            }
        }
    }
}
