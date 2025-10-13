using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using UnityEngine;

public class WordChecker : MonoBehaviour
{
    const string connectionString = "Data Source=Assets\\NWL2023.db";
    static SqliteConnection connection = new SqliteConnection(connectionString);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool CheckWord(string word)
    {
        if (word.Length == 1)
        {
            return true;
        }

        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM words WHERE word=@searchWord";
        command.Parameters.AddWithValue("@searchWord", word);

        //Debug.Log("Executing query: " + word);

        var reader = command.ExecuteReader();
        if (!reader.HasRows)
        {
            Debug.Log("No results found");
            return false;
        }

        while (reader.Read())
        {
            var definition = reader["definition"]?.ToString();
            //Debug.Log($"Word: {word}, Definition: {definition}");
        }
        return true;
    }

    public static string RandomWord()
    {
        if (connection.State != System.Data.ConnectionState.Open)
        {
            connection.Open();
        }

        using (var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT * FROM words ORDER BY RANDOM() LIMIT 1";
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var word = reader["word"]?.ToString();
                    var definition = reader["definition"]?.ToString();
                    //Debug.Log($"Word: {word}, Definition: {definition}");
                    return word;
                }
            }
        }

        //Debug.Log("No results found");
        return "";
    }

    // get random char from RandomWord()
    public static char RandomLetter()
    {
        string word = RandomWord();
        if (!string.IsNullOrEmpty(word))
        {
            return word[Random.Range(0, word.Length)];
        }
        return ' ';
    }
}