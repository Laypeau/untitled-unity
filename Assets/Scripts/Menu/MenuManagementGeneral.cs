using System;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.SceneManagement;

namespace MenuManagement
{
    public class MenuManagementGeneral : MonoBehaviour
    {
        // Make sure this class extends monobehaviour, otherwise the main menu won't work because unity ui is dumb

        public static void GoToNextSceneInIndex()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        /// <summary>
        /// Quits the application
        /// </summary>
        public static void ApplicationQuit()
        {
            Application.Quit();
            Debug.Log("Game quit successfully");
        }

        /// <summary>
        /// Loads the scene at index 0
        /// </summary>
        public static void OpenMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(0);
        }
    }
}
