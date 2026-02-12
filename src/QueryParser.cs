using System;
using System.Collections.Generic;
using System.Data;

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
        // Do fuzzy search using Levenshtein distance and set a threshold for the max distance allowed,
        //   since we don't want to return all commands all the time.
        // Check if command is allowed in current context, e.g. "start" should not be allowed in an unpaused session.
        List<IAppCommand> matchedCommands = new List<IAppCommand>();

        foreach (IAppCommand command in _commands)
        {
            if (query.Search.ToLower() == command.CommandString)
            {
                matchedCommands.Add(command);
            }
        }

        return matchedCommands;
    }
}
