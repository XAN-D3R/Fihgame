using Godot;

namespace Fihgame.Scripts.Model;

public partial class RodDragData : GodotObject
{
    public RodItem Rod { get; }

    public RodDragData(RodItem rod)
    {
        Rod = rod;
    }
}