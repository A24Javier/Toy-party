using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class Stats
{
    public float Playtime;
    public int GamesPlayed;
}

public class PlayerStats : MonoBehaviour
{
    private Stats _stats;

    [SerializeField]
    [Tooltip("Time that will be doing autosave (Provide time in seconds)")]
    private float _autosaveTime = 300f;

    private float _autosaveElapsedTime = 0f;

    [SerializeField]
    private string _statsFilename = "PlayerStats.save";

    public static PlayerStats Instance;

    void Awake()
    {
        if(Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        // Carga el archivo de guardado en caso de tener
        _stats = LoadData();
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    void Update()
    {
        _stats.Playtime += Time.unscaledDeltaTime;
        _autosaveElapsedTime += Time.unscaledDeltaTime;

        if(_autosaveElapsedTime >= _autosaveTime)
        {
            _autosaveElapsedTime = 0f;
            SaveData();
        }
    }

    private Stats LoadData()
    {
        string path = Path.Combine(Application.persistentDataPath, _statsFilename);

        if (File.Exists(path))
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);
                Stats data = formatter.Deserialize(stream) as Stats;
                stream.Close();
                return data;
            }
            catch(Exception e)
            {
                Debug.Log("Archivo corrupto, generamos uno nuevo");
                return new Stats();
            }
            
        }
        else
        {
            Debug.LogError("Save file not found at " + path);
            return new Stats();
        }
    }

    public void SaveData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Path.Combine(Application.persistentDataPath, _statsFilename);
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, _stats);
        stream.Close();
        Debug.Log("Stats saved correctly");
    }

    public void OneGameMore()
    {
        _stats.GamesPlayed++;
    }
}
