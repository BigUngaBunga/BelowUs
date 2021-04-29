using Mirror;
using UnityEngine;

namespace BelowUs
{
    public class PlayerColor : NetworkBehaviour
    {
        private SpriteRenderer spriteRenderer;
        [SyncVar(hook = nameof(SetColor))]
        private Color color;

        public void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            RandomizeColor();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Following mirror syntax")]
        private void SetColor(Color oldColor, Color newColor)
        {
            spriteRenderer.color = newColor;
        }

        private void RandomizeColor()
        {
            color = new Color(Random.value, Random.value, Random.value);
        }
    }
}