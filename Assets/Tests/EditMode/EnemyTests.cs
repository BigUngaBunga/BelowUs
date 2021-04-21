using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyTests
{
    [Test]
    public void EnemyTestsSimplePasses()
    {
        //Grabs all the enemy prefabs
        string[] guids = AssetDatabase.FindAssets("", new[] { "Assets/Prefabs/Enemies" });
        List<GameObject> files = new List<GameObject>();

        //Grabs all the scripts from the enemies
        foreach (string guid in guids)
            files.Add(AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid)));

        //Checks so that all the prefabs have the submarinetag defined
        foreach (GameObject prefab in files)
        {
            EnemyBase script = prefab.GetComponent<EnemyBase>();
            bool incorrect = script.SubmarineTag == "";

            if (incorrect)
                Debug.Log(prefab.name + " is missing the submarine tag!!");

            Assert.IsFalse(incorrect);
        }
    }
}