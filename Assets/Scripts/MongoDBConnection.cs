using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class MongoDBConnection : MonoBehaviour
{
    private MongoClient client;
    private IMongoDatabase database;
    private IMongoCollection<BsonDocument> usersCollection;

    private const string connectionString = "mongodb+srv://admin:admin@toypartyanalitics.unayhe1.mongodb.net/?retryWrites=true&w=majority&appName=ToyPartyAnalitics";

    public static MongoDBConnection Instance;

    void Awake()
    {
        if(Instance != this && Instance != null) { Destroy(gameObject); return; }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    async Task InitializeMongo()
    {
        client = new MongoClient(connectionString);
        database = client.GetDatabase("ToyPartyAnalitics");
        usersCollection = database.GetCollection<BsonDocument>("UserTelemetry");

        await database.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));
    }

    async void Start()
    {
        try
        {
            await InitializeMongo();
            Debug.Log("Conexión a MongoDB OK");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

    }

    public async void InsertData(BsonDocument bsonDoc) // Función para insertar datos en la base de datos
    {
        try
        {
            await usersCollection.InsertOneAsync(bsonDoc);
            Debug.Log("Data insertada correctamente");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Cannot insert bsonDoc into Database: " + e.Message);
        }

    }

}
