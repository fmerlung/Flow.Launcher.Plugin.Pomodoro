using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Flow.Launcher.Plugin.Pomodoro.src.Commands;

/// <summary>
/// Utility class to load in all available command classes.
/// </summary>
public static class CommandLoader
{
    /// <summary>
    /// Loads all classes that implement IAppCommand and returns them as a list.
    /// </summary>
    /// <returns></returns>
    public static List<IAppCommand> LoadAll()
    {
        Assembly currentAssembly = Assembly.GetExecutingAssembly();

        var commandTypes = currentAssembly.GetTypes()
            .Where(t => 
                t.IsClass && 
                !t.IsAbstract && 
                typeof(IAppCommand).IsAssignableFrom(t)
            );

        List<IAppCommand> commands = new List<IAppCommand>();

        foreach (Type type in commandTypes)
        {
            IAppCommand instance = (IAppCommand)Activator.CreateInstance(type);
            commands.Add(instance);
        }

        return commands;
    }
}
