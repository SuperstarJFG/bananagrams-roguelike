using Microsoft.Data.Sqlite;
using UnityEngine;

public class WordChecker : MonoBehaviour
{
    const string connectionString = "Data Source=Assets\\NWL2023.db";
    static SqliteConnection connection = new SqliteConnection(connectionString);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        connection.Open();
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

        Debug.Log("Executing query: " + word);

        var reader = command.ExecuteReader();
        if (!reader.HasRows)
        {
            Debug.Log("No results found");
            return false;
        }

        while (reader.Read())
        {
            var definition = reader["definition"]?.ToString();
            Debug.Log($"Word: {word}, Definition: {definition}");
        }
        return true;
    }

    public string RandomWord()
    {
        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM words ORDER BY RANDOM() LIMIT 1";

        var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var word = reader["word"]?.ToString();
            var definition = reader["definition"]?.ToString();
            Debug.Log($"Word: {word}, Definition: {definition}");
            return word;
        }

        Debug.Log("No results found");
        return "";
    }

    // get random char from RandomWord()
    public char RandomLetter()
    {
        string word = RandomWord();
        if (word.Length > 0)
        {
            return word[Random.Range(0, word.Length)];
        }
        return ' ';
    }
}