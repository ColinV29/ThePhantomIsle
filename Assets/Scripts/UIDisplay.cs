using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text healthText;
    [SerializeField] TMP_Text gameVersionText;
    [SerializeField] TMP_Text unityVersionText;
    [SerializeField] TMP_Text pauseText;
    // Start is called before the first frame update
    void Start()
    {
        healthText.text = "Health: "; //TODO: Get health stat from player and display it here
        gameVersionText.text = "v" + Application.version;
        unityVersionText.text = "Unity " + Application.unityVersion;
    }

    // Update is called once per frame
    void Update()
    {
        displayPauseUI();
    }
    void displayPauseUI()
    // When the game is paused, display the Pause UI elements 
    {
        if (PauseControl.isPaused)
        {
            pauseText.gameObject.SetActive(true);
        }
        else if(!PauseControl.isPaused)
        {
            pauseText.gameObject.SetActive(false);
        }
    }
}
