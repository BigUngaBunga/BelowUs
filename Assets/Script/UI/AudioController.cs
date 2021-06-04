using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace BelowUs
{
    public class AudioController : MonoBehaviour
    {
        [SerializeReference] AudioMixer mixer;

        public void SetMasterVolume(float value) => mixer.SetFloat("MasterVolume", GetLogarithmicSound(value));

        private float GetLogarithmicSound(float value) => Mathf.Log10(value) * 20;
    }
}


