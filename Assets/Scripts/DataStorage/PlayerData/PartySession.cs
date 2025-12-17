using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartySession : MonoBehaviour
{
    public static PartySession instance;

    public CharacterSnapshot[] characters = new CharacterSnapshot[4];

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
}

