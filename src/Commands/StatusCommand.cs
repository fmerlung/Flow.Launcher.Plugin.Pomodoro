using System;

namespace Flow.Launcher.Plugin.Pomodoro.src.Commands;

/// <summary>
/// Command to sho wthe status of the current session.
/// </summary>
public class StatusCommand : IAppCommand
{
    /// <inheritdoc/>
    public string CommandString {get;} = "status";

    /// <inheritdoc/>
    public string DisplayTitle {get;}= "Show Status";

    /// <inheritdoc/>
    public string DisplaySubtitle {get;} = "Show session status";

    /// <inheritdoc/>
    public void Execute(Engine engine)
    {
        engine.DisplaySessionStatus();
    }
}
