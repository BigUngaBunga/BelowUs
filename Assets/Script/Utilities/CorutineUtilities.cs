using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BelowUs
{
    public static class CorutineUtilities
    {
        private static float timeSinceLastCall; 
        public static float TimeInSeconds { get { return DateTime.Now.Second + DateTime.Now.Millisecond / 1000f; } }

        public static void UpdateTimeSinceLastCall()
        {
            timeSinceLastCall = TimeInSeconds;
        }

        public static WaitForSeconds Wait(float timeToWait, string textToWrite = "")
        {
            //Disabled debugging measure
            //remove "&& textToWrite == "" to reactivate
            if (textToWrite != "" )//&& textToWrite == "")
            {
                float time = TimeInSeconds - timeSinceLastCall;
                UpdateTimeSinceLastCall();
                if (time > 0.05f)
                    Debug.Log($"{textToWrite} took {time} seconds");
            }

            return new WaitForSeconds(timeToWait);
        }

        public static bool WaitAmountOfTimes(int counter, int total, int timesToPass)
        {
            if (total / timesToPass > 0)
                return counter % (total / timesToPass) <= 1;

            return false;
        }
    }
}

