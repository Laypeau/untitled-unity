using UnityEngine;

public class MenuManagementPause : MonoBehaviour
{
    public GameObject PauseMenu;        //Remember to set in editor
    public GameObject SettingsSubmenu;  //Remember to set in editor
    public bool IsPaused = false;
    public KeyCode KeyCodePause = KeyCode.Escape;

    void Update()
    {
        if (Input.GetKeyDown(KeyCodePause))
        {
            if (IsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;

        SettingsSubmenu.SetActive(false);
        PauseMenu.SetActive(false);
    }

    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;

        PauseMenu.SetActive(true);
        SettingsSubmenu.SetActive(false);
    }
}
