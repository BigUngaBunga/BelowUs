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

        private void Awake() => DontDestroyOnLoad(gameObject);

        #region transforms
        [SerializeField] private Transform bulletParent;
        [SerializeField] private Transform goldParent;
        [SerializeField] private Transform scrapParent;

        public Transform BulletParent => bulletParent;
        public Transform GoldParent => goldParent;
        public Transform ScrapParent => scrapParent;
        #endregion

        #region tags
        [TagSelector] [SerializeField] private string localPlayerTag;
        [TagSelector] [SerializeField] private string playerTag;
        [TagSelector] [SerializeField] private string submarineTag;
        [TagSelector] [SerializeField] private string enemyTag;
        [TagSelector] [SerializeField] private string stationTag;
        [TagSelector] [SerializeField] private string untagged;


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
