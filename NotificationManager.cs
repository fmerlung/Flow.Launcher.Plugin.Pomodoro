using System.Threading;
using Microsoft.Toolkit.Uwp.Notifications;

namespace Flow.Launcher.Plugin.Pomodoro;

/// <summary>
/// Notification manager for displaying notifications to the user.
/// </summary>
public static class NotificationManager
{
    private static Timer clearTimer = new(ClearHistory, null, Timeout.Infinite, Timeout.Infinite);
    /// <summary>
    /// Shows a notification with a specified message and cleans it up when the timer elapses.
    /// </summary>
    /// <param name="message"></param>
    public static void Show(string message)
    {
        new ToastContentBuilder()
            .AddText("🍅 " + message + " 🍅")
            .Show();
        
            clearTimer.Change(2 * 1000 + 500, Timeout.Infinite);
    }

    private static void ClearHistory(object state)
    {
        ToastNotificationManagerCompat.History.Clear();
    }
}