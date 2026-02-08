using System;
using System.Collections.Generic;
using System.Threading;
using Flow.Launcher.Plugin;
using Microsoft.Toolkit.Uwp.Notifications;

#pragma warning disable 1591

namespace Flow.Launcher.Plugin.Pomodoro
{
    public class Pomodoro : IPlugin
    {
        private PluginInitContext _context;
        private int workDurationMins = 25 * 60 * 1000;
        private int breakDurationMins = 5 * 60 * 1000;
        private Timer timer;
        private bool isRunning = false;

        private DateTime timeSinceModeStart;

        private enum Mode
        {
            INIT,
            WORK,
            BREAK
        }

        private Mode currentMode;
        Dictionary<Mode, Mode> nextMode = new Dictionary<Mode, Mode>
        {
            { Mode.WORK, Mode.BREAK },
            { Mode.BREAK, Mode.WORK },
            { Mode.INIT, Mode.WORK }
        };

        public void Init(PluginInitContext context)
        {
            this.currentMode = Mode.INIT;
            this.timer = new Timer(TimerCallback);
            _context = context;
        }

        public List<Result> Query(Query query)
        {
            Result result = new Result();
            result.RoundedIcon = true;
            result.IcoPath = "Images/icon.png";

            if (!isRunning)
            {
                result.Title = $"No session";
            }
            else
            {
                result.Title = $"{this.currentMode}\t{GetTimeLeft()}";
            }

            if (query.Search.ToLower() == "start")
            {
                result.Title = $"start";
                result.SubTitle = $"Start session";
                result.Action = e =>
                {
                    this.isRunning = true;
                    StartTimer();
                    return true;
                };
            }

            if (query.Search.ToLower() == "stop")
            {
                this.isRunning = false;
                result.Title = $"stop";
                result.SubTitle = $"Stop session";
                result.Action = e =>
                {
                    this.isRunning = true;
                    this.timer.Change(Timeout.Infinite, Timeout.Infinite);
                    return true;
                };
            }

            if (query.Search.ToLower() == "skip")
            {
                result.Title = $"skip";
                result.SubTitle = $"skip to next work/break";
                result.Action = e =>
                {
                    StartTimer();
                    return true;
                };
            }

            List<Result> output = new List<Result>();
            output.Add(result);
            return output;
        }

        private Result GenerateResult(string title)
        {
            Result result = new Result();
            result.RoundedIcon = true;
            result.IcoPath = "Images/icon.png";
            result.Title = $"{title}";
            result.SubTitle = $"{title} session";

            return result;
        }

        private string GetTimeLeft()
        {
            if (this.currentMode == Mode.INIT) return "INIT";

            TimeSpan timeSpent = DateTime.Now - this.timeSinceModeStart;
            TimeSpan timeLeft = TimeSpan.FromMinutes(currentMode == Mode.WORK ? workDurationMins/60/1000 : breakDurationMins/60/1000) - timeSpent;

            return $"{timeLeft.Minutes}:{timeLeft.Seconds}";
        }

        private void StartTimer()
        {
            ToastContentBuilder toast = new ToastContentBuilder();

            this.timeSinceModeStart = DateTime.Now;
            if (!isRunning)
            {
                toast.AddText("Session stopped!");
                this.timer.Change(Timeout.Infinite, Timeout.Infinite);
                this.currentMode = Mode.INIT;
            }

            this.currentMode = nextMode[currentMode];
            if (currentMode == Mode.WORK) {
                timer.Change(workDurationMins, Timeout.Infinite);
                toast.AddText("Work start!");
            }
            else
            {
                timer.Change(breakDurationMins, Timeout.Infinite);
                toast.AddText("Time for a break!");
            }
            toast.Show();
        }
        private void TimerCallback(object state)
        {
            StartTimer();
        }
    }
}