namespace Flow.Launcher.Plugin.Pomodoro;

/// <summary>
/// Interface describing a command that can be executed within the application.
/// </summary>
public interface IAppCommand
{
    /// <summary>
    /// keyword triggering the command.
    /// </summary>
    string CommandString { get; }

    /// <summary>
    /// Title for displaying in the results list.
    /// </summary>
    string DisplayTitle { get; }
    /// <summary>
    /// Subtitle for displaying in the results list.
    /// </summary>
    string DisplaySubtitle { get; }

    /// <summary>
    /// Executes the command's action.
    /// </summary>
    void Execute(Engine engine);
}