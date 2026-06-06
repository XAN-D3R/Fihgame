using System;
using System.Collections.Generic;
using Godot;

namespace Fihgame.Scripts.Model;

public class Inventory
{
    private Dictionary<string, int> _count = new();
    private Dictionary<string, Item> _data = new();

    public event Action OnChanged;

    public void AddItem(Item item)
    {
        if (_count.ContainsKey(item.Name))
            _count[item.Name] += item.Quantity;
        else
        {
            _count[item.Name] = item.Quantity;
            _data[item.Name] = item;
        }

        GD.Print(_count[item.Name]);
        OnChanged?.Invoke();
    }

    public Dictionary<string, int> GetCount() => _count;
    public Item GetData(string name) => _data[name];
}