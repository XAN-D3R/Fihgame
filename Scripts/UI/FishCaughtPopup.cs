using Fihgame.Scripts.Model;
using Godot;

namespace Fihgame.Scripts.UI;

public partial class FishCaughtPopup : Panel
{
    private Label _fishName;
    private Label _rarityLabel;
    private TextureRect _fishSprite;
    
    [Export] public float DisplayTime = 3f;
    private float _displayTimer = 0f;

    public override void _Ready()
    {
        _fishName = GetNode<Label>("FishName");
        _rarityLabel = GetNode<Label>("RarityLabel");
        _fishSprite = GetNode<TextureRect>("FishSprite");
    }

    public override void _Process(double delta)
    {
        if (Visible)
        {
            _displayTimer -= (float)delta;
            if (_displayTimer <= 0f)
            {
                Visible = false;
            }
        }
    }

    public void ShowFish(Fish fish)
    {
        _fishName.Text = fish.Name;
        _rarityLabel.Text = fish.GetRarityText();
        _rarityLabel.AddThemeColorOverride("font_color", fish.GetRarityColor());
        _fishSprite.Texture = GD.Load<Texture2D>(fish.SpritePath);
        _displayTimer = DisplayTime;
        Visible = true;
    }
}