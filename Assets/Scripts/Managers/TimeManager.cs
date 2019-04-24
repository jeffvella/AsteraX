using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager
{
    // Note: User interface animation should be set to unscaled time if it needs to work while paused.

    public bool IsPaused => _isPaused;
    private bool _isPaused;

    public void Pause()
    {
        Time.timeScale = 0;
        _isPaused = true;
    }

    public void Resume()
    {
        Time.timeScale = 1;
        _isPaused = false;
    }

    public void Toggle()
    {
        if (_isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }
}
