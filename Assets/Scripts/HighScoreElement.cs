using System;

[Serializable]
public class HighScoreElement
{
    public string playerName;
    public int[] playerHighScore;

    public HighScoreElement(string name, int[] highscores)
    {
        playerName = name;
        playerHighScore = highscores;
    }
}
