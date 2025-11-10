using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameController : MonoBehaviour
{
    private enum MinigameType { AllVSAll, TwoVSTwo, ThreeVSOne }

    [Header("Minijuegos")]
    [SerializeField] private MinigameType[] minigameType;
    [SerializeField] private string[] sceneName;
    private string minigameToLoad;

    private Dictionary<string, MinigameType> minigames = new Dictionary<string, MinigameType>();

    public static MinigameController instance;

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

    void Start()
    {
        CreateMinigamesDictionary();
    }

    /// <summary>
    /// Función para crear el diccionario
    /// </summary>
    private void CreateMinigamesDictionary()
    {
        for (int i = 0; i < sceneName.Length; i++)
        {
            minigames.Add(sceneName[i], minigameType[i]);
        }

        /*foreach(KeyValuePair<string, MinigameType> kvp in minigames)
        {
            Debug.Log("Minigame: " + kvp.Key);
            Debug.Log("MinigameType: " + kvp.Value + "\n------------");
        }*/

        Array.Clear(minigameType, 0, minigameType.Length);
        Array.Clear(sceneName, 0, sceneName.Length);
    }

    /// <summary>
    /// Función para seleccionar minijuego de manera aleatoria pasando un valor que es el tipo de minijuego que quieres
    /// </summary>
    /// <param name="strMinigameType"></param>
    public void SelectMinigame(string strMinigameType)
    {
        MinigameType minType = (MinigameType)Enum.Parse(typeof(MinigameType), strMinigameType);
        Debug.Log("MinType: " + minType.ToString());
        List<string> possibleMinigames = new List<string>();

        foreach(KeyValuePair<string, MinigameType> kvp in minigames)
        {
            // Dentro de la iteración 'Key' se refiere a string, y 'Value' se refiere a MinigameType
            if(kvp.Value == minType)
            {
                Debug.Log("Encuentra escena");
                possibleMinigames.Add(kvp.Key);
            }
        }

        int rand = UnityEngine.Random.Range(0, possibleMinigames.Count);
        minigameToLoad = possibleMinigames[rand];

        StartCoroutine(UIManager.instance.FadeInOut(true, LoadMinigame));
    }

    private void LoadMinigame()
    {
        SceneManager.LoadScene(minigameToLoad);
    }
}
