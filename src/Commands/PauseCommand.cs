namespace Flow.Launcher.Plugin.Pomodoro.src.Commands;

/// <summary>
/// Command to pause the current session.
/// </summary>
public class PauseCommand : IAppCommand
{
    /// <inheritdoc/>
    public string CommandString {get;} = "pause";

    /// <inheritdoc/>
    public string DisplayTitle {get;}= "Pause session";

    /// <inheritdoc/>
    public string DisplaySubtitle {get;} = "Pause the current session";

    /// <inheritdoc/>
    public void Execute(Engine engine)
    {
        engine.PauseSession();
    }
}
