using UnityEngine;

namespace BelowUs
{
    public class TagManager : MonoBehaviour
    {
        /// <summary>The one and only NetworkManager</summary>
        public static TagManager Singleton { get; private set; }

        [TagSelector] [SerializeField] private string localPlayerTag;
        [TagSelector] [SerializeField] private string playerTag;

        public string LocalPlayerTag => localPlayerTag;
        public string PlayerTag => playerTag;

        public void Start() => Singleton = this;
    }
}
