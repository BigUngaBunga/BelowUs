using UnityEngine;

namespace BelowUs
{
    public class StationCollision : MonoBehaviour
    {
        private StationController controller;
        private GameObject buttonUI;

        void Start()
        {
            controller = GetComponentInParent<StationController>();
            buttonUI = GameObject.Find("Game/UI/EnterStationControl");
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("LocalPlayer"))
            {
                collision.gameObject.GetComponent<PlayerCharacterController>().Station = controller;
                InvokeRepeating(nameof(ShouldButtonBeDisplayed), 0, 0.1f);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("LocalPlayer"))
            {
                CancelInvoke(nameof(ShouldButtonBeDisplayed));
                buttonUI.SetActive(false);
            }
        }

        private void ShouldButtonBeDisplayed() => buttonUI.SetActive(controller.StationPlayerController == null);
    }
}
