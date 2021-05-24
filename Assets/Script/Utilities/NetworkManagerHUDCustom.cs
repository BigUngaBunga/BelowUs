using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BelowUs
{
    public class NetworkManagerHudCustom : MonoBehaviour
    {
        [Header("References")]
        private TMP_InputField ipText;
        private TextMeshProUGUI statusText;
        
        //Connection Buttons
        private GameObject connectBtnGameObj;
        private GameObject cancelBtnGameObj;
        private GameObject cancelConnBtnGameObj;
        private GameObject optionsBtnGameObj;

        //Options Buttons
        private GameObject optionsBackButtonObj;

        private NetworkManager manager = NetworkManager.singleton;

        private bool inConnectPnl = false;

        private GameObject mainPnl;
        private GameObject connectPnl;
        private GameObject optionsPnl;

        //options - resolutions
        private Resolution[] resolutions;
        [SerializeField] private Dropdown resolutionsDropdown;

        private readonly bool debug = true;

        private void Start()
        {
            manager = NetworkManager.singleton;

            if (manager == null)
                Debug.LogError("There is no networkmanager in the scene!");

            //Panels
            mainPnl = transform.Find("MainPnl").gameObject;
            connectPnl = transform.Find("ConnectPnl").gameObject;
            optionsPnl = transform.Find("OptionsPnl").gameObject;

            //Main Buttons Listeners
            mainPnl.transform.Find("HostBtn").GetComponent<Button>().onClick.AddListener(HostClicked);
            mainPnl.transform.Find("ConnectBtn").GetComponent<Button>().onClick.AddListener(SwitchPanelConnection);
            mainPnl.transform.Find("OptionsBtn").GetComponent<Button>().onClick.AddListener(SwitchPanelOptions);

            //Connection Misc
            ipText = connectPnl.transform.Find("IPField").GetComponent<TMP_InputField>();
            statusText = connectPnl.transform.Find("Status").GetComponent<TextMeshProUGUI>();

            //Connection Buttons
            Transform connectBtnTransform = connectPnl.transform.Find("Buttons");
            connectBtnGameObj = connectBtnTransform.Find("ConnectBtn").gameObject;
            cancelBtnGameObj = connectBtnTransform.Find("CancelBtn").gameObject;
            cancelConnBtnGameObj = connectBtnTransform.Find("CancelConnBtn").gameObject;

            connectBtnGameObj.GetComponent<Button>().onClick.AddListener(Connect);
            cancelBtnGameObj.GetComponent<Button>().onClick.AddListener(SwitchPanelConnection);
            cancelConnBtnGameObj.GetComponent<Button>().onClick.AddListener(CancelConnect);

            //Options Buttons
            Transform optionsBtnTransForm = optionsPnl.transform.Find("Buttons");
            optionsBackButtonObj = optionsBtnTransForm.Find("BackBtn").gameObject;

            optionsBackButtonObj.GetComponent<Button>().onClick.AddListener(SwitchPanelOptions);


            //Options Resolution
            resolutions = Screen.resolutions;
            resolutionsDropdown.ClearOptions();
            List<string> options = new List<string>();
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height + "@" + resolutions[i].refreshRate;
                options.Add(option);
            }

            resolutionsDropdown.AddOptions(options);
        }

        public void HostClicked() => manager.StartHost();

        private void Connect()
        {
            string txt = ipText.text;
            manager.StartClient();
            manager.networkAddress = txt;
            statusText.text = "Connecting to " + txt + "...";
            ChangeVisibleButtons();
            InvokeRepeating(nameof(CheckConnecting), 10, 0.5f);
        }

        private void ChangeVisibleButtons()
        {
            inConnectPnl = !inConnectPnl;

            connectBtnGameObj.SetActive(!inConnectPnl);
            cancelBtnGameObj.SetActive(!inConnectPnl);
            cancelConnBtnGameObj.SetActive(inConnectPnl);
        }

        private void CancelConnect()
        {
            manager.StopClient();
            statusText.text = "Waiting for input";
            ChangeVisibleButtons();
            CancelInvoke(nameof(CheckConnecting));
        }

        private void CheckConnecting()
        {
            if (!NetworkClient.active)
                CancelConnect();
        }

        private void SwitchPanelConnection()
        {
            mainPnl.gameObject.SetActive(!mainPnl.activeSelf);
            connectPnl.gameObject.SetActive(!connectPnl.activeSelf);
        }
        private void SwitchPanelOptions()
        {
            mainPnl.gameObject.SetActive(!mainPnl.activeSelf);
            optionsPnl.gameObject.SetActive(!optionsPnl.activeSelf);
        }

        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
    }
}
