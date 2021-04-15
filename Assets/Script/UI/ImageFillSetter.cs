using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Sets an Image component's fill amount to represent how far Variable is
/// between Min and Max.
/// </summary>
public class ImageFillSetter : MonoBehaviour
{
    [Tooltip("Value to use as the current ")]
    [SerializeField] private FloatReference Variable;

    [Tooltip("Min value that Variable to have no fill on Image.")]
    [SerializeField] private FloatReference Min;

    [Tooltip("Max value that Variable can be to fill Image.")]
    [SerializeField] private FloatReference Max;

    [Tooltip("Image to set the fill amount on.")]
    [SerializeField] private Image Image = null;

    private void Start()
    {
        if (Image != null)
            UpdateImageFill();
        else
            Debug.Log(transform.name + " has an unassigned image!");
    }

    public float GetFillAmount(FloatReference min, FloatReference max, FloatReference var)
    {
        return Mathf.Clamp01(Mathf.InverseLerp(min, max, var));
    }

    private void UpdateImageFill()
    {
        Image.fillAmount = GetFillAmount(Min, Max, Variable);
    }
}