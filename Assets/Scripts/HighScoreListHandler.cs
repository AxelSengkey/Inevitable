using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreListHandler : MonoBehaviour
{
    List<HighScoreElement> highScoreList = new List<HighScoreElement>();
    int maxCount = 10;
    private static string fileName = "highscores.json";

    public delegate void OnHighScoreListChanged(List<HighScoreElement> list);
    public static event OnHighScoreListChanged onHighScoreListChanged;

    // Start is called before the first frame update
    void Start()
    {
        LoadHighScores();
    }

    private void LoadHighScores()
    {
        highScoreList = FileHandler.ReadListFromJSON<HighScoreElement>(fileName);

        while (highScoreList.Count > maxCount)
        {
            highScoreList.RemoveAt(maxCount);
        }

        if (onHighScoreListChanged != null)
        {
            onHighScoreListChanged.Invoke(highScoreList);
        }
    }

    private void SaveHighScore()
    {
        FileHandler.SaveToJSON<HighScoreElement>(highScoreList, fileName);
    }

    public void AddHighScoreIfPossible(HighScoreElement element)
    {
        for (int i = 0; i < maxCount; i++)
        {
            if (highScoreList.Count <= i || element.playerHighScore[0] >= highScoreList[i].playerHighScore[0])
            {
                // Check if player name already exists in the high score list
                int existingIndex = -1;
                for (int j = 0; j < highScoreList.Count; j++)
                {
                    if (highScoreList[j].playerName == element.playerName)
                    {
                        existingIndex = j;
                        break;
                    }
                }

                // If player name already exists, insert the new high score and remove the existing one.
                // Otherwise, insert the new high score
                highScoreList.Insert(i, element); // Add new High Score
                if (existingIndex != -1)
                {
                    highScoreList.RemoveAt(existingIndex + 1); // Remove existing high score
                }

                // Trim the list if it exceeds maxCount
                while (highScoreList.Count > maxCount)
                {
                    highScoreList.RemoveAt(maxCount);
                }

                // Save the updated high score list
                SaveHighScore();

                // Invoke the high score list changed event
                if (onHighScoreListChanged != null)
                {
                    onHighScoreListChanged.Invoke(highScoreList);
                }

                break;
            }
        }
    }

    public static void ResetHighScoreList()
    {
        FileHandler.DeleteFile(fileName);
    }
}
