using System;

namespace Flow.Launcher.Plugin.Pomodoro.src.Commands;

/// <summary>
/// Command to start a new Pomodoro session.
/// </summary>
public class StartCommand : IAppCommand
{
    /// <inheritdoc/>
    public string CommandString {get;} = "start";

    /// <inheritdoc/>
    public string DisplayTitle {get;}= "Start Session";

    /// <inheritdoc/>
    public string DisplaySubtitle {get;} = "Start a new Pomodoro session";

    /// <inheritdoc/>
    public void Execute(Engine engine)
    {
        engine.StartSession();
    }
}
