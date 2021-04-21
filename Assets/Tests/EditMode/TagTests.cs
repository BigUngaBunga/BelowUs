using BelowUs;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class TagTests
    {
        [Test]
        public void TagTestSimplePasses()
        {
            EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(1));

            //Checks so that there is only one submarine tag in the scene
            Assert.IsTrue(GameObject.FindGameObjectsWithTag("Submarine").Length == 1);

            //Checks so that there aren't any player tags in the scene
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Assert.IsNull(player);

            //Checks so that all the stations have a player and station tag
            Station[] stations = Object.FindObjectsOfType<Station>();
            foreach (Station station in stations)
            {
                Assert.IsFalse(station.PlayerTag == "");
                Assert.IsFalse(station.SwitchTag == "");
            }

            //Checks so that there is only one TestingSpawner and that it has a player tag
            BackupPlayerSpawner[] spawners = Object.FindObjectsOfType<BackupPlayerSpawner>();
            Assert.IsTrue(spawners.Length == 1);
            if (spawners.Length != 0)
            {
                Assert.IsNotNull(spawners[0].PlayerPrefab);
                Assert.IsNotNull(spawners[0].NetManager);
            }

            //Transfer this to a playmode test
            //Assert.IsTrue(GameObject.FindGameObjectsWithTag("Player").Length == NetworkManager.singleton.numPlayers);
        }
    }
}