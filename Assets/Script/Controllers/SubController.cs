using MyBox;
using UnityEngine;
using UnityEngine.UI;

namespace BelowUs
{
    public class SubController : StationController
    {
        [SerializeField] [MustBeAssigned] protected Button controlsButton;

        
        //protected override void SetButtonActiveStatus(bool activate)
        //{
        //    base.SetButtonActiveStatus(activate);

        //    controlsButton.gameObject.SetActive(activate);
        //}
    }
}
