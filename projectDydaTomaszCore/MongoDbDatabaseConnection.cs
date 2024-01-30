﻿using Amazon.Auth.AccessControlPolicy.ActionIdentifiers;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using projectDydaTomasz.Core.Interfaces;
using projectDydaTomasz.Core.Models;
using projectDydaTomaszCore.Interfaces;
using projectDydaTomaszCore.Models;

public class MongoDbDatabaseConnection<T> : IDatabaseConnectionExtended<T>
{
    private MongoClient _mongoClient;
    private IMongoDatabase _database;
    private string _collectionName;

    public void Connect(string connectionString, string databaseName, string collectionName)
    {
        try
        {
            // Implementacja kodu do nawiązywania połączenia z bazą danych MongoDB.
            _mongoClient = new MongoClient(connectionString);
            _database = _mongoClient.GetDatabase(databaseName);
            _collectionName = collectionName;
            Console.WriteLine("Połączenie z bazą danych MongoDB nawiązane.");
        }
        catch
        {
            Console.WriteLine("BŁĄD POŁĄCZENIA");
        }
    }

    public void Disconnect()
    {
        // Implementacja kodu do zamykania połączenia z bazą danych MongoDB.
        Console.WriteLine("Połączenie z bazą danych MongoDB zamknięte.");
    }

    public IMongoCollection<T> GetCollection(string collectionName)
    {
        try
        {
            return _database.GetCollection<T>(collectionName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas pobierania kolekcji z bazy danych MongoDB: {ex.Message}");
            return null;
        }
    }

    public void AddToDb(T input)
    {
        try
        {
            var collection = GetCollection(_collectionName);
            collection.InsertOne(input);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas dodawania do bazy danych MongoDB: {ex.Message}");
        }
    }
    public T GetFilteredData(string property, string searchingTerm)
    {
        var filter = Builders<T>.Filter.Eq(property, searchingTerm);
        var collection = GetCollection(_collectionName);
        var result = collection.Find(filter).FirstOrDefault();
        return result;
    }

    public List<T> GetFilteredDataList(string property, string searchingTerm)
    {
        var filter = Builders<T>.Filter.Eq(property, searchingTerm);
        var collection = GetCollection(_collectionName);
        var result = collection.Find(filter).ToList();
        return result;
    }

    public List<T> GetAllDataList()
    {
        var collection = GetCollection(_collectionName);
        var result = collection.Find(_ => true).ToList();
        return result;
    }

    public void UpdateData(string property, string searchTerm, T updatingData)
    {
        var filter = Builders<T>.Filter.Eq(property, searchTerm);
        var collection = GetCollection(_collectionName);
        collection.ReplaceOne(filter, updatingData);
    }

    public void DeleteData(string property, string searchTerm)
    {
        var filter = Builders<T>.Filter.Eq(property, searchTerm);
        var collection = GetCollection(_collectionName);
        collection.DeleteOne(filter);
    }

}
