using Mirror;
using UnityEngine;

namespace BelowUs
{
    public class PlayerColor : NetworkBehaviour
    {
        private SpriteRenderer spriteRenderer;
        [SyncVar(hook = nameof(SetColor))]
        private Color color;

        public void Awake() => spriteRenderer = GetComponent<SpriteRenderer>();

        public override void OnStartClient()
        {
            base.OnStartClient();
            RandomizeColor();
        }

        #pragma warning disable S1172 // Unused method parameters should be removed
        #pragma warning disable IDE0060 // Remove unused parameter
        private void SetColor(Color oldColor, Color newColor) => spriteRenderer.color = newColor;

        private void RandomizeColor() => color = new Color(Random.value, Random.value, Random.value);
    }
}