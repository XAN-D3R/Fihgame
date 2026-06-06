using Fihgame.Scripts.Model;
using Godot;

namespace Fihgame.Scripts.UI;

public partial class DeathMenu : Panel
{
    private Label _titleLabel;
    private Label _killedByLabel;
    private Label _fishCaughtLabel;
    private Label _creaturesDefeatedLabel;
    private Label _timeLabel;
    private Button _retryButton;
    private Button _quitButton;

    public override void _Ready()
    {
        _titleLabel = GetNode<Label>("TitleLabel");
        _killedByLabel = GetNode<Label>("KilledByLabel");
        _fishCaughtLabel = GetNode<Label>("StatsContainer/FishCaughtLabel");
        _creaturesDefeatedLabel = GetNode<Label>("StatsContainer/CreaturesDefeatedLabel");
        _timeLabel = GetNode<Label>("StatsContainer/TimeLabel");
        _retryButton = GetNode<Button>("RetryButton");
        _quitButton = GetNode<Button>("QuitButton");

        _retryButton.Pressed += OnRetryPressed;
        _quitButton.Pressed += OnQuitPressed;
    }

    public void ShowDeathMenu(GameStats stats)
    {
        _titleLabel.Text = "You Died";
        _killedByLabel.Text = $"Defeated by {stats.LastKilledBy}";
        _fishCaughtLabel.Text = $"Fish caught\n{stats.FishCaught}";
        _creaturesDefeatedLabel.Text = $"Creatures defeated\n{stats.CreaturesDefeated}";
        _timeLabel.Text = $"Time survived\n{stats.GetFormattedTime()}";

        Visible = true;
        GetTree().Paused = true;
    }

    private void OnRetryPressed()
    {
        GetTree().Paused = false;
        GetTree().ReloadCurrentScene();
    }

    private void OnQuitPressed()
    {
        GetTree().Paused = false;
        GetTree().Quit();
    }
}