using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [TagSelector] [SerializeField] private string playerTag;

    private MenuAction action;
    private PlayerInput playerInput;
    private NetworkManager manager;

    private void Awake()
    {
        action = new MenuAction();
        action.Menu.MenuButton.performed += _ => ShowOrHideMenu();
        if (SceneManager.GetActiveScene().buildIndex != 0)
            InvokeRepeating(nameof(FindPlayer), 0, 0.01f);
    }

    private void FindPlayer()
    {
        GameObject[] playerGameObjects = GameObject.FindGameObjectsWithTag(playerTag);
        GameObject playerGameObject = null;

        for (int i = 0; i < playerGameObjects.Length; i++) //Iterates through all players and checks if any of them are the local player
            if (playerGameObjects[i].GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                playerGameObject = playerGameObjects[i];
                break;
            }

        if (playerGameObject != null)
        {
            playerInput = playerGameObject.GetComponent<PlayerInput>();
            CancelInvoke(nameof(FindPlayer));
        }
    }

    private void Start()
    {
        manager = NetworkManager.singleton;
    }

    private void OnEnable()
    {
        action.Enable();
    }

    private void OnDisable()
    {
        action.Disable();
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
        playerInput.enabled = false;
        CheckIfPause();
    }

    private void HideMenu()
    {
        pauseMenu.SetActive(false);
        playerInput.enabled = true;
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
