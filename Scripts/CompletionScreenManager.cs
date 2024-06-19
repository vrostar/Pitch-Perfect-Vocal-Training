using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CompletionScreenManager : MonoBehaviour
{
    public GameObject completionScreen;
    public TextMeshProUGUI finalScoreText; // For showing score on stat screen
    public TextMeshProUGUI noteStatsText; // For showing note stats on stat screen

    void Start()
    {
        // Hide the completion screen at the start
        completionScreen.SetActive(false);
    }

    public void Setup(int finalScore, Dictionary<string, int> noteHits, Dictionary<string, int> noteMisses)
    {
        finalScoreText.text = finalScore.ToString();
        completionScreen.SetActive(true);

        // Display the note stats as text
        noteStatsText.text = "";
        // For each note display the hits and misses
        foreach (var note in noteHits.Keys)
        {
            noteStatsText.text += $"{note}: Hits = {noteHits[note]}, Misses = {noteMisses[note]}\n";
        }
    }

    // Return to level select
    public void ReturnToLevelSelect()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("PPMainMenu");
    }
}
