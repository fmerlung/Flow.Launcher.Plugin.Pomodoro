using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Flow.Launcher.Plugin;
using Microsoft.Toolkit.Uwp.Notifications;

namespace Flow.Launcher.Plugin.Pomodoro
{
    /// <summary>
    /// The main class for the Pomodoro plugin, passing queries and returning results.
    /// </summary>
    public class Pomodoro : IPlugin
    {
        private PluginInitContext _context;
        private Engine _engine;
        private ResultFactory _resultFactory;

        /// <inheritdoc/>
        public void Init(PluginInitContext context)
        {
            _context = context;
            _engine = new Engine();
            _resultFactory = new ResultFactory(_engine);
        }

        /// <inheritdoc/>
        public List<Result> Query(Query query)
        {
            Result result = _resultFactory.Create(query);
            List<Result> output = new List<Result>();
            output.Add(result);
            return output;
        }
    }
}