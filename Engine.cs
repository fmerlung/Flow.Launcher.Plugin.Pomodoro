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
    private Phase _currentPhase = Phase.INIT;
    private Timer _timer;
    private int _workDurationMins = 25;
    private int _breakDurationMins = 5;
    private DateTime _timeSincePhaseStart;

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
        if (_currentPhase == Phase.INIT) return "ERROR: No session";

        TimeSpan timeSpent = DateTime.Now - _timeSincePhaseStart;
        TimeSpan timeLeft = TimeSpan.FromMinutes(_currentPhase == Phase.WORK ?
        _workDurationMins :
         _breakDurationMins) - timeSpent;

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
        ToastContentBuilder toast = new ToastContentBuilder();

        _currentPhase = _getNextPhase[_currentPhase];
        _timeSincePhaseStart = DateTime.Now;

        if (_currentPhase == Phase.WORK)
        {
            StartTimer(_workDurationMins);
            toast.AddText("Work start!");
        }
        else
        {
            StartTimer(_breakDurationMins);
            toast.AddText("Time for a break!");
        }
        toast.Show();
    }

    /// <summary>
    /// Starts a new Pomodoro session.
    /// </summary>
    public void StartSession()
    {
        IsRunning = true;

        ToastContentBuilder toast = new ToastContentBuilder();
        if (_currentPhase == Phase.INIT)
        {
            _currentPhase = Phase.WORK;
            _timeSincePhaseStart = DateTime.Now;
            StartTimer(_workDurationMins);
            toast.AddText("Session started!");
        }
        else
        {
            toast.AddText("Session already in progress!");
        }
        toast.Show();
    }

    /// <summary>
    /// Stops and terminates the current Pomodoro session.
    /// </summary>
    public void StopSession()
    {
        IsRunning = false;

        ToastContentBuilder toast = new ToastContentBuilder();
        if (_currentPhase == Phase.INIT)
        {
            toast.AddText("No session in progress!");
            toast.Show();
            return;
        }

        _currentPhase = Phase.INIT;
        StopTimer();

        toast.AddText("Session stopped!");
        toast.Show();
    }

    /// <summary>
    /// Skips to the next phase of a session.
    /// </summary>
    public void SkipSession()
    {
        ToastContentBuilder toast = new ToastContentBuilder();
        _currentPhase = _getNextPhase[_currentPhase];
        _timeSincePhaseStart = DateTime.Now;

        if (_currentPhase == Phase.WORK)
        {
            StartTimer(_workDurationMins);
            toast.AddText("Skipped to work!");
        }
        else
        {
            StartTimer(_breakDurationMins);
            toast.AddText("Skipped to break!");
        }
        toast.Show();
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
