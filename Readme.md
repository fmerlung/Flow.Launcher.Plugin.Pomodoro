Flow.Launcher.Plugin.Pomodoro
==================

A plugin for the Flow Launcher, based on the [pomodoro time management technique](https://en.wikipedia.org/wiki/Pomodoro_Technique).


### Usage

When the user runs 'po start' a new session is created. A session is understood as a sequence of 'work' and 'break' phases, alternating between the two indefinitely.

The default duration a work phase is 25 minutes, while a break phase is 5 minutes.

### Commands

    po start
    - starts a new session or resumes a paused session.

    po status
    - displays the status of the current session.

    po pause
    - pausing the current session.

    po stop 
    - stops the current session, resetting the plugin.

    po skip
    - skips to the next part of the session, whether it be a pomodoro or a break.