using System.Collections.Generic;

namespace Fihgame.Scripts.Model;

public class Inventory
{
    private Dictionary<string, int> _fishCount = new();
    private Dictionary<string, Fish> _fishData = new();

    public void AddFish(Fish fish)
    {
        if (_fishCount.ContainsKey(fish.Name))
        {
            _fishCount[fish.Name]++;
        }
        else
        {
            _fishCount[fish.Name] = 1;
            _fishData[fish.Name] = fish;
        }
    }

    public Dictionary<string, int> GetFishCount() => _fishCount;
    public Fish GetFishData(string name) => _fishData[name];
}