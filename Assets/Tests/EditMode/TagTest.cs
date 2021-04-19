using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class TagTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void TagTestSimplePasses()
        {
            EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(1));

            //Checks so that there is only one submarine tag in the scene
            Assert.IsTrue(GameObject.FindGameObjectsWithTag("Submarine").Length == 1);

            //Checks so that there aren't any player tags in the scene
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Assert.IsNull(player);

            //Transfer this to a playmode test
            //Assert.IsTrue(GameObject.FindGameObjectsWithTag("Player").Length == NetworkManager.singleton.numPlayers);
        }
    }
}