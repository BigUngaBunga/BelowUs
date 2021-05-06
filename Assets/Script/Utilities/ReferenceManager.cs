using UnityEngine;

namespace BelowUs
{
    public class ReferenceManager : MonoBehaviour
    {
        /// <summary>The one and only NetworkManager</summary>
        public static ReferenceManager Singleton { get; private set; }

        [TagSelector] [SerializeField] private string localPlayerTag;
        [TagSelector] [SerializeField] private string playerTag;
        [SerializeField] private LayerMask ladderMask;
        [SerializeField] private LayerMask groundMask;

        public string LocalPlayerTag => localPlayerTag;
        public string PlayerTag => playerTag;
        public int LadderMask => ladderMask;
        public int GroundMask => groundMask;

        public void Start() => Singleton = this;
    }
}
