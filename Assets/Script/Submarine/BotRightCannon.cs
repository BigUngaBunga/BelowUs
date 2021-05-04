using UnityEngine;

public class BotRightCannon : BaseCannon
{
    void Start()
    {
        leftRestrict = 18;
        rightRestrict = -120;
    }
    
    void Update()
    {
        ActiveCannon();

        if (whichCannon == 2)
        {
            Targeting(this.transform.position, -90, 180, leftRestrict, rightRestrict);
            Fire();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!flipped)
            {
                flipped = true;
                leftRestrict = 120;
                rightRestrict = -18;
            }
            else
            {
                flipped = false;
                leftRestrict = 18;
                rightRestrict = -120;
            }
        }
    }
}
