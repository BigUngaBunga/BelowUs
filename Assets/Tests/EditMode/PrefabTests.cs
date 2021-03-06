using BelowUs;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class PrefabTests
    {
        [Test]
        public void PlayerPrefabTests()
        {
            string[] guids = AssetDatabase.FindAssets("Player", new[] { "Assets/Prefabs" });

            int foundAssets = guids.Length;
            Assert.IsTrue(foundAssets == 1, "Found more or less than one player asset. Printing the names of all of them!");
            if (foundAssets > 1)
                foreach (string gui in guids)
                    Debug.LogError(AssetDatabase.GUIDToAssetPath(gui));

            PlayerSetup playerSetup = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guids[0])).GetComponent<PlayerSetup>();
            Assert.IsNotNull(playerSetup);
            Assert.IsFalse(playerSetup.MoveSpeed.Value <= 0, ZeroText(playerSetup.gameObject.name, "Move Speed"));
            Assert.IsFalse(playerSetup.JumpForce.Value <= 0, ZeroText(playerSetup.gameObject.name, "Jump Force"));
            Assert.IsFalse(playerSetup.ClimbSpeed.Value <= 0, ZeroText(playerSetup.gameObject.name, "Climb Speed"));
        }

        [Test]
        public void EnemyPrefabTests()
        {
            //Grabs all the enemy prefabs
            string[] guids = AssetDatabase.FindAssets("", new[] { "Assets/Prefabs/Enemies" });
            List<EnemyBase> scripts = new List<EnemyBase>();

            //Grabs all the scripts from the enemies
            foreach (string guid in guids)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                EnemyBase eBase = prefab.GetComponent<EnemyBase>();
                Assert.IsNotNull(eBase, "The prefab " + prefab.name + " is missing an enemy script or should not be in the enemy prefab folder!");
                scripts.Add(eBase);
            }
        }

        [Test]
        public void MapPrefabTests()
        {
            string[] guids = AssetDatabase.FindAssets("Map", new[] { "Assets/Prefabs/Maps" });

            int foundAssets = guids.Length;

            for (int i = 0; i < foundAssets; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                GameObject mapPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                ReefGenerator mapGen = mapPrefab.GetComponent<ReefGenerator>();

                Assert.IsFalse(mapGen.MinimumOpenWaterPercentage > mapGen.MaximumOpenWaterPercentage, "The MinimumOpenWaterPercentage should not be bigger than the MaximumOpenWaterPercentage!");
                Assert.IsFalse(mapGen.MinimumEnclaveRemovalSize > mapGen.MaximumEnclaveRemovalSize, "The MinimumEnclaveRemovalSize should not be bigger than the MaximumEnclaveRemovalSize!");

                MeshGenerator meshGen = mapPrefab.GetComponent<MeshGenerator>();
                Assert.IsNotNull(meshGen.MeshFilter, "The meshfilter on " + assetPath + " is null!");
            }
        }

        private string ZeroText(string objectName, string varName) => "The " + objectName + " prefab has zero or lower " + varName + "!";
    }
}
