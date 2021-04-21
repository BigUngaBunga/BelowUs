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
            Assert.IsTrue(foundAssets == 1, "Found more than one player asset. Printing the names of all of them!");
            if (foundAssets > 1)
                foreach (string gui in guids)
                    Debug.LogError(AssetDatabase.GUIDToAssetPath(gui));

            PlayerCharacterController playerCharCon = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guids[0])).GetComponent<PlayerCharacterController>();
            Assert.IsFalse(playerCharCon.MovementSpeed.Value == 0, ZeroText(playerCharCon.gameObject.name, "Movement Speed"));
            Assert.IsFalse(playerCharCon.JumpForce.Value == 0, ZeroText(playerCharCon.gameObject.name, "Jump Force"));
            Assert.IsFalse(playerCharCon.ClimbingSpeed.Value == 0, ZeroText(playerCharCon.gameObject.name, "Climbing Speed"));
        }

        [Test]
        public void EnemyPrefabTests()
        {
            //Grabs all the enemy prefabs
            string[] guids = AssetDatabase.FindAssets("", new[] { "Assets/Prefabs/Enemies" });
            List<EnemyBase> scripts = new List<EnemyBase>();

            //Grabs all the scripts from the enemies
            foreach (string guid in guids)
                scripts.Add(AssetDatabase.LoadAssetAtPath<EnemyBase>(AssetDatabase.GUIDToAssetPath(guid)).GetComponent<EnemyBase>());

            //Checks so that all the prefabs have the submarinetag defined
            foreach (EnemyBase script in scripts)
                Assert.IsFalse(script.SubmarineTag == "", script.gameObject.name + " is missing the submarine tag!");

            //Checks so that all the prefabs have an assigned speed
            foreach (EnemyBase script in scripts)
            {
                Assert.IsFalse(script.MoveSpeedChasing == 0, ZeroText(script.gameObject.name, "Move Speed Chasing"));
                Assert.IsFalse(script.MoveSpeedPatrolling == 0, ZeroText(script.gameObject.name, "Move Speed Patrolling"));
                Assert.IsFalse(script.MaxPatrolRange == 0, ZeroText(script.gameObject.name, "Max Patrol Range"));
            }
        }

        private string ZeroText(string objectName, string varName)
        {
            return "The " + objectName + " prefab has zero " + varName + "!";
        }
    }
}
