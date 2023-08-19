using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI gameVersionText;
    [SerializeField] TextMeshProUGUI unityVersionText;
    [SerializeField] TextMeshProUGUI pauseText;
    
    // Start is called before the first frame update
    void Start()
    {
        healthText.text = "Health: " + GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMovement>().getPlayerHP(); //TODO: Get health stat from player and display it here
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
