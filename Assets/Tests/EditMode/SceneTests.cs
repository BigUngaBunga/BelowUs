using BelowUs;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    public class SceneTests : IPrebuildSetup
    {
        /**
         * Guarantees that the active scene is the game scene
         */
        public void Setup()
        {
            if (SceneManager.GetActiveScene().buildIndex != 1)
                EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(1));
        }

        
        /**
         * Checks so that all the stations have a player and station tag
         */
        [Test] public void StationTests()
        {
            Station[] stations = Object.FindObjectsOfType<Station>();
            foreach (Station station in stations)
            {
                string stationName = station.gameObject.name;
                Assert.IsFalse(station.PlayerTag == "", stationName + " is missing a PlayerTag!");
                Assert.IsFalse(station.SwitchTag == "", stationName + " is missing a SwitchTag!");
            }
        }

        /**
         * Checks so that there is only one submarine tag in the scene
         */
        [Test] public void SubmarineTests()
        {
            Assert.IsTrue(GameObject.FindGameObjectsWithTag("Submarine").Length == 1, "There are more or less than one Submarine tag in the scene!");
        }

        /**
         * 
         */
        [Test] public void PlayerTests()
        {
            //Checks so that there aren't any player tags in the scene
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Assert.IsNull(player, "There shouldn't be any players in the scene before the game has started!");

            //Checks so that there is only one BackupPlayerSpawner and that it has a player tag
            BackupPlayerSpawner[] spawners = Object.FindObjectsOfType<BackupPlayerSpawner>();
            Assert.IsTrue(spawners.Length == 1);
            if (spawners.Length != 0)
            {
                string spawnerName = spawners[0].gameObject.name;
                Assert.IsNotNull(spawners[0].PlayerPrefab, spawnerName + " is missing a PlayerPrefab!");
                Assert.IsNotNull(spawners[0].NetManager, spawnerName + " is missing a NetManagerPrefab!");
            }
        }
    }
}