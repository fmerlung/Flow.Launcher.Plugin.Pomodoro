using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
    /// <param name="query"></param>
    /// <returns></returns>
    public List<IAppCommand> Parse(Query query)
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
            if (distance <= 1)
            {
                matchedCommands.Add((command, distance));
            }
        }

        List<IAppCommand> sortedCommands = matchedCommands.OrderBy(x => x.distance).ToList().ConvertAll(x => x.command);

        return sortedCommands;
    }
}
