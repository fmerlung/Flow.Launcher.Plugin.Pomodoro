using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.XPath;
using Flow.Launcher.Plugin;
using Microsoft.Toolkit.Uwp.Notifications;

#pragma warning disable 1591

namespace Flow.Launcher.Plugin.Pomodoro
{
    public class Pomodoro : IPlugin
    {
        private PluginInitContext _context;
        private int totalQueries = 0;
        private Timer timer = new Timer(TimerCallback);

        public void Init(PluginInitContext context)
        {
            _context = context;
        }

        public List<Result> Query(Query query)
        {
            totalQueries++;


            Result result = new Result();
            result.Title = $"Query count: {totalQueries}";

            if (query.Search == "start")
            {
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

        private void StartTimer()
        {
            // Start the timer to execute every 25 minutes (1500 seconds)
            timer.Change(0, 5);
        }
        private static void TimerCallback(object state)
        {
            new ToastContentBuilder()
                .AddText("Pomodoro Timer")
                .Show();

            Console.WriteLine("Timer callback executed at: " + DateTime.Now);
        }

    }
}