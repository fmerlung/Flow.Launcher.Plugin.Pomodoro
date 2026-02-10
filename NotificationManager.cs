using System;
using Microsoft.Toolkit.Uwp.Notifications;

namespace Flow.Launcher.Plugin.Pomodoro;

/// <summary>
/// Notification manager for displaying notifications to the user.
/// </summary>
public static class NotificationManager
{
    /// <summary>
    /// Displays a notification with the given message.
    /// </summary>
    /// <param name="message">The message body of the notification.</param>
    public static void Show(string message)
    {
        new ToastContentBuilder()
            .AddText(message)
            .Show();
    }
}