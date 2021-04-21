using BelowUs;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class FloatReferenceTests
{
    [Test]
    public void FloatReferenceTestsSimplePasses()
    {
        string[] guids = AssetDatabase.FindAssets("Player", new[] { "Assets/Prefabs" });

        int foundAssets = guids.Length;
        if (foundAssets > 1)
        {
            Debug.LogError("Found more than one player asset. Printing the names of all of them!");
            foreach (string gui in guids)
                Debug.LogError(AssetDatabase.GUIDToAssetPath(gui));
        }

        Assert.IsTrue(foundAssets == 1);

        PlayerCharacterController playerCharacterController = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guids[0])).GetComponent<PlayerCharacterController>();
        Assert.IsFalse(playerCharacterController.MovementSpeed.Value == 0);
        Assert.IsFalse(playerCharacterController.JumpForce.Value == 0);
        Assert.IsFalse(playerCharacterController.ClimbingSpeed.Value == 0);
    }
}
