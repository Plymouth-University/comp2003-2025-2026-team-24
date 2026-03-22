using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScoreEntry
{
    public int score;
    public ScoreEntry(int score) { this.score = score; }
}

public static class LeaderboardManager
{
    public static int CurrentRunScore = 0;

    const string Key = "Scores";
    const int MaxEntries = 10;

    [System.Serializable]
    class Wrapper { public List<ScoreEntry> list; }

    public static void SaveCurrentScore()
    {
        var scores = LoadScores();
        scores.Add(new ScoreEntry(CurrentRunScore));
        scores.Sort((a, b) => b.score.CompareTo(a.score));
        if (scores.Count > MaxEntries)
            scores = scores.GetRange(0, MaxEntries);

        var w = new Wrapper { list = scores };
        PlayerPrefs.SetString(Key, JsonUtility.ToJson(w));
        PlayerPrefs.Save();
    }

    public static List<ScoreEntry> LoadScores()
    {
        string json = PlayerPrefs.GetString(Key, "");
        if (string.IsNullOrEmpty(json))
            return new List<ScoreEntry>();

        var w = JsonUtility.FromJson<Wrapper>(json);
        return w.list ?? new List<ScoreEntry>();
    }
}
