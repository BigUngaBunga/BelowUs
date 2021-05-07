using UnityEngine;

namespace BelowUs
{
    public class TopRightCannon : BaseCannon
    {
        protected override void Start()
        {
            base.Start();
            leftRestrict = 135;
            rightRestrict = 15;
        }

        protected override void Update()
        {
            base.Update();

            if (whichCannon == 4)
            {
                Targeting(transform.position, 90, 0, leftRestrict, rightRestrict);
                Fire();
            }
        }
    }
}