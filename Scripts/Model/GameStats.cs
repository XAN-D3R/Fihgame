namespace Fihgame.Scripts.Model;

public class GameStats
{
    public int FishCaught { get; private set; }
    public int CreaturesDefeated { get; private set; }
    public float TimeSurvived { get; private set; }
    public string LastKilledBy { get; private set; }

    public void AddFishCaught() => FishCaught++;
    public void AddCreatureDefeated() => CreaturesDefeated++;
    public void UpdateTime(float delta) => TimeSurvived += delta;
    public void SetKilledBy(string name) => LastKilledBy = name;

    public string GetFormattedTime()
    {
        int minutes = (int)(TimeSurvived / 60);
        int seconds = (int)(TimeSurvived % 60);
        return $"{minutes}:{seconds:D2}";
    }
}