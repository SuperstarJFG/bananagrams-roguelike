using System.Collections.Generic;
using UnityEngine;

public class WordReader : MonoBehaviour
{
    TileManager tileManager;

    HashSet<List<GameObject>> lines;
    HashSet<List<GameObject>> incorrectLines;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tileManager = GetComponent<TileManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ReadWords()
    {
        lines = new();
        incorrectLines = new();

        // Find correct and incorrect words
        foreach (GameObject tile in tileManager.placedTiles)
        {
            LinesFromTile(tile);
        }

        // Print results and set colors
        foreach (GameObject tile in tileManager.placedTiles)
        {
            tile.GetComponent<SpriteRenderer>().color = Color.white;
        }
        if (incorrectLines.Count == 0)
        {
            Debug.Log("All words are correct!");
        }
        else
        {
            string words = "";
            foreach (List<GameObject> line in incorrectLines)
            {
                string word = "";
                foreach (GameObject letter in line)
                {
                    word += letter.GetComponent<LetterController>().letter;

                    letter.GetComponent<SpriteRenderer>().color = Color.red;
                }
                if (!words.Contains(word))
                {
                    words += word + ", ";
                }
            }
            Debug.LogError("Incorrect words: " + words);
        }
    }

    void LinesFromTile(GameObject tile)
    {
        Debug.Log("init letter: " + tile.GetComponent<LetterController>().letter);
        List<GameObject> horizontalLine = new()
        {
            tile
        };
        List<GameObject> verticalLine = new()
        {
            tile
        };

        // Build horizontal and vertical lines
        horizontalLine.InsertRange(0, CheckLeft(tile.transform.position));
        horizontalLine.AddRange(CheckRight(tile.transform.position));

        verticalLine.InsertRange(0, CheckUp(tile.transform.position));
        verticalLine.AddRange(CheckDown(tile.transform.position));

        lines.Add(horizontalLine);
        lines.Add(verticalLine);

        // Get incorrect lines
        foreach (List<GameObject> line in lines)
        {
            string word = "";
            foreach (GameObject letter in line)
            {
                word += letter.GetComponent<LetterController>().letter;
            }
            if (!GetComponent<WordChecker>().CheckWord(word))
            {
                incorrectLines.Add(line);
            }
        }
    }

    List<GameObject> CheckLeft(Vector3 position)
    {
        foreach (GameObject placedTile in tileManager.placedTiles)
        {
            if (placedTile.transform.position == position + new Vector3(-1f, 0f))
            {
                List<GameObject> ret = new();
                ret.InsertRange(0, CheckLeft(placedTile.transform.position));
                ret.Add(placedTile);
                return ret;
            }
        }
        return new();
    }

    List<GameObject> CheckRight(Vector3 position)
    {
        foreach (GameObject placedTile in tileManager.placedTiles)
        {
            if (placedTile.transform.position == position + new Vector3(1f, 0f))
            {
                List<GameObject> ret = new();
                ret.Add(placedTile);
                ret.AddRange(CheckRight(placedTile.transform.position));
                return ret;
            }
        }
        return new();
    }

    List<GameObject> CheckUp(Vector3 position)
    {
        foreach (GameObject placedTile in tileManager.placedTiles)
        {
            if (placedTile.transform.position == position + new Vector3(0f, 1f))
            {
                List<GameObject> ret = new();
                ret.AddRange(CheckUp(placedTile.transform.position));
                ret.Add(placedTile);
                return ret;
            }
        }
        return new();
    }

    List<GameObject> CheckDown(Vector3 position)
    {
        foreach (GameObject placedTile in tileManager.placedTiles)
        {
            if (placedTile.transform.position == position + new Vector3(0f, -1f))
            {
                List<GameObject> ret = new();
                ret.Add(placedTile);
                ret.AddRange(CheckDown(placedTile.transform.position));
                return ret;
            }
        }
        return new();
    }
}
