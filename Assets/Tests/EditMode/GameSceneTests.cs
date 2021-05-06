using BelowUs;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    public class GameSceneTests : IPrebuildSetup
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
            StationController[] stations = Object.FindObjectsOfType<StationController>();
            foreach (StationController station in stations)
            {
                string stationName = station.gameObject.name;
                string errorMsg = stationName + " is missing a ";
                Assert.IsNotNull(station.Controller, errorMsg + "Camera Controller!");
                Assert.IsNotNull(station.LeaveButton, errorMsg + "Leave Button!");

                Assert.IsFalse(station.SwitchTag == "", errorMsg + "SwitchTag!");

                Assert.IsTrue(station.CompareTag("Station"), stationName + " does not have a station tag!");
            }
        }

        /**
         * Checks so that there is only one submarine tag in the scene
         */
        [Test]
        public void SubmarineTests() => Assert.IsTrue(GameObject.FindGameObjectsWithTag("Submarine").Length == 1, "There are more or less than one Submarine tag in the scene!");

        /**
         * 
         */
        [Test] public void PlayerTests()
        {
            //Checks so that there aren't any player tags in the scene
            GameObject player = GameObject.FindGameObjectWithTag(ReferenceManager.Singleton.PlayerTag);
            Assert.IsNull(player, "There shouldn't be any players in the scene before the game has started!");

            GameObject localPlayer = GameObject.FindGameObjectWithTag(ReferenceManager.Singleton.LocalPlayerTag);
            Assert.IsNull(localPlayer, "There shouldn't be any players in the scene before the game has started!");

            //Checks so that there is only one BackupPlayerSpawner and that it has a player tag
            BackupPlayerSpawner[] spawners = Object.FindObjectsOfType<BackupPlayerSpawner>();
            Assert.IsTrue(spawners.Length == 1);
            if (spawners.Length != 0)
            {
                string spawnerName = spawners[0].gameObject.name;
                Assert.IsNotNull(spawners[0].NetManager, spawnerName + " is missing a NetManagerPrefab!");
            }
        }

        [Test] public void MapTests()
        {
            //Checks so that there is one MapHandler in the scene in the scene
            MapHandler[] mapHandlers = Object.FindObjectsOfType<MapHandler>();
            Assert.IsTrue(mapHandlers.Length > 0, "There is no map handler in the scene!");
            Assert.IsTrue(mapHandlers.Length < 2, "There are more than one map handlers in the scene!");

            Assert.IsNotNull(mapHandlers[0].MapPrefab, "The map handler is missing a map prefab!");
        }

        [Test] public void SubmarineInsideTests()
        {
            //TODO fix this so that it actually finds the submarine floor in the scene
            //Assert.IsTrue(GameObject.Find("Floor").layer == ReferenceManager.Singleton.GroundMask, "The floor should have " + ReferenceManager.Singleton.GroundMask + "!");

        }
    }
}