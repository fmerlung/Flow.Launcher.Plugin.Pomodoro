using System;

namespace Flow.Launcher.Plugin.Pomodoro.src.Commands;

/// <summary>
/// Command to skip the current phase.
/// </summary>
public class SkipCommand : IAppCommand
{
    /// <inheritdoc/>
    public string CommandString {get;} = "skip";

    /// <inheritdoc/>
    public string DisplayTitle {get;}= "Skip current Phase";

    /// <inheritdoc/>
    public string DisplaySubtitle {get;} = "Skip the current Pomodoro phase";

    /// <inheritdoc/>
    public void Execute(Engine engine)
    {
        engine.SkipPhase();
    }
}
