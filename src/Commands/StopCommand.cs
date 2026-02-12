using System;

namespace Flow.Launcher.Plugin.Pomodoro.src.Commands;

/// <summary>
/// Command to stop the current Pomodoro session.
/// </summary>
public class StopCommand : IAppCommand
{
    /// <inheritdoc/>
    public string CommandString {get;} = "stop";

    /// <inheritdoc/>
    public string DisplayTitle {get;}= "Stop session";

    /// <inheritdoc/>
    public string DisplaySubtitle {get;} = "Stop the current session";

    /// <inheritdoc/>
    public void Execute(Engine engine)
    {
        engine.StopSession();
    }
}