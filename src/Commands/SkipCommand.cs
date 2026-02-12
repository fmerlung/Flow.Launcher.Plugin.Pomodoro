namespace Flow.Launcher.Plugin.Pomodoro.src.Commands;

/// <summary>
/// Command to skip the current phase.
/// </summary>
public class SkipCommand : IAppCommand
{
    /// <inheritdoc/>
    public string CommandString {get;} = "skip";

    /// <inheritdoc/>
    public string DisplayTitle {get;}= "Skip phase";

    /// <inheritdoc/>
    public string DisplaySubtitle {get;} = "Skip to next phase in session";

    /// <inheritdoc/>
    public void Execute(Engine engine)
    {
        engine.SkipPhase();
    }
}
