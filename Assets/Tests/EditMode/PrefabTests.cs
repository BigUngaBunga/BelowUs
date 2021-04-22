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

            PlayerCharacterController playerCharCon = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guids[0])).GetComponent<PlayerCharacterController>();
            Assert.IsNotNull(playerCharCon);
            Assert.IsFalse(playerCharCon.MovementSpeed.Value <= 0, ZeroText(playerCharCon.gameObject.name, "Movement Speed"));
            Assert.IsFalse(playerCharCon.JumpForce.Value <= 0, ZeroText(playerCharCon.gameObject.name, "Jump Force"));
            Assert.IsFalse(playerCharCon.ClimbingSpeed.Value <= 0, ZeroText(playerCharCon.gameObject.name, "Climbing Speed"));
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

            //Checks so that all the prefabs have the submarinetag defined
            foreach (EnemyBase script in scripts)
                Assert.IsFalse(script.SubmarineTag == "", script.gameObject.name + " is missing the submarine tag!");
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
                MapGenerator mapGen = mapPrefab.GetComponent<MapGenerator>();

                Assert.IsFalse(mapGen.MinimumOpenWaterPercentage > mapGen.MaximumOpenWaterPercentage, "The MinimumOpenWaterPercentage should not be bigger than the MaximumOpenWaterPercentage!");
                Assert.IsFalse(mapGen.MinimumEnclaveRemovalSize > mapGen.MaximumEnclaveRemovalSize, "The MinimumEnclaveRemovalSize should not be bigger than the MaximumEnclaveRemovalSize!");

                MeshGenerator meshGen = mapPrefab.GetComponent<MeshGenerator>();
                Assert.IsNotNull(meshGen.MeshFilter, "The meshfilter on " + assetPath + " is null!");
            }
        }

        private string ZeroText(string objectName, string varName)
        {
            return "The " + objectName + " prefab has zero or lower " + varName + "!";
        }
    }
}
