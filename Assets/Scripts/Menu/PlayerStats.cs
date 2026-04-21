using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MongoDB.Bson;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Stats
{
    public int userID;

    public float Playtime;
    public int GamesPlayed;

    public int MinigamesPlayed;
    public int MinigamesWon;
    public int MinigamesLost;
}

public class PlayerStats : MonoBehaviour
{
    public Stats PStats { get; private set; }

    [SerializeField]
    [Tooltip("Time that will be doing autosave (Provide time in seconds)")]
    private float _autosaveTime = 300f;

    [SerializeField]
    [Tooltip("Time that will be putting info in the mongodb (Provide time in seconds)")]
    private float _telemetryTime = 300f;

    private float _autosaveElapsedTime = 0f;
    private float _telemetryElapsedTime = 0f;

    [SerializeField]
    private string _statsFilename = "PlayerStats.save";

    public static PlayerStats Instance;

    void Awake()
    {
        if(Instance != null && Instance != this) { Destroy(gameObject); return; }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Carga el archivo de guardado en caso de tener
        PStats = LoadData();
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SaveData();
    }

    void Update()
    {
        PStats.Playtime += Time.unscaledDeltaTime;

        _autosaveElapsedTime += Time.unscaledDeltaTime;
        _telemetryElapsedTime += Time.unscaledDeltaTime;

        if(_autosaveElapsedTime >= _autosaveTime)
        {
            _autosaveElapsedTime = 0f;
            SaveData();
        }

        if(_telemetryElapsedTime >= _telemetryTime)
        {
            _telemetryElapsedTime = 0f;
            UpdateDatabase();
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

                Debug.Log($"Playtime: {data.Playtime}, GamesPlayed: {data.GamesPlayed}");

                return data;
            }
            catch(Exception e)
            {
                Debug.LogError($"Archivo corrupto, generamos uno nuevo.\nException: {e}");
                return new Stats();
            }
            
        }
        else
        {
            Debug.LogError("Save file not found at " + path);
            Stats stats = new Stats();

            string guidStr = Guid.NewGuid().ToString();
            int guidInt = guidStr.GetHashCode();
            stats.userID = guidInt;

            return stats;
        }
    }

    public void SaveData()
    {
        string path = Path.Combine(Application.persistentDataPath, _statsFilename);
        FileStream stream = new FileStream(path, FileMode.Create);
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, PStats);

            Debug.Log("Stats saved correctly");
        }
        catch(Exception e)
        {
            Debug.LogError($"Error al guardar archivo. Exception: {e}");
        }
        finally
        {
            stream.Close();
        }

    }

    public void OneGameMore()
    {
        PStats.GamesPlayed++;
    }

    private void UpdateDatabase()
    {
        BsonDocument bsonDoc = CreateUserTelemetryBSON();
        MongoDBConnection.Instance?.InsertData(bsonDoc);
    }

    public BsonDocument CreateUserTelemetryBSON()
    {
        BsonDocument bsonDoc = new BsonDocument
        {
            { "UserID", PStats.userID },
            { "Playtime", PStats.Playtime },
            { "GamesPlayed", PStats.GamesPlayed },
            { "MinigamesPlayed", PStats.MinigamesPlayed },
            { "MinigamesWon", PStats.MinigamesWon },
            { "MinigamesLost", PStats.MinigamesLost }
        };
        return bsonDoc;
    }

}
