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

        void Update()
        {
            ActiveCannon();

            if (whichCannon == 4)
            {
                Targeting(this.transform.position, 90, 0, leftRestrict, rightRestrict);
                Fire();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!flipped)
                {
                    flipped = true;
                    leftRestrict = -15;
                    rightRestrict = -135;
                }
                else
                {
                    flipped = false;
                    leftRestrict = 135;
                    rightRestrict = 15;
                }
            }
        }
    }
}