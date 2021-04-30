using UnityEngine;
using TMPro;

namespace BelowUs
{
    public class GoldDisplay : MonoBehaviour
    {
        [SerializeReference] private Transform gold;
        private TMP_Text depthDisplay;
        void Start()
        {
            depthDisplay = GetComponentInParent<TMP_Text>();
            InvokeRepeating(nameof(UpdateDisplay), 0.1f, 0.1f);
        }
        private void UpdateDisplay() => depthDisplay.text = ((int)gold.position.y).ToString();
    }
}

