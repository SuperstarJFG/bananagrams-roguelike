using System.Collections.Generic;
using UnityEngine;

public class LetterCountTester : MonoBehaviour
{
    Dictionary<char, int> letterCount = new();
    int i = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        i++;

        char letter = WordChecker.RandomLetter();
        if (letterCount.ContainsKey(letter))
        {
            letterCount[letter]++;
        }
        else
        {
            letterCount[letter] = 1;
        }

        List<string> lines = new();
        foreach (var pair in letterCount)
        {
            lines.Add($"Letter: {pair.Key}, Count: {pair.Value}, Dist: {(float)pair.Value/i}");
        }
        lines.Sort();
        foreach (var line in lines)
        {
            Debug.Log(line);
        }
    }
}
