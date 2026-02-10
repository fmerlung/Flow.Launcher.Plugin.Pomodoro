using System;
using Flow.Launcher.Plugin;

namespace Flow.Launcher.Plugin.Pomodoro;

/// <summary>
/// Factory class for creating result objects.
/// </summary>
public class ResultFactory
{
    private Engine _engine;

    /// <summary>
    /// A factory which produces result objects depending on the incoming query
    /// </summary>
    /// <param name="engine">The Pomodoro engine instance</param>
    public ResultFactory(Engine engine)
    {
        _engine = engine;
    }

    /// <summary>
    /// Creates a result object based on the incoming query.
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public Result Create(Query query)
    {
        Result result = new Result();
        result.IcoPath = "Images/icon.png";

        if (!_engine.IsRunning)
        {
            result.Title = "No session";
        }
        else
        {
            result.Title = $"{_engine.GetCurrentPhase()} {_engine.GetTimeLeft()}";
        }

        if (query.Search.ToLower() == "start")
        {
            result.Title = $"Start";
            result.SubTitle = $"Start session";
            result.Action = e =>
            {
                _engine.StartSession();
                return true;
            };
        }

        if (query.Search.ToLower() == "stop")
        {
            result.Title = $"Stop";
            result.SubTitle = $"Stop session";
            result.Action = e =>
            {
                _engine.StopSession();
                return true;
            };
        }

        if (query.Search.ToLower() == "skip")
        {
            result.Title = $"Skip";
            result.SubTitle = $"Skip to next pomodoro or break";
            result.Action = e =>
            {
                _engine.SkipSession();
                return true;
            };
        }
        return result;
    }
}
