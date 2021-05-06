using UnityEngine;

namespace BelowUs
{
    public class ReferenceManager : MonoBehaviour
    {
        /// <summary>The one and only NetworkManager</summary>
        private static ReferenceManager instance;
        public static ReferenceManager Singleton
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<ReferenceManager>();

                return instance;
            }
        }

        [TagSelector] [SerializeField] private string localPlayerTag;
        [TagSelector] [SerializeField] private string playerTag;
        [TagSelector] [SerializeField] private string submarineTag;
        [TagSelector] [SerializeField] private string enemyTag;

        [SerializeField] private LayerMask ladderMask;
        [SerializeField] private LayerMask groundMask;

        public string LocalPlayerTag => localPlayerTag;
        public string PlayerTag => playerTag;
        public string SubmarineTag => submarineTag;
        public string EnemyTag => enemyTag;

        public int LadderMask => ladderMask;
        public int GroundMask => groundMask;
    }
}