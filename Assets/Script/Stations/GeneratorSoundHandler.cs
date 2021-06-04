using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class GeneratorSoundHandler : NetworkBehaviour
    {
        [SerializeReference] private ShipResource resource;
        [SerializeReference] private AudioSource generatorStart, generatorStop;
        [SerializeField] private float repeatTime;
        private float lastValue;

        [Server]
        private void Start()
        {
            lastValue = resource.CurrentValue;
            InvokeRepeating(nameof(CheckIfPlaySound), repeatTime, repeatTime);
        }

        [Server]
        private void CheckIfPlaySound()
        {
            if (lastValue > 0 && resource.CurrentValue <= 0)
                PlaySound(false);
            else if (lastValue <= 0 && resource.CurrentValue > 0)
                PlaySound(true);

            lastValue = resource.CurrentValue;
        }

        [ClientRpc]
        private void PlaySound(bool generatorStarted)
        {
            if (generatorStarted)
                generatorStart.Play();
            else
                generatorStop.Play();
        }
    }
}

