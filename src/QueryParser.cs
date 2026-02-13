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
            if (distance <= 2 && IsCommandAllowed(command, currentPhase, isPaused))
            {
                matchedCommands.Add((command, distance));
            }
        }

        // Something is wrong with the prefix matching, as sometimes the same result is locked in the first place despite spelling out another command.
        // Maybe use score instead?
        List<IAppCommand> sortedCommands = matchedCommands
        .OrderByDescending(x => x.command.CommandString.Substring(0, Math.Min(query.Search.Length, x.command.CommandString.Length)) == 
            query.FirstSearch.Substring(0, Math.Min(query.Search.Length, x.command.CommandString.Length)))
        .OrderBy(x => x.distance).ToList().ConvertAll(x => x.command);

        return sortedCommands;
    }

    private bool IsCommandAllowed(IAppCommand command, Phase currentPhase, bool isPaused)
    {
        if (command is HelpCommand)
        {
            return true;
        }

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
