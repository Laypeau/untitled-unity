using System;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.SceneManagement;

public class MenuManagementGeneral : MonoBehaviour
{
    public void GoToNextSceneInIndex()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ApplicationQuit()
    {
        Application.Quit();
        Debug.Log("Pretend the game quit");
    }

    public void OpenMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void ForceCrash()
    {
        //DO NOT CRASH THE EDITOR
        Utils.ForceCrash(ForcedCrashCategory.FatalError);
    }
}
