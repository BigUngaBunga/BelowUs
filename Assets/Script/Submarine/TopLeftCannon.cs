using UnityEngine;

namespace BelowUs
{
    public class TopLeftCannon : BaseCannon
    {
        protected override void Start()
        {
            base.Start();
            leftRestrict = -15;
            rightRestrict = -135;
        }

        protected override void Update()
        {
            base.Update();

            if (whichCannon == 3)
            {
                Targeting(transform.position, 90, 0, leftRestrict, rightRestrict);
                Fire();
            }
        }
    }
}