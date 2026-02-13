namespace Flow.Launcher.Plugin.Pomodoro.src.Commands;

/// <summary>
/// Command to pause the current session.
/// </summary>
public class HelpCommand : IAppCommand
{
    /// <inheritdoc/>
    public string CommandString {get;} = "help";

    /// <inheritdoc/>
    public string DisplayTitle {get;}= "Help information";

    /// <inheritdoc/>
    public string DisplaySubtitle {get;} = "Show help information (will open browser)";

    /// <inheritdoc/>
    public void Execute(Engine engine)
    {
        engine.ShowHelp();
    }
}

