using Mirror;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace BelowUs
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject optionsPnl;
        [SerializeField] private GameObject pausePnl;
        [SerializeField] private Dropdown resolutionsDropdown;

        public static bool IsOpen { get; private set; } = false;
        public static bool IsEnabled { get; set; } = true;

        private MenuAction action;
        private NetworkManager manager = null;

        private readonly bool debug = true;

        private void Awake()
        {
            action = new MenuAction();
            action.Menu.MenuButton.performed += _ => ShowOrHideMenu();

            //Resolutions
            List<string> resolutions;

            resolutionsDropdown.ClearOptions();
            resolutions = new List<string>();
            for (int i = 0; i < Screen.resolutions.Length; i+=3)
                resolutions.Add(Screen.resolutions[i].width + " x " + Screen.resolutions[i].height + "@" + Screen.resolutions[i].refreshRate);

            resolutionsDropdown.AddOptions(resolutions);
        }

        private void OnEnable() => action?.Enable();

        private void OnDisable() => action?.Disable();

        public void ShowOrHideMenu()
        {
            //TODO disable if options menu (and more) is open
            //so that pause menu is not opened when esc is pressed with the purpose of closing options
            if (!IsEnabled)
            {
                if (debug)
                    Debug.Log("Menu " + nameof(IsEnabled) + " is " + IsEnabled);
                return;
            }
                

            if (pauseMenu.activeSelf)
                HideMenu();
            else
                ShowMenu();
        }

        private void ShowMenu()
        {
            if (debug)
                Debug.Log(nameof(transform) + " is being shown!");

            pauseMenu.SetActive(true);
            optionsPnl.SetActive(false);
            IsOpen = true;
            CheckIfPause();
        }

        private void HideMenu()
        {
            if (debug)
                Debug.Log(nameof(transform) + " is being hidden!");

            pauseMenu.SetActive(false);
            IsOpen = false;
            CheckIfPause();
        }

        public void OpenOptions()
        {
            pausePnl.SetActive(false);
            optionsPnl.SetActive(true);
        }

        public void BackOptions()
        {
            pausePnl.SetActive(true);
            optionsPnl.SetActive(false);
        }

        public void CheckIfPause()
        {
            //Only pause if you are the only player in the server and the pausemenu is open
            if (manager == null)
                manager = NetworkManager.singleton;

            if (manager != null && pauseMenu != null)
                Time.timeScale = manager.numPlayers == 1 && pauseMenu.activeSelf ? 0 : 1;
        }

        public void QuitGame() => Application.Quit();

        public void SetFullscreen(bool isFullscreen) => Screen.fullScreen = isFullscreen;

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = Screen.resolutions[resolutionIndex * 3];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
    }
}
