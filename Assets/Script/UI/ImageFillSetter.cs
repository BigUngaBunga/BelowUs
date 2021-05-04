using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Sets an Image component's fill amount to represent how far Variable is
/// between Min and Max.
/// </summary>
namespace BelowUs
{
    public class ImageFillSetter : MonoBehaviour
    {
        [Tooltip("Value to use as the current ")]
        [SerializeField] private FloatReference variable;

        [Tooltip("Min value that Variable to have no fill on Image.")]
        [SerializeField] private FloatReference min;

        [Tooltip("Max value that Variable can be to fill Image.")]
        [SerializeField] private FloatReference max;

        [Tooltip("Image to set the fill amount on.")]
        [SerializeField] private Image image = null;

        private void Start()
        {
            if (image != null)
                UpdateImageFill();
            else
                Debug.Log(transform.name + " has an unassigned image!");
        }

        public float GetFillAmount(FloatReference min, FloatReference max, FloatReference var) => Mathf.Clamp01(Mathf.InverseLerp(min, max, var));

        private void UpdateImageFill() => image.fillAmount = GetFillAmount(min, max, variable);
    }
}
