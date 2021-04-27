using NUnit.Framework;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class GameTests
    {
        /* Location path not working so it pollutes the project folder
        [Test]
        public void BuildTest()
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new string[SceneManager.sceneCountInBuildSettings],
                locationPathName = "Test Build",
                target = BuildTarget.StandaloneWindows64 
            };

            for (int i = 0; i < buildPlayerOptions.scenes.Length; i++)
                buildPlayerOptions.scenes[i] = SceneUtility.GetScenePathByBuildIndex(i);

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);

            Assert.IsTrue(report.summary.result == BuildResult.Succeeded, "Build failed");
        }
        */
    }
}