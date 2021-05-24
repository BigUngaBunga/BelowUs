using Mirror;
using MyBox;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BelowUs
{
    public class NetworkManagerHudCustom2 : MonoBehaviour
    {
        [Header("References")]
        private TMP_InputField ipText;
        private TextMeshProUGUI statusText;
        
        //Connection Buttons
        private GameObject connectBtnGameObj;
        private GameObject cancelBtnGameObj;
        private GameObject cancelConnBtnGameObj;

        private NetworkManager manager = NetworkManager.singleton;

        private bool inConnectPnl = false;

        [SerializeField] [ReadOnly] private GameObject mainPnl;
        private GameObject connectPnl;
        private GameObject optionsPnl;

        [SerializeField] private bool debug = true;

        private void Start()
        {
            manager = NetworkManager.singleton;

            if (manager == null)
                Debug.LogError("There is no networkmanager in the scene!");

            //Panels
            mainPnl = transform.Find("MainPnl").gameObject;
            connectPnl = transform.Find("ConnectPnl").gameObject;
            //optionsPnl = transform.Find("OptionsPnl").gameObject; //TODO uncomment this when it exists

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
            //optionsPnl.transform.Find("Buttons").Find("BackBtn").gameObject.GetComponent<Button>().onClick.AddListener(SwitchPanelOptions); //TODO uncomment this when it exists
        }

        public void HostClicked()
        {
            if (!NetworkServer.active && !NetworkClient.active)
                manager.StartHost();
        }

        private void Connect()
        {
            if (NetworkServer.active || NetworkClient.active)
                return;

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
            if (debug)
                Console.WriteLine(nameof(SwitchPanelConnection) + " was called!");

            mainPnl.gameObject.SetActive(!mainPnl.activeSelf);
            connectPnl.gameObject.SetActive(!connectPnl.activeSelf);
        }
        private void SwitchPanelOptions()
        {
            mainPnl.gameObject.SetActive(!mainPnl.activeSelf);
            optionsPnl.gameObject.SetActive(!optionsPnl.activeSelf);
        }
    }
}
