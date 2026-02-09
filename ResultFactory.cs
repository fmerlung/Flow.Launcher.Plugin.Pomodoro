using System;
using System.Threading;
using Flow.Launcher.Plugin;

namespace Flow.Launcher.Plugin.Pomodoro;

#pragma warning disable CS1591

public class ResultFactory
{
    private Timer timer;

    public ResultFactory(Timer timer)
    {
        this.timer = timer;
    }

    public static Result HandleStartArg()
    {
        Result result = new Result
        {
            Title = "Start Pomodoro Session",
            SubTitle = "Starts a new Pomodoro work session.",
            IcoPath = "Images/icon.png",
            Action = e =>
            {
                return true;
            }
        };
        return result;
    }
}
