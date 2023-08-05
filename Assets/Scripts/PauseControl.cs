using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseControl : MonoBehaviour
{
    public static bool isPaused;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }
    public void TogglePause()
    {
        if (Time.timeScale > 0)
        {
            isPaused = true;
            Time.timeScale = 0;
        }
        else if (Time.timeScale < Mathf.Epsilon)
        {
            isPaused = false;
            Time.timeScale = 1;
        }
    }
}
