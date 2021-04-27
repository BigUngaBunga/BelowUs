using System.Collections;
using System.Collections.Generic;
using Mirror;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class ServerTest
{
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    //TODO fix the errors
    public IEnumerator ServerPlayModeTest()
    {
        var Obj = new GameObject();
        var networkMgr = Obj.AddComponent<NetworkManager>();
        var hud = Obj.AddComponent<NetworkManagerHUDCustom>();
        hud.HostClicked();

        yield return new WaitForSeconds(1);
        Assert.IsTrue(SceneManager.GetActiveScene().buildIndex == 1); //Asserts that the scene has changed to the game scene
        Assert.IsTrue(networkMgr.isNetworkActive); //Asserts that the server has been started
    }
}
