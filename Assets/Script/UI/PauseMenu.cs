using Mirror;
using UnityEngine;

namespace BelowUs
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenu;
        [TagSelector] [SerializeField] private string playerTag;

        public static bool IsOpen { get; private set; } = false;

        private MenuAction action;
        private NetworkManager manager = null;

        private void Awake()
        {
            action = new MenuAction();
            action.Menu.MenuButton.performed += _ => ShowOrHideMenu();
        }

        private void OnEnable() => action?.Enable();

        private void OnDisable() => action?.Disable();

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
            IsOpen = true;
            CheckIfPause();
        }

        private void HideMenu()
        {
            pauseMenu.SetActive(false);
            IsOpen = false;
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
            if (manager == null)
                manager = NetworkManager.singleton;

            Time.timeScale = manager.numPlayers == 1 && pauseMenu.activeSelf ? 0 : 1;
        }

        public void QuitGame() => Application.Quit();
    }
}
