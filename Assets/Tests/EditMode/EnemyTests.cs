using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class EnemyTests
{
    [Test]
    public void EnemyTestsSimplePasses()
    {
        //Checks so that all the enemies have a submarine tag
        /*
        GameObject bubba = new GameObject();
        Component bubbaScript = bubba.AddComponent(typeof(BubbaScript));

        List<EnemyBase> derivedScripts = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(EnemyBase)))
                .Select(type => Activator.CreateInstance(type) as EnemyBase).ToList();
        */

        string scriptType = "t:" + nameof(EnemyBase);
        //, new[] { "Assets/Script/Enemy" }
        string[] guids = AssetDatabase.FindAssets(scriptType);
        foreach (string guid in guids)
        {
            Debug.Log("ScriptObj: " + AssetDatabase.GUIDToAssetPath(guid));
        }
    }
}
