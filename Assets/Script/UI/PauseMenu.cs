using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;

    public void ShowOrHideMenu()
    {
        //TODO disable if options menu (and more) is open
        //so that pause menu is not opened when esc is pressed with the purpose of closing options
        if (pauseMenu.activeSelf)
            HideMenu();
        else
            ShowMenu();
    }

    private void ShowMenu()
    {
        pauseMenu.SetActive(true);
    }

    private void HideMenu()
    {
        pauseMenu.SetActive(false);
    }

    public void OpenOptions()
    {
        HideMenu();
        //TODO Open options
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
