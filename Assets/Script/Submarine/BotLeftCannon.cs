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

        void Update()
        {
            ActiveCannon();
            if (whichCannon == 1)
            {
                Targeting(this.transform.position, -90, 180, leftRestrict, rightRestrict);
                Fire();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!flipped)
                {
                    flipped = true;
                    leftRestrict = -1;
                    rightRestrict = -122;
                }
                else
                {
                    flipped = false;
                    leftRestrict = 122;
                    rightRestrict = 1;
                }
            }
        }
    }
}