using MyBox;
using UnityEngine;

namespace BelowUs
{
    public class ReferenceManager : MonoBehaviour
    {
        /// <summary>The one and only ReferenceManager</summary>
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

        private void Awake() 
        { 
            if (instance == null)
                DontDestroyOnLoad(gameObject); 
        }

        #region tags
        [Tag] [SerializeField] private string localPlayerTag;
        [Tag] [SerializeField] private string playerTag;
        [Tag] [SerializeField] private string submarineTag;
        [Tag] [SerializeField] private string enemyTag;
        [Tag] [SerializeField] private string stationTag;
        [Tag] [SerializeField] private string untagged;


        public string LocalPlayerTag => localPlayerTag;
        public string PlayerTag => playerTag;
        public string SubmarineTag => submarineTag;
        public string EnemyTag => enemyTag;
        public string StationTag => stationTag;
        public string Untagged => untagged;
        #endregion

        #region masks
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private LayerMask ladderMask;
        [SerializeField] private LayerMask stationMask;

        public int GroundMask => groundMask;
        public int LadderMask => ladderMask;
        public int StationMask => stationMask;
        #endregion
    }
}
