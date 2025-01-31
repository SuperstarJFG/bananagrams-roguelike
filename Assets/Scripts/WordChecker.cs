using Microsoft.Data.Sqlite;
using System;
using TMPro;
using UnityEngine;

public class WordChecker : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool CheckWord(string searchWord)
    {
        if (searchWord.Length == 1)
        {
            return true;
        }

        string connectionString = "Data Source=Assets\\NWL2023.db";
        SqliteConnection connection = new SqliteConnection(connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM words WHERE word=@searchWord";
        command.Parameters.AddWithValue("@searchWord", searchWord);

        Debug.Log("Executing query: " + searchWord);

        using (var reader = command.ExecuteReader())
        {
            if (!reader.HasRows)
            {
                Debug.Log("No results found");
                return false;
            }

            while (reader.Read())
            {
                // Get values by column name instead of index for better reliability
                var word = reader["word"]?.ToString();
                var definition = reader["definition"]?.ToString();
                Debug.Log($"Word: {word}, Definition: {definition}");
            }
            return true;
        }
    }

    //public string RandomWord()
    //{

    //}
}