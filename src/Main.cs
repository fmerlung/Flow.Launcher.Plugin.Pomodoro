using System.Collections.Generic;
using Flow.Launcher.Plugin;
using Flow.Launcher.Plugin.Pomodoro.src.Commands;

namespace Flow.Launcher.Plugin.Pomodoro
{
    /// <summary>
    /// The main class for the Pomodoro plugin, passing queries and returning results.
    /// </summary>
    public class Pomodoro : IPlugin
    {
        private PluginInitContext _context;
        private Engine _engine;
        private QueryParser _queryParser;
        private ResultFactory _resultFactory;

        private List<IAppCommand> _commands;

        /// <inheritdoc/>
        public void Init(PluginInitContext context)
        {
            _context = context;
            _commands = CommandLoader.LoadAll();
            _queryParser = new QueryParser(_commands);
            _engine = new Engine(_commands);
            _resultFactory = new ResultFactory(_engine, _commands);
        }

        /// <inheritdoc/>
        public List<Result> Query(Query query)
        {
            List<IAppCommand> matchedCommands = _queryParser.Parse(query);

            List<Result> results = new List<Result>();
            foreach (IAppCommand command in matchedCommands)
            {
                results.Add(_resultFactory.Create(command));
            }
            return results;
        }
    }
}