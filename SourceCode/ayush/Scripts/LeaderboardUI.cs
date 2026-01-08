using System.Text;
using UnityEngine;
using TMPro;

public class LeaderboardUI : MonoBehaviour
{
    public TMP_Text leaderboardText;

    void Start()
    {
        var scores = LeaderboardManager.LoadScores();
        StringBuilder sb = new StringBuilder();

        if (scores.Count == 0)
        {
            sb.AppendLine("No scores yet. Play a game!");
        }
        else
        {
            int rank = 1;
            foreach (var s in scores)
            {
                sb.AppendLine($"{rank}. {s.score}");
                rank++;
            }
        }

        leaderboardText.text = sb.ToString();
    }
}
