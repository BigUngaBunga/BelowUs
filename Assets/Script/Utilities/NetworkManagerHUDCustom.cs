using Mirror;
using TMPro;
using UnityEngine;

namespace BelowUs
{
    public class NetworkManagerHudCustom : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI ipText;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private GameObject connectBtn;
        [SerializeField] private GameObject cancelBtn;
        [SerializeField] private GameObject cancelConnBtn;

        private NetworkManager manager = NetworkManager.singleton;

        private void Start() => manager = NetworkManager.singleton;

        public void HostClicked() => manager.StartHost();

        public void Connect()
        {
            string txt = ipText.text.Substring(0, ipText.text.Length - 1);
            manager.StartClient();
            manager.networkAddress = txt;
            statusText.text = "Connecting to " + txt + "...";
            ChangeVisibleButtons(true);
            InvokeRepeating(nameof(CheckConnecting), 10, 0.5f);
        }

        public void ChangeVisibleButtons(bool enableCancelCon)
        {
            connectBtn.SetActive(!enableCancelCon);
            cancelBtn.SetActive(!enableCancelCon);
            cancelConnBtn.SetActive(enableCancelCon);
        }

        public void CancelConnect()
        {
            manager.StopClient();
            statusText.text = "Waiting for input";
            ChangeVisibleButtons(false);
            CancelInvoke(nameof(CheckConnecting));
        }

        private void CheckConnecting()
        {
            if (!NetworkClient.active)
                CancelConnect();
        }
    }
}
