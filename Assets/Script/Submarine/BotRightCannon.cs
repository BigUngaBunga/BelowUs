using UnityEngine;

namespace BelowUs
{
    public class BotRightCannon : BaseCannon
    {
        protected override void Start()
        {
            base.Start();
            leftRestrict = 18;
            rightRestrict = -120;
        }

        protected override void Update()
        {
            base.Update();

            if (whichCannon == 2)
            {
                Targeting(transform.position, -90, 180, leftRestrict, rightRestrict);
                Fire();
            }
        }
    }
}