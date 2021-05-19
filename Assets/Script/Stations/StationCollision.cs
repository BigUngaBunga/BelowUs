using Mirror;
using System;
using UnityEngine;

namespace BelowUs
{
    public class StationCollision : MonoBehaviour
    {
        private NetworkBehaviour controller;
        private GameObject buttonUI;
        private Type t;

        void Start()
        {
            controller = GetComponentInParent<StationController>();

            if (controller == null)
                controller = GetComponentInParent<GeneratorController>();

            t = controller.GetType();

            buttonUI = GameObject.Find("/Game/UI/EnterStationControl");
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("LocalPlayer"))
                InvokeRepeating(nameof(ShouldButtonBeDisplayed), 0, 0.1f);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("LocalPlayer"))
            {
                CancelInvoke(nameof(ShouldButtonBeDisplayed));
                buttonUI.SetActive(false);
            }
        }

        private void ShouldButtonBeDisplayed()
        {
            if (buttonUI == null)
                return;

            if (t == typeof(StationController))
                buttonUI.SetActive(!((StationController)controller).IsOccupied);
            else
                buttonUI.SetActive(!((GeneratorController)controller).IsOccupied);
        }
    }
}
