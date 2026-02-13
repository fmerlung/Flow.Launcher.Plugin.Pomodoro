using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Flow.Launcher.Plugin.Pomodoro;

/// <summary>
/// The main engine for the Pomodoro timer.
/// </summary>
public class Engine
{
    /// <summary>
    /// Indicates whether a session is in progress.
    /// </summary>
    public bool IsRunning { get; private set; } = false;
    /// <summary>
    /// Indicates whether the current session is paused.
    /// </summary>
    public bool IsPaused { get; private set; } = false;
    /// <summary>
    /// Represents the different phases of the Pomodoro timer.
    /// </summary>
    public enum Phase
    {
        /// <summary>
        /// Initial phase before any work begins.
        /// </summary>
        INIT,
        /// <summary>
        /// The work aka "pomodoro' phase.
        /// </summary>
        WORK,
        /// <summary>
        /// The break phase.
        /// </summary>
        BREAK
    }
    private Phase _currentPhase = Phase.INIT;
    private Timer _timer;
    private Dictionary<Phase, int> _phaseDurationsSeconds = new Dictionary<Phase, int>
    {
        { Phase.WORK, 25 * 60 },
        { Phase.BREAK, 5 * 60}
    };
    private DateTime _phaseStartTime;
    private TimeSpan _elapsedTimeInPhase = TimeSpan.Zero;

    private readonly Dictionary<Phase, Phase> _getNextPhase = new Dictionary<Phase, Phase>
    {
        { Phase.WORK, Phase.BREAK },
        { Phase.BREAK, Phase.WORK },
        { Phase.INIT, Phase.WORK }
    };

    private List<IAppCommand> _commands;

    /// <summary>
    /// Initializes a new instance of the <see cref="Engine"/> class.
    /// </summary>
    /// <param name="commands">The list of available app commands</param>
    public Engine(List<IAppCommand> commands)
    {
        _commands = commands;
        _timer = new Timer(TimerCallback);
    }

    /// <summary>
    /// Gets the time left in the current phase.
    /// </summary>
    /// <returns>A TimeSpan representing the time left in the current phase</returns>
    public TimeSpan GetTimeLeftInPhase()
    {
        return TimeSpan.FromSeconds(_phaseDurationsSeconds[_currentPhase]) - GetTimeElapsedInCurrentPhase();
    }

    /// <summary>
    /// Gets the time elapsed in the current phase.
    /// </summary>
    /// <returns>A TimeSpan representing the time elapsed in the current phase</returns>
    public TimeSpan GetTimeElapsedInCurrentPhase()
    {
        if (IsPaused)
        {
            return _elapsedTimeInPhase;
        }
        else
        {
            return DateTime.Now - _phaseStartTime + _elapsedTimeInPhase;
        }
    }

    /// <summary>
    /// Returns the current phase.
    /// </summary>
    /// <returns>The string representation of the current phase</returns>
    public Phase GetCurrentPhase()
    {
        return _currentPhase;
    }

    private void SwitchTimerToNextPhase()
    {
        _currentPhase = _getNextPhase[_currentPhase];
        _phaseStartTime = DateTime.Now;
        _elapsedTimeInPhase = TimeSpan.Zero;

        StartTimer(_phaseDurationsSeconds[_currentPhase]);
        string phaseEmoji = _currentPhase == Phase.WORK ? "⚙" : "☕";
        string phaseName = _currentPhase.ToString();
        string phaseNameCapitalized = char.ToUpper(phaseName[0]) + phaseName.Substring(1).ToLower();

        NotificationManager.Show($"{phaseEmoji} {phaseNameCapitalized} start!");
    }

    /// <summary>
    /// Starts a new Pomodoro session.
    /// </summary>
    public void StartSession()
    {
        IsRunning = true;

        if (_currentPhase == Phase.INIT)
        {
            _currentPhase = Phase.WORK;
            _phaseStartTime = DateTime.Now;
            _elapsedTimeInPhase = TimeSpan.Zero;
            StartTimer(_phaseDurationsSeconds[_currentPhase]);
            NotificationManager.Show("⏱ Session started!");
        }
        else if (IsPaused)
        {
            IsPaused = false;
            _phaseStartTime = DateTime.Now;
            StartTimer(_phaseDurationsSeconds[_currentPhase] - (int)_elapsedTimeInPhase.TotalSeconds);
            NotificationManager.Show("⚙ Session resumed!");
        }
        else
        {
            NotificationManager.Show("⛔ Session already in progress!");
        }
    }

    /// <summary>
    /// Stops and terminates the current Pomodoro session.
    /// </summary>
    public void StopSession()
    {
        IsRunning = false;
        IsPaused = false;

        if (_currentPhase == Phase.INIT)
        {
            NotificationManager.Show("No session in progress!");
            return;
        }

        _currentPhase = Phase.INIT;
        StopTimer();

        NotificationManager.Show("⏱ Session stopped!");
    }

    /// <summary>
    /// Skips to the next phase of a session.
    /// </summary>
    public void SkipPhase()
    {
        if (_currentPhase == Phase.INIT)
        {
            NotificationManager.Show("No session in progress!");
            return;
        }

        _currentPhase = _getNextPhase[_currentPhase];
        _phaseStartTime = DateTime.Now;
        _elapsedTimeInPhase = TimeSpan.Zero;
        IsPaused = false;

        StartTimer(_phaseDurationsSeconds[_currentPhase] - (int)_elapsedTimeInPhase.TotalSeconds);
        NotificationManager.Show($"🏃‍♂️ Skipped to {_currentPhase.ToString().ToLower()}!");

    }

    /// <summary>
    /// Pauses the current session.
    /// </summary>
    public void PauseSession()
    {
        IsPaused = true;
        _elapsedTimeInPhase += DateTime.Now - _phaseStartTime;
        StopTimer();
        NotificationManager.Show("⏱ Session paused!");
    }

    /// <summary>
    /// Sets the duration for a specific phase.
    /// </summary>
    /// <param name="phase">Phase to set the duration for</param>
    /// <param name="minutes">Duration in minutes</param>
    public void SetPhaseDuration(Phase phase, int minutes)
    {
        if (_phaseDurationsSeconds.ContainsKey(phase))
        {
            _phaseDurationsSeconds[phase] = minutes * 60;
        }
    }

    /// <summary>
    /// Gets the status of the current session.
    /// </summary>
    
    public class StatusNotificationData
    {
        /// <summary>
        /// Current phase of the session (WORK, BREAK, or INIT)
        /// </summary>
        public Phase CurrentPhase { get; set; }
        /// <summary>
        /// Time elapsed in the current phase
        /// </summary>
        public TimeSpan TimeElapsedInPhase { get; set; }
        /// <summary>
        /// Total duration of the current phase in seconds
        /// </summary>
        public int CurrentPhaseDurationSeconds { get; set; }
        /// <summary>
        /// Indicates whether the session is currently paused
        /// </summary>
        public bool IsPaused { get; set; }
    }

    /// <summary>
    /// Displays the status of the current session in a notification.
    /// </summary>
    public void DisplaySessionStatus()
    {
        if (_currentPhase == Phase.INIT)
        {
            NotificationManager.Show("No session in progress!\nRun 'po start' to start a new session.");
            return;
        }

        StatusNotificationData data = new StatusNotificationData
        {
            CurrentPhase = _currentPhase,
            TimeElapsedInPhase = GetTimeElapsedInCurrentPhase(),
            CurrentPhaseDurationSeconds = _phaseDurationsSeconds[_currentPhase],
            IsPaused = IsPaused
        };

        NotificationManager.ShowStatus(data);
    }

    private void TimerCallback(object state)
    {
        SwitchTimerToNextPhase();
    }

    private void StartTimer(int durationSeconds)
    {
        _timer.Change(durationSeconds * 1000, Timeout.Infinite);
    }

    private void StopTimer()
    {
        _timer.Change(Timeout.Infinite, Timeout.Infinite);
    }

    /// <summary>
    /// Opens the help page for the plugin in the default web browser.
    /// </summary>
    public void ShowHelp()
    {
        ProcessStartInfo processStartInfo = new ProcessStartInfo
        {
            UseShellExecute = true,
            FileName = "https://github.com/fmerlung/Flow.Launcher.Plugin.Pomodoro/blob/main/Readme.md",
        };
        Process.Start(processStartInfo);
    }
}
