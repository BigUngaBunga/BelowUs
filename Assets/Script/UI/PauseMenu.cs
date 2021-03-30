using Mirror;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;

    private NetworkManager manager;

    private void Start()
    {
        manager = NetworkManager.singleton;
    }

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
        CheckIfPause();
    }

    private void HideMenu()
    {
        pauseMenu.SetActive(false);
        CheckIfPause();
    }

    public void OpenOptions()
    {
        HideMenu();
        //TODO Open options
    }

    public void CheckIfPause()
    {
        //Only pause if you are the only player in the server and the pausemenu is open
        if (manager.numPlayers == 1 && pauseMenu.activeSelf)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
