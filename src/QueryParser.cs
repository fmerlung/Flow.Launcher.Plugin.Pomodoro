using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Flow.Launcher.Plugin.Pomodoro.src.Commands;
using static Flow.Launcher.Plugin.Pomodoro.Engine;

namespace Flow.Launcher.Plugin.Pomodoro;

/// <summary>
/// Parses the incoming query search strings.
/// </summary>
public class QueryParser
{
    private List<IAppCommand> _commands;
    /// <summary>
    /// Initializes a new instance of the <see cref="QueryParser"/> class.
    /// </summary>
    public QueryParser(List<IAppCommand> commands)
    {
        _commands = commands;
    }

    /// <summary>
    /// Parses the incoming query and returns a list of commands.
    /// </summary>
    /// <param name="query">Query to parse.</param>
    /// <param name="currentPhase">Current phase.</param>
    /// <param name="isPaused">Indicates if the session is paused.</param>
    /// <returns></returns>
    public List<IAppCommand> Parse(Query query, Phase currentPhase, bool isPaused)
    {
        // Check if command is allowed in current context, e.g. "start" should not be allowed in an unpaused session.
        if (query.Search.Length == 0)
        {
            return new List<IAppCommand>();
        }

        Fastenshtein.Levenshtein lev = new Fastenshtein.Levenshtein(query.Search.ToLower());

        List<(IAppCommand command, int distance)> matchedCommands = new List<(IAppCommand command, int distance)>();
        foreach (IAppCommand command in _commands)
        {
            int maxLength = Math.Min(query.Search.Length, command.CommandString.Length);
            int distance = lev.DistanceFrom(command.CommandString.Substring(0, maxLength).ToLower());
            if (distance <= 1 && IsCommandAllowed(command, currentPhase, isPaused))
            {
                matchedCommands.Add((command, distance));
            }
        }

        List<IAppCommand> sortedCommands = matchedCommands.OrderBy(x => x.distance).ToList().ConvertAll(x => x.command);

        return sortedCommands;
    }

    private bool IsCommandAllowed(IAppCommand command, Phase currentPhase, bool isPaused)
    {
        if (currentPhase == Phase.INIT)
        {
            return command is StartCommand;
        }

        if (isPaused)
        {
            return command is not PauseCommand;
        }
        else
        {
            return command is not StartCommand;
        }
    }
}
