using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BelowUs
{
    public class Main : MonoBehaviour
    {
        private VisualTreeAsset visualTree;

        private void Start()
        {
            visualTree = Resources.Load<VisualTreeAsset>("UI/Options.uxml");
            VisualElement labelFromUXML = visualTree.CloneTree();

            VisualElement options = new VisualElement();
            VisualElement buttons = new VisualElement();

            for (int i = 0; i < labelFromUXML.childCount; i++)
            {
                VisualElement child = labelFromUXML.ElementAt(i);
                if (child.name == "Options")
                    options = child;
                else if (child.name == "Buttons")
                    buttons = child;
            }

            VisualElement resolution = new VisualElement();
            VisualElement graphics = new VisualElement();
            VisualElement volume = new VisualElement();
            for (int i = 0; i < options.childCount; i++)
            {
                VisualElement child = labelFromUXML.ElementAt(i);
                string name = child.name;
                if (name == "Resolution")
                    resolution = child;
                else if (name == "Graphics")
                    graphics = child;
                else if (name == "Volume")
                    volume = child;
            }

            //((DropdownField)resolution).
        }

        private void OnEnable()
        {
            //visualElement root = rootVisualElement;
        }


    }

}
