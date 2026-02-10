using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Toolkit.Uwp.Notifications;

namespace Flow.Launcher.Plugin.Pomodoro;

/// <summary>
/// The main engine for the Pomodoro timer, managing the timer and notifications.
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
    private Phase _currentPhase = Phase.INIT;
    private Timer _timer;
    private int _workDurationMins = 25;
    private int _breakDurationMins = 5;
    private Dictionary<Phase, int> _phaseDurations = new Dictionary<Phase, int>
    {
        { Phase.WORK, 25 },
        { Phase.BREAK, 5 }
    };
    private DateTime _phaseStartTime;
    private TimeSpan _elapsedTimeInPhase;
    private enum Phase
    {
        INIT,
        WORK,
        BREAK
    }

    private readonly Dictionary<Phase, Phase> _getNextPhase = new Dictionary<Phase, Phase>
    {
        { Phase.WORK, Phase.BREAK },
        { Phase.BREAK, Phase.WORK },
        { Phase.INIT, Phase.WORK }
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="Engine"/> class.
    /// </summary>
    public Engine()
    {
        _timer = new Timer(TimerCallback);
    }

    /// <summary>
    /// Gets the time left in the current phase.
    /// </summary>
    /// <returns>Minutes and seconds in formatted into a string</returns>
    public string GetTimeLeft()
    {
        if (_currentPhase == Phase.INIT) return "No session";

        TimeSpan timeLeft;
        if (IsPaused)
        {
            timeLeft = TimeSpan.FromMinutes(_phaseDurations[_currentPhase]) - _elapsedTimeInPhase;
        }
        else
        {
            timeLeft = TimeSpan.FromMinutes(_phaseDurations[_currentPhase]) - (DateTime.Now - _phaseStartTime + _elapsedTimeInPhase);
        }

        return $"{timeLeft.Minutes.ToString().PadLeft(2, '0')}:{timeLeft.Seconds.ToString().PadLeft(2, '0')}";
    }

    /// <summary>
    /// Returns the current phase.
    /// </summary>
    /// <returns>The string representation of the current phase</returns>
    public string GetCurrentPhase()
    {
        return _currentPhase.ToString();
    }

    private void SwitchTimerToNextPhase()
    {
        _currentPhase = _getNextPhase[_currentPhase];
        _phaseStartTime = DateTime.Now;
        _elapsedTimeInPhase = TimeSpan.Zero;

        if (_currentPhase == Phase.WORK)
        {
            StartTimer(_workDurationMins);
            NotificationManager.Show("Work start!");
        }
        else
        {
            StartTimer(_breakDurationMins);
            NotificationManager.Show("Break start!");
        }
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
            StartTimer(_workDurationMins);
            NotificationManager.Show("Session started!");
        }
        else if (IsPaused)
        {
            IsPaused = false;
            _phaseStartTime = DateTime.Now;
            StartTimer(_workDurationMins * 60 - (int)_elapsedTimeInPhase.TotalSeconds);
            NotificationManager.Show("Session resumed!");
        }
        else
        {
            NotificationManager.Show("Session already in progress!");
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

        NotificationManager.Show("Session stopped!");
    }

    /// <summary>
    /// Skips to the next phase of a session.
    /// </summary>
    public void SkipSession()
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

        // For some reason skipping starts next phase paused.

        if (_currentPhase == Phase.WORK)
        {
            StartTimer(_workDurationMins);
            NotificationManager.Show("Skipped to work!");
        }
        else
        {
            StartTimer(_breakDurationMins);
            NotificationManager.Show("Skipped to break!");
        }
    }

    /// <summary>
    /// Pauses the current session.
    /// </summary>
    public void PauseSession()
    {
        IsPaused = true;
        _elapsedTimeInPhase += DateTime.Now - _phaseStartTime;
        StopTimer();
        NotificationManager.Show("Session paused!");
    }

    private void TimerCallback(object state)
    {
        SwitchTimerToNextPhase();
    }

    private void StartTimer(int durationMins)
    {
        _timer.Change(durationMins * 60 * 1000, Timeout.Infinite);
    }

    private void StopTimer()
    {
        _timer.Change(Timeout.Infinite, Timeout.Infinite);
    }

}
