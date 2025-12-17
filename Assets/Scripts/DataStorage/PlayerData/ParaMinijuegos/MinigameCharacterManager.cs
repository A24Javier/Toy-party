using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameCharacterManager : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private Character[] spawnedCharacters;

    void Start()
    {
        SpawnCharacters();
    }

    // ======================
    // SPAWN
    // ======================
    private void SpawnCharacters()
    {
        spawnedCharacters = new Character[PartySession.instance.characters.Length];

        for (int i = 0; i < PartySession.instance.characters.Length; i++)
        {
            CharacterSnapshot snap = PartySession.instance.characters[i];

            GameObject go = Instantiate(
                characterPrefab,
                spawnPoints[i].position,
                Quaternion.identity
            );

            Character c = go.GetComponent<Character>();

            // Restaurar datos
            c.characterId = snap.characterId;
            c.coins = snap.coins;
            c.stars = snap.stars;
            c.isPlayer = snap.isPlayer;
            c.characterImage = snap.characterImage;

            // Añadir IA si es NPC
            if (!c.isPlayer)
            {
                go.AddComponent<NpcMinigameAI>();
            }

            spawnedCharacters[i] = c;
        }
    }

    // ======================
    // RESULTADOS
    // ======================
    public void SaveResults()
    {
        for (int i = 0; i < spawnedCharacters.Length; i++)
        {
            Character c = spawnedCharacters[i];
            CharacterSnapshot snap = PartySession.instance.characters[i];

            snap.coins = c.coins;
            snap.stars = c.stars;
        }
    }

    // ======================
    // ACCESO
    // ======================
    public Character[] GetCharacters()
    {
        return spawnedCharacters;
    }
}