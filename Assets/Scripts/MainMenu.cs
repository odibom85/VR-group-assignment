using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI startButtonText;
    private static bool hasStartedGame = false;

    void Start()
    {
        if (hasStartedGame && startButtonText != null)
        {
            startButtonText.text = "Resume Game";
            startButtonText.fontSize = 75;
        }
    }

    public void StartGame()
    {
        hasStartedGame = true;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}

