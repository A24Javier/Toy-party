using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartySession : MonoBehaviour
{
    public static PartySession instance;

    public CharacterSnapshot[] characters = new CharacterSnapshot[4];
    public BoardSessionData boardState = new BoardSessionData();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCharacters()
    {
        Character[] charac = GameController.instance.characters;
        characters = new CharacterSnapshot[charac.Length];

        for(int i = 0; i < characters.Length; i++)
        {
            characters[i] = new CharacterSnapshot();

            characters[i].characterId = charac[i].characterId;
            characters[i].coins = charac[i].coins;
            characters[i].stars = charac[i].stars;
            characters[i].isPlayer = charac[i].isPlayer;
            characters[i].characterImage = charac[i].characterImage;
            characters[i].actualBoxIndex = Board.instance.GetBoxIndex(charac[i].actualBox);
            characters[i].characterSettingIndex = charac[i].characterSettingIndex;
        }
    }
}