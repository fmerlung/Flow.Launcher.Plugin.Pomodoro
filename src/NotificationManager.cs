using System;
using System.Threading;
using Microsoft.Toolkit.Uwp.Notifications;
using static Flow.Launcher.Plugin.Pomodoro.Engine;

namespace Flow.Launcher.Plugin.Pomodoro;

/// <summary>
/// Notification manager for displaying notifications to the user.
/// </summary>
public static class NotificationManager
{
    private static Timer clearTimer = new(ClearHistory, null, Timeout.Infinite, Timeout.Infinite);
    /// <summary>
    /// Shows a notification with a specified message
    /// </summary>
    /// <param name="message"></param>
    public static void Show(string message)
    {
        new ToastContentBuilder()
            .AddHeader("pomodoro_notification", "🍅 Pomodoro", "")
            .AddText(message)
            .Show();
        
            clearTimer.Change(3 * 1000, Timeout.Infinite);
    }

    /// <summary>
    /// Shows a notification with the remaining time left.
    /// </summary>
    /// <param name="data">Data object with the session status information</param>
    public static void ShowStatus(StatusNotificationData data)
    {
        string timeElapsedString = $"{data.TimeElapsedInPhase.Minutes.ToString().PadLeft(2, '0')}:{data.TimeElapsedInPhase.Seconds.ToString().PadLeft(2, '0')}";
        string phaseDurationString = $"{(data.CurrentPhaseDurationSeconds / 60).ToString().PadLeft(2, '0')}:{(data.CurrentPhaseDurationSeconds % 60).ToString().PadLeft(2, '0')}";

        string statusString = data.IsPaused ? "PAUSED " : "";

        new ToastContentBuilder()
            .AddText("🍅 Pomodoro")
            .AddProgressBar(title: "Session status", 
                            value: data.TimeElapsedInPhase.TotalSeconds / data.CurrentPhaseDurationSeconds,
                            valueStringOverride: $"{timeElapsedString} / {phaseDurationString}",
                            status: statusString + (data.CurrentPhase == Phase.WORK ? "⚙ WORKING..." : "☕ ON BREAK..."))
            .Show();
        
            clearTimer.Change(5 * 1000, Timeout.Infinite);
    }

    private static void ClearHistory(object state)
    {
        ToastNotificationManagerCompat.History.Clear();
    }
}