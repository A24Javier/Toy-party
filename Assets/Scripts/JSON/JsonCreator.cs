using System.IO;
using UnityEngine;

public class JsonCreator : MonoBehaviour
{
    public static JsonCreator Instance;

    private void Start()
    {
        if (Instance != this && Instance != null) { Destroy(gameObject); return; }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveStats(Stats stats)
    {
        string json = JsonUtility.ToJson(stats, true);

        string path = Path.Combine(Application.persistentDataPath, "stats.json");

        File.WriteAllText(path, json);

        Debug.Log("Stats guardadas en: " + path);
    }

    public Stats LoadStats()
    {
        string path = Path.Combine(Application.persistentDataPath, "stats.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<Stats>(json);
        }

        Debug.LogWarning("No existe archivo de stats.");
        return new Stats();
    }
}
