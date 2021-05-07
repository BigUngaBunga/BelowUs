using UnityEngine;

namespace BelowUs
{
    public class BotLeftCannon : BaseCannon
    {
        protected override void Start()
        {
            base.Start();
            leftRestrict = 122;
            rightRestrict = 1;
        }

        protected override void Update()
        {
            base.Update();
            if (whichCannon == 1)
            {
                Targeting(transform.position, -90, 180, leftRestrict, rightRestrict);
                Fire();
            }
        }
    }
}