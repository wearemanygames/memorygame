using System;

[System.Serializable]
public class LevelResult { 
    public int score;
    public String name;

    public LevelResult(int score, string name)
    {
        this.score = score;
        this.name = name;
    }
}
