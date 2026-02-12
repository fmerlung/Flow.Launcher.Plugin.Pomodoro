using System.Collections.Generic;

namespace Flow.Launcher.Plugin.Pomodoro;

/// <summary>
/// Factory class for creating result objects.
/// </summary>
public class ResultFactory
{
    private Engine _engine;
    private List<IAppCommand> _commands;

    /// <summary>
    /// A factory which produces result objects depending on the incoming query
    /// </summary>
    /// <param name="engine">The Pomodoro engine instance</param>
    /// <param name="commands">The list of available app commands</param>
    public ResultFactory(Engine engine, List<IAppCommand> commands)
    {
        _engine = engine;
        _commands = commands;
    }


    /// <summary>
    /// Creates a result object based on the incoming command.
    /// </summary>
    /// <param name="matchedCommand">The command to create a result for</param>
    /// <returns></returns>
    public Result Create(IAppCommand matchedCommand)
    {
        Result result = new Result();
        result.IcoPath = "Images/icon.png";
        result.Title = matchedCommand.DisplayTitle;
        result.SubTitle = matchedCommand.DisplaySubtitle;
        result.Action = e =>
        {
            matchedCommand.Execute(_engine);
            return true;
        };

        return result;
    }
}
