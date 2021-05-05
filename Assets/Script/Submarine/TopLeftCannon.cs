using UnityEngine;

namespace BelowUs
{
    public class TopLeftCannon : BaseCannon
    {
        void Start()
        {
            leftRestrict = -15;
            rightRestrict = -135;
        }

        void Update()
        {
            ActiveCannon();

            if (whichCannon == 3)
            {
                Targeting(transform.position, 90, 0, leftRestrict, rightRestrict);
                Fire();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!flipped)
                {
                    flipped = true;
                    leftRestrict = 135;
                    rightRestrict = 15;
                }
                else
                {
                    flipped = false;
                    leftRestrict = -15;
                    rightRestrict = -135;
                }
            }
        }
    }
}