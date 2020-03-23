using System;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.SceneManagement;

namespace MenuManagement
{
    public class MenuManagementGeneral : MonoBehaviour
    {
        // Make sure this class extends monobehaviour, otherwise the main menu won't work because unity ui is dumb / I'm bad

        public void GoToNextSceneInIndex()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void ApplicationQuit()
        {
            Application.Quit();
            Debug.Log("Game quit successfully");
        }

        /// <summary>
        /// Loads the scene called MainMenu
        /// </summary>
        public void OpenMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
    }
}
