using Mirror;
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
        
        private GameObject connectBtnGameObj;
        private GameObject cancelBtnGameObj;
        private GameObject cancelConnBtnGameObj;

        private NetworkManager manager = NetworkManager.singleton;

        private bool inConnectPnl = false;

        private GameObject mainPnl;
        private GameObject connectPnl;

        private readonly bool debug = true;

        private void Start()
        {
            manager = NetworkManager.singleton;

            if (manager == null)
                Debug.LogError("There is no networkmanager in the scene!");

            mainPnl = transform.Find("MainPnl").gameObject;
            connectPnl = transform.Find("ConnectPnl").gameObject;

            mainPnl.transform.Find("HostBtn").GetComponent<Button>().onClick.AddListener(HostClicked);
            mainPnl.transform.Find("ConnectBtn").GetComponent<Button>().onClick.AddListener(SwitchPanel);

            ipText = connectPnl.transform.Find("IPField").GetComponent<TMP_InputField>();
            statusText = connectPnl.transform.Find("Status").GetComponent<TextMeshProUGUI>();

            Transform btnTransform = connectPnl.transform.Find("Buttons");
            connectBtnGameObj = btnTransform.Find("ConnectBtn").gameObject;
            cancelBtnGameObj = btnTransform.Find("CancelBtn").gameObject;
            cancelConnBtnGameObj = btnTransform.Find("CancelConnBtn").gameObject;

            connectBtnGameObj.GetComponent<Button>().onClick.AddListener(Connect);
            cancelBtnGameObj.GetComponent<Button>().onClick.AddListener(SwitchPanel);
            cancelConnBtnGameObj.GetComponent<Button>().onClick.AddListener(CancelConnect);
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

        private void SwitchPanel()
        {
            mainPnl.gameObject.SetActive(!mainPnl.activeSelf);
            connectPnl.gameObject.SetActive(!connectPnl.activeSelf);
        }
    }
}
