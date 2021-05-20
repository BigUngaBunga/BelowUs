using Mirror;
using UnityEngine;

namespace BelowUs
{
    public class VillageController : GeneratorController
    {
        [SerializeReference] private GameObject stationUI;

        protected override void Start() => SetUIActive(false);

        protected override void SetUIActive(bool setActive) => stationUI.SetActive(setActive);
    }
}

