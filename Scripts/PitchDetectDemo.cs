using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Runtime.InteropServices;
using TMPro;
using System.Collections.Generic;

public class PitchDetectDemo : MonoBehaviour
{
    [DllImport("AudioPluginDemo")]
    private static extern float PitchDetectorGetFreq(int index);

    [DllImport("AudioPluginDemo")]
    private static extern int PitchDetectorDebug(float[] data);

    float[] history = new float[1000];
    float[] debug = new float[65536];

    string[] noteNames = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

    public Material mat;
    public string frequency = "detected frequency";
    public string note = "detected note";
    public AudioMixer mixer;
    public InfoText pitchText;
    public PlayableDirector playableDirector;
    public List<string> targetNotes = new List<string> { "C", "D", "E", "B", "A", "G" }; // Add all the target notes you will be using in your level here
    public int playerScore = 0;

    public TextMeshProUGUI scoreText;
    public CompletionScreenManager completionScreenManager;

    private Dictionary<string, bool> isScoring = new Dictionary<string, bool>();
    private Dictionary<string, int> noteHits = new Dictionary<string, int>();
    private Dictionary<string, int> noteMisses = new Dictionary<string, int>();

    void Start()
    {
        // Ensure PlayableDirector is assigned
        if (playableDirector == null)
        {
            Debug.LogError("PlayableDirector is not assigned!");
        }

        // Initialize scoring states for each target note
        foreach (var note in targetNotes)
        {
            isScoring[note] = false;
            noteHits[note] = 0;
            noteMisses[note] = 0;
        }
    }

    void Update()
    {
        float freq = PitchDetectorGetFreq(0), deviation = 0.0f;
        frequency = freq.ToString() + " Hz";

        if (freq > 0.0f)
        {
            float noteval = 57.0f + 12.0f * Mathf.Log10(freq / 440.0f) / Mathf.Log10(2.0f);
            float f = Mathf.Floor(noteval + 0.5f);
            deviation = Mathf.Floor((noteval - f) * 100.0f);
            int noteIndex = (int)f % 12;
            int octave = (int)Mathf.Floor((noteval + 0.5f) / 12.0f);
            note = noteNames[noteIndex] + " " + octave;

            // Scoring functionality for eacht target note
            foreach (var targetNote in targetNotes)
            {
                // Check if the target note isScoring
                if (isScoring[targetNote])
                {
                    if (noteNames[noteIndex] == targetNote)
                    {
                        Debug.Log($"Gaining points for {targetNote}!");
                        playerScore++; // Award points for hitting the correct note
                        noteHits[targetNote]++; // Award correct note hits for hitting the correct note
                    }
                    else
                    {
                        Debug.Log("Not gaining points :(");
                        noteMisses[targetNote]++; // Award note misses for missing the note
                    }
                }
            }
        }
        else
        {
            note = "unknown";
        }

        if (pitchText != null)
        {
            pitchText.text = "FREQUENCY: " + frequency + "\nNOTE: " + note + " (deviation: " + deviation + " cents)"; // Show te sung note, frequency and note deviation
        }

        if (scoreText != null)
        {
            scoreText.text = "Score: " + playerScore.ToString();
        }
    }

    public void ShowCompletionScreen()
    {
        completionScreenManager.Setup(playerScore, noteHits, noteMisses);
    }

    public void DisableInfoText()
    {
        if (pitchText != null)
        {
            pitchText.gameObject.SetActive(false);
        }
    }

    // Start scoring functions to be used with Timeline Signals for scoring functionality
    public void StartScoringSignalC() { StartScoring("C"); }
    public void StartScoringSignalD() { StartScoring("D"); }
    public void StartScoringSignalE() { StartScoring("E"); }
    public void StartScoringSignalB() { StartScoring("B"); }
    public void StartScoringSignalA() { StartScoring("A"); }
    public void StartScoringSignalG() { StartScoring("G"); }

    // Stop scoring functions to be used with Timeline Signals for scoring functionality
    public void StopScoringSignalC() { StopScoring("C"); }
    public void StopScoringSignalD() { StopScoring("D"); }
    public void StopScoringSignalE() { StopScoring("E"); }
    public void StopScoringSignalB() { StopScoring("B"); }
    public void StopScoringSignalA() { StopScoring("A"); }
    public void StopScoringSignalG() { StopScoring("G"); }

    // Start Scoring function begins scoring for the target note
    void StartScoring(string note)
    {
        Debug.Log($"Start scoring for {note}");
        isScoring[note] = true;
    }

    // Stop Scoring function stops scoring for the target note
    void StopScoring(string note)
    {
        Debug.Log($"Stopped scoring for {note}! Total score: " + playerScore);
        isScoring[note] = false;
    }

    Vector3 Plot(float[] data, int num, float x0, float y0, float w, float h, Color col, float thr)
    {
        GL.Begin(GL.LINES);
        GL.Color(col);
        float xs = w / num, ys = h;
        float px = 0, py = 0;
        for (int n = 1; n < num; n++)
        {
            float nx = x0 + n * xs, ny = y0 + data[n] * ys;
            if (n > 1 && data[n] > thr && data[n - 1] > thr)
            {
                GL.Vertex3(px, py, 0);
                GL.Vertex3(nx, ny, 0);
            }
            px = nx;
            py = ny;
        }
        GL.End();
        return new Vector3(x0 + w, py, 0);
    }

    void OnRenderObject()
    {
        mat.SetPass(0);

        GL.Begin(GL.LINES);
        GL.Color(Color.green);
        GL.Vertex3(-5, 0, 0);
        GL.Vertex3(5, 0, 0);
        GL.End();

        for (int n = 1; n < history.Length; n++)
            history[n - 1] = history[n];
        history[history.Length - 1] = PitchDetectorGetFreq(0);
        transform.position = Plot(history, history.Length, -45.0f, 0.0f, 50.0f, 0.01f, Color.blue, 0.1f);

        int num = PitchDetectorDebug(debug);
        // Plot(debug, num, -5.0f, 1.0f, 10.0f, 0.0002f, Color.red, 0.1f);
    }

    // void OnGUI()
    // {
    //     float monitor;
    //     if (mixer != null && mixer.GetFloat("Monitor", out monitor))
    //     {
    //         GUILayout.Space(30);
    //         if (GUILayout.Button(monitor > 0.0f ? "Monitoring is ON" : "Monitoring is OFF"))
    //             monitor = (monitor > 0.0f) ? 0.0f : 0.5f;
    //         mixer.SetFloat("Monitor", monitor);
    //     }
    // }
}
